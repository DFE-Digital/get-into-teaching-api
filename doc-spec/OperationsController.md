# OperationsController

**File:** `GetIntoTeachingApi/Controllers/OperationsController.cs`

A controller for operational/admin endpoints: health checks, CRM integration pause/resume, and Apply candidate backfill jobs.

---

## Endpoints

| Method | Route | Auth | Purpose |
|--------|-------|------|---------|
| `GET` | `/api/operations/generate_mapping_info` | None | Reflection-based CRM entity mapping info |
| **`GET`** | **`/api/operations/health_check`** | **None** | **Composite health check across 5 dependencies** |
| `PUT` | `/api/operations/pause_crm_integration` | `Admin, Crm` | Pause CRM calls for 6 hours |
| `PUT` | `/api/operations/resume_crm_integration` | `Admin, Crm` | Resume CRM calls immediately |
| `POST` | `/api/operations/backfill_apply_candidates` | `Admin` | Backfill Apply candidates from a date |
| `POST` | `/api/operations/backfill_apply_candidates_from_ids` | `Admin` | Backfill specific Apply candidate IDs |

---

## `GET /api/operations/health_check`

**Purpose:** Probe the health of all external dependencies the API relies on and report a composite status.

### Flow

```
Client → GET /api/operations/health_check
  → Check Hangfire (synchronous)
  → Check Database (Postgres — async)
  → Check Crm (Dynamics 365 — synchronous)
  → Check Notify (GOV.UK Notify — async)
  → Check Redis (async)
  → Compute composite Status (healthy / degraded / unhealthy)
  → Return 200 OK with all statuses
```

#### Controller code

```csharp
[HttpGet]
[Route("health_check")]
public async Task<IActionResult> HealthCheck()
{
    var response = new HealthCheckResponse()
    {
        GitCommitSha = _env.GitCommitSha,
        Environment = _env.EnvironmentName,
        Hangfire = _hangfire.CheckStatus(),
        Database = await _store.CheckStatusAsync(),
        Crm = _crm.CheckStatus(),
        Notify = await _notifyService.CheckStatusAsync(),
        Redis = await _redis.CheckStatusAsync(),
    };

    return Ok(response);
}
```

### Response shape

```json
{
  "gitCommitSha": "abc123def",
  "environment": "Production",
  "database": "ok",
  "hangfire": "ok",
  "crm": "ok",
  "redis": "ok",
  "notify": "ok",
  "status": "healthy"
}
```

| Field | Type | Source | Description |
|-------|------|--------|-------------|
| `gitCommitSha` | `string` | `IEnv.GitCommitSha` → env var `GIT_COMMIT_SHA` | The git commit the running build was compiled from |
| `environment` | `string` | `IEnv.EnvironmentName` → env var `ASPNETCORE_ENVIRONMENT` | Current environment (`Development`, `Staging`, `Production`, or `Test`) |
| `database` | `string` | `IStore.CheckStatusAsync()` — opens/closes a Postgres connection | Postgres database reachability |
| `hangfire` | `string` | `IHangfireService.CheckStatus()` — checks if any server is processing the `Default` queue | Hangfire worker availability |
| `crm` | `string` | `ICrmService.CheckStatus()` — pings Dynamics 365 via `GetEntityDisplayName("contact")` | CRM (Dataverse) reachability |
| `redis` | `string` | `IRedisService.CheckStatusAsync()` — pings Redis via `PingAsync()` | Redis reachability |
| `notify` | `string` | `INotifyService.CheckStatusAsync()` — calls GOV.UK Notify `GetAllTemplatesAsync()` | GOV.UK Notify email service reachability |
| `status` | `string` | Computed by `HealthCheckResponse.Status` (see below) | `"healthy"`, `"degraded"`, or `"unhealthy"` |

Each individual service field returns `"ok"` on success, or the exception message string on failure. CRM may also return `"integration-paused"`.

---

### Composite status logic

The `HealthCheckResponse.Status` property classifies services into two tiers:

```csharp
public const string StatusOk = "ok";

public IEnumerable<string> CriticalServices => new[] { Database, Hangfire };
public IEnumerable<string> NonCriticalServices => new[] { Crm, Notify, Redis };

public string Status
{
    get
    {
        if (Services.All(s => s == StatusOk))
            return "healthy";

        if (NonCriticalServices.Any(s => s != StatusOk) && CriticalServices.All(s => s == StatusOk))
            return "degraded";

        return "unhealthy";
    }
}
```

| Status | Condition |
|--------|-----------|
| `"healthy"` | All 5 services report `"ok"` |
| `"degraded"` | Both critical services (Database, Hangfire) are `"ok"`, but one or more non-critical services (CRM, Notify, Redis) are failing |
| `"unhealthy"` | Any critical service (Database or Hangfire) is failing |

**Critical vs non-critical rationale:**
- **Database (Postgres)** and **Hangfire** are critical because the API can't serve cached data or process background jobs without them.
- **CRM**, **Notify**, and **Redis** are non-critical because the API can operate in a degraded state (e.g., CRM paused, emails fire-and-forget, Redis dedup skipped).

---

### Individual health checks

#### Database (`IStore.CheckStatusAsync()`)

```csharp
public async Task<string> CheckStatusAsync()
{
    try
    {
        await _dbContext.Database.OpenConnectionAsync();
        await _dbContext.Database.CloseConnectionAsync();
    }
    catch (Exception e)
    {
        return e.Message;
    }

    return HealthCheckResponse.StatusOk;
}
```

**What it does:** Opens and immediately closes a raw ADO.NET connection to the Postgres database. If the `OpenConnectionAsync()` call succeeds, Postgres is reachable and accepting connections.

**File:** `Services/Store.cs:50`

---

#### Hangfire (`IHangfireService.CheckStatus()`)

```csharp
public string CheckStatus()
{
    try
    {
        var servers = _jobStorage.GetMonitoringApi().Servers();
        bool queueIsBeingProcessed = servers.Any(
            s => s.Queues.Contains("Default", StringComparer.InvariantCultureIgnoreCase));
        return queueIsBeingProcessed ? HealthCheckResponse.StatusOk : "No workers are processing the Default queue!";
    }
    catch (Exception e)
    {
        return e.Message;
    }
}
```

**What it does:** Queries Hangfire's monitoring API for registered servers and checks if any of them list the `"Default"` queue. If no Hangfire worker is running, background jobs (candidate upserts, syncs) will never execute.

**File:** `Services/HangfireService.cs:17`

---

#### CRM (`ICrmService.CheckStatus()`)

```csharp
public string CheckStatus()
{
    if (_appSettings.IsCrmIntegrationPaused)
        return HealthCheckResponse.StatusIntegrationPaused;

    if (_previousStatus != null && _dateTime.UtcNow.Subtract(_previousStatusCheckAt) < _statusCheckInterval)
        return _previousStatus;

    _previousStatusCheckAt = _dateTime.UtcNow;
    return _previousStatus = _service.CheckStatus();
}
```

Delegates to `IOrganizationServiceAdapter.CheckStatus()`:

```csharp
public string CheckStatus()
{
    try
    {
        _client.GetEntityDisplayName("contact");
    }
    catch (Exception e)
    {
        return e.Message;
    }
    return HealthCheckResponse.StatusOk;
}
```

**What it does:** Pings Dynamics 365 by fetching the display name of the `"contact"` entity. This verifies the CRM is reachable and the authenticated connection is working.

**Rate-limiting:** The result is cached for 1 minute (`_statusCheckInterval`) to avoid hammering the CRM on every health check poll. If the integration is paused (via `PUT /api/operations/pause_crm_integration`), returns `"integration-paused"` immediately without calling CRM.

**Files:** `Services/CrmService.cs:46`, `Adapters/OrganizationServiceAdapter.cs:25`

---

#### Notify (`INotifyService.CheckStatusAsync()`)

```csharp
public async Task<string> CheckStatusAsync() =>
    await _client.CheckStatusAsync(ApiKey());
```

Delegates to `INotificationClientAdapter.CheckStatusAsync()`:

```csharp
public async Task<string> CheckStatusAsync(string apiKey)
{
    try
    {
        var client = Client(apiKey);
        await client.GetAllTemplatesAsync();
    }
    catch (Exception e)
    {
        return e.Message;
    }
    return HealthCheckResponse.StatusOk;
}
```

**What it does:** Calls the GOV.UK Notify API's `GetAllTemplatesAsync()` endpoint. This verifies the API key is valid and the Notify service is reachable. The `NotificationClient` is lazily created and cached per API key.

**Files:** `Services/NotifyService.cs:27`, `Adapters/NotificationClientAdapter.cs:18`

---

#### Redis (`IRedisService.CheckStatusAsync()`)

```csharp
public async Task<string> CheckStatusAsync()
{
    try
    {
        await _redis.GetServer(_endpoint).PingAsync();
        return HealthCheckResponse.StatusOk;
    }
    catch (Exception e)
    {
        return e.Message;
    }
}
```

**What it does:** Sends a `PING` command to the Redis server. If the server responds, Redis is reachable. Used primarily for Hangfire job deduplication and as Hangfire's job storage in production.

**File:** `Services/RedisService.cs:31`

---

### Response codes

| Status | When |
|--------|------|
| `200 OK` | Always — the endpoint returns `200` regardless of individual service health (the composite `status` field in the body communicates the severity) |

### Auth

| Attribute | Value |
|-----------|-------|
| Controller-level | None (no `[Authorize]` on the controller class) |
| Endpoint-level | None (no `[Authorize]` on this action) |

**The health check is completely unauthenticated.** Any client can call it without credentials.

**Note:** Other endpoints in this controller (`pause_crm_integration`, `resume_crm_integration`, `backfill_*`) are `[Authorize]` protected with `Admin` or `Crm` roles, but the health check and mapping info endpoints are public.

---

### Data flow diagram

```
         ┌─────────────────────────────────────────────┐
         │              GET /api/operations/health_check │
         │              (no auth required)               │
         └─────────────────────┬───────────────────────┘
                               │
                    ┌──────────┴──────────┐
                    ▼                     ▼
            IEnv (env vars)      HealthCheckResponse
            ├─ GitCommitSha      (model object)
            └─ EnvironmentName
                                      │
             ┌────────────────────────┼────────────────────────┐
             │           │            │            │           │
             ▼           ▼            ▼            ▼           ▼
        Hangfire    Database       CRM          Notify      Redis
        CheckStatus CheckStatus  CheckStatus  CheckStatus  CheckStatus
           │           │            │            │           │
           │    Open/close     Ping contact  GetAllTemplates  Ping
           │    Postgres conn  entity in     from GOV.UK      Redis
           │                  Dynamics 365   Notify API       server
           │           │            │            │           │
           ▼           ▼            ▼            ▼           ▼
        ┌────────────────────────────────────────────────────────┐
        │              HealthCheckResponse                       │
        │  { gitCommitSha, environment,                          │
        │    database, hangfire, crm, redis, notify,             │
        │    status: "healthy" | "degraded" | "unhealthy" }     │
        └────────────────────────┬───────────────────────────────┘
                                 │
                                 ▼
                         200 OK (always)
```

---

### Dependencies

| Service | Method called | What it checks |
|---------|--------------|----------------|
| `IEnv` | `.GitCommitSha`, `.EnvironmentName` | Reads env vars `GIT_COMMIT_SHA` and `ASPNETCORE_ENVIRONMENT` |
| `IHangfireService` | `.CheckStatus()` | Queries Hangfire `JobStorage.GetMonitoringApi().Servers()` for any worker processing the `Default` queue |
| `IStore` | `.CheckStatusAsync()` | Opens/closes a Postgres connection via `_dbContext.Database.OpenConnectionAsync()` |
| `ICrmService` | `.CheckStatus()` | Pings Dynamics 365 via `_client.GetEntityDisplayName("contact")`, with 1-minute rate limiting and pause detection |
| `INotifyService` | `.CheckStatusAsync()` | Calls GOV.UK Notify `GetAllTemplatesAsync()` via `NotificationClient` |
| `IRedisService` | `.CheckStatusAsync()` | Pings the Redis server via `ConnectionMultiplexer.GetServer().PingAsync()` |
| `HealthCheckResponse` | `.Status` (computed) | Classifies services into critical (Database, Hangfire) and non-critical (CRM, Notify, Redis) and computes the composite status |
