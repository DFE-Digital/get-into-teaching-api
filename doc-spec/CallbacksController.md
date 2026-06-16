# CallbacksController

**File:** `GetIntoTeachingApi/Controllers/GetIntoTeaching/CallbacksController.cs`

A controller for the "Get into Teaching" callback booking flow. Contains 3 endpoints, all under `POST /api/get_into_teaching/callbacks`.

---

## 1. `POST /api/get_into_teaching/callbacks` — `Book()`

**Purpose:** Accept a callback request and queue it for asynchronous processing. The caller gets an immediate `204`; the actual CRM write happens later in a Hangfire background job.

### Request path (synchronous)

```
Client → POST /api/get_into_teaching/callbacks (body: GetIntoTeachingCallback)
```

```csharp
public IActionResult Book(GetIntoTeachingCallback request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    request.DateTimeProvider = _dateTime;
    string json = request.Candidate.SerializeChangeTracked();
    _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

    return NoContent();
}
```

#### Step-by-step

| Step | Code | What happens |
|------|------|-------------|
| 1 | `ModelState.IsValid` | Validates the request body against data annotations. Returns `400 Bad Request` with error details if invalid. |
| 2 | `request.DateTimeProvider = _dateTime` | Injects `IDateTimeProvider` into the DTO. The DTO's `CreateCandidate()` method uses `DateTimeProvider.UtcNow` to timestamp the privacy policy acceptance. This injection exists so contract tests can freeze time. |
| 3 | `request.Candidate` | A `[JsonIgnore]` computed property that calls `CreateCandidate()` to build a `Candidate` CRM entity from the flat DTO fields (see below). |
| 4 | `.SerializeChangeTracked()` | Serializes only the properties that were explicitly set (changed) into a compact JSON string. Unset properties (null/default) are omitted. |
| 5 | `_jobClient.Enqueue<UpsertCandidateJob>(...)` | Enqueues a Hangfire background job with the JSON payload. The second argument is always `null` (channel type parameter, unused here). |
| 6 | `return NoContent()` | Returns `204 No Content` immediately. The HTTP response is sent before the Hangfire job starts. |

#### What `CreateCandidate()` builds

```csharp
private Candidate CreateCandidate()
{
    var candidate = new Candidate()
    {
        Id = CandidateId,
        Email = Email,
        FirstName = FirstName,
        LastName = LastName,
        AddressTelephone = AddressTelephone,
    };
    candidate.ConfigureChannel(candidateId: CandidateId, primaryContactChannel: this);
    SchedulePhoneCall(candidate);
    AcceptPrivacyPolicy(candidate);
    return candidate;
}
```

It constructs a `Candidate` CRM entity with:

- **Basic fields** — Id, Email, FirstName, LastName, AddressTelephone (from the request body)
- **Channel metadata** (via `ConfigureChannel`) — tracks how this candidate was created:
  - `DefaultContactCreationChannel` = `GetIntoTeachingCallback` channel ID
  - `DefaultCreationChannelSourceId` = `GITWebsite` source ID
  - `DefaultCreationChannelServiceId` = `MailingList` service ID
- **PhoneCall child entity** (if `PhoneCallScheduledAt` is provided):
  - Telephone = candidate's telephone
  - Destination = UK
  - ScheduledAt = the requested time
  - ChannelId = `WebsiteCallbackRequest`
  - Subject = `"Scheduled phone call requested by {FullName}"`
  - TalkingPoints = free-text notes from the user
- **PrivacyPolicy child entity** (if `AcceptedPolicyId` is provided):
  - AcceptedPolicyId = the privacy policy version the user consented to
  - AcceptedAt = current UTC time (using the injected `DateTimeProvider`)

### Response

| Status | When |
|--------|------|
| `204 No Content` | Success — request accepted and queued |
| `400 Bad Request` | `ModelState` validation failed |

---

### Background path (Hangfire job — asynchronous)

```
Hangfire worker picks up the job later
  → UpsertCandidateJob.Run(json, context)
```

```csharp
public void Run(string json, PerformContext context)
{
    var candidate = json.DeserializeChangeTracked<Candidate>();

    if (Deduplicate(Signature(candidate), context, _contextAdapter))
        return;                               // skip if duplicate job already queued

    if (_appSettings.IsCrmIntegrationPaused)
        throw new InvalidOperationException(); // retry later

    if (IsLastAttempt(context, _contextAdapter))
        SendFailureEmail(candidate.Email);     // last retry: notify user, exit cleanly
    else
        _upserter.Upsert(candidate);           // normal path: write to CRM
}
```

#### Step-by-step

| Step | What happens |
|------|-------------|
| 1 | **Deserialize** — the change-tracked JSON is deserialized back into a `Candidate` object with only the previously-set properties populated |
| 2 | **Deduplicate** — computes a signature from `{candidate.Id}-{candidate.Email}-{changedPropertyNames}` and checks Redis. If an identical job is already in the queue, skip (prevents duplicate processing when the user resubmits the same form) |
| 3 | **CRM pause check** — if `CrmIntegrationPausedUntil` is set (via the operations endpoint), throws `InvalidOperationException`. Hangfire catches the exception and retries the job later |
| 4 | **Last attempt check** — if this is the final Hangfire retry (24th in production, 5th in dev), skips the upsert entirely and sends a failure email via GOV.UK Notify using template `CandidateRegistrationFailedEmailTemplateId` |
| 5 | **Upsert to CRM** — calls `CandidateUpserter.Upsert(candidate)` (see below) |
| 6 | **Metrics** — records Prometheus histogram `HangfireJobQueueDuration` labelled with `"UpsertCandidateJob"` |

#### `CandidateUpserter.Upsert()` — the CRM write path

```csharp
public void Upsert(Candidate candidate)
{
    var registrations = ClearTeachingEventRegistrations(candidate);
    var phoneCall = ClearPhoneCall(candidate);
    var privacyPolicy = ClearPrivacyPolicy(candidate);
    var qualifications = ClearQualifications(candidate);
    var pastTeachingPositions = ClearPastTeachingPositions(candidate);
    var applicationForms = ClearApplicationForms(candidate);
    var schoolExperiences = ClearSchoolExperiences(candidate);
    var contactChannelCreations = ClearContactChannelCreations(candidate);

    PreventCandidateDataFromBeingOverwritten(candidate);
    UpdateEventSubscriptionType(candidate);

    SaveCandidate(candidate);
    SaveQualifications(qualifications, candidate);

    SavePastTeachingPositions(pastTeachingPositions, candidate);
    SaveApplicationForms(applicationForms, candidate);
    SaveTeachingEventRegistrations(registrations, candidate);
    SavePrivacyPolicy(privacyPolicy, candidate);
    SavePhoneCall(phoneCall, candidate);
    SaveSchoolExperiences(schoolExperiences, candidate);
    SaveContactChannelCreations(contactChannelCreations, candidate.Id);

    IncrementCallbackBookingQuotaNumberOfBookings(phoneCall);
    AddQualifications(qualifications, candidate);
}
```

| Step | What happens | CRM calls? |
|------|-------------|------------|
| **Clear** | Extracts child entities from the candidate into local variables and clears them from the navigation properties. This prevents CRM from trying to deep-insert them (which would fail due to CRM relationship quirks). | None |
| **PreventDataOverwrite** | If the candidate has an existing CRM ID, fetches the current record via `_crm.GetCandidate(id)` and preserves: `Email` (always from existing), `VisaStatus`, `Citizenship`, `Location`, `Situation` (only if non-null in existing). This ensures the callback endpoint doesn't accidentally blank out fields the user isn't submitting. | 1 CRM read |
| **UpdateEventSubscriptionType** | If changing subscription type and the candidate already has a `LocalEvent` subscription, prevents downgrading to `SingleEvent`. | 1 CRM read |
| **SaveCandidate** | Sets `IsNewRegistrant` flag, optionally executes CRM custom action `dfe_ReRegisterCandidate` (if `HasReRegistered`), then calls `_crm.Save(candidate)` — writes the `Contact` entity to Dynamics 365. | 1 CRM execute + 1 CRM save |
| **SaveQualifications** | For each qualification: checks `_crm.CandidateHasDegreeQualification(...)` to avoid duplicates, then calls `_crm.Save(qualification)` synchronously (needs the CRM-generated ID). | 1 CRM read + 1 CRM save per qualification |
| **SavePastTeachingPositions** | Enqueues `UpsertModelWithCandidateIdJob<CandidatePastTeachingPosition>` in Hangfire for each position. | None (deferred) |
| **SaveApplicationForms** | Enqueues `UpsertApplicationFormJob` in Hangfire for each form. | None (deferred) |
| **SaveTeachingEventRegistrations** | For each registration: checks `_crm.CandidateYetToRegisterForTeachingEvent(...)` to dedup, then enqueues `UpsertModelWithCandidateIdJob<TeachingEventRegistration>`. | 1 CRM read per registration |
| **SavePrivacyPolicy** | Checks `_crm.CandidateYetToAcceptPrivacyPolicy(...)` to dedup, then enqueues `UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>`. | 1 CRM read |
| **SavePhoneCall** | Enqueues `UpsertModelWithCandidateIdJob<PhoneCall>` in Hangfire. | None (deferred) |
| **SaveSchoolExperiences** | Enqueues `UpsertModelWithCandidateIdJob<CandidateSchoolExperience>` in Hangfire for each experience. | None (deferred) |
| **SaveContactChannelCreations** | Enqueues `UpsertContactCreationChannelsJob` in Hangfire. | None (deferred) |
| **IncrementCallbackBookingQuota** | If a phone call was scheduled, enqueues `ClaimCallbackBookingSlotJob` in Hangfire (decrements available callback slots). | None (deferred) |

---

### Complete data flow diagram

```
Client App
  │
  │  POST /api/get_into_teaching/callbacks
  │  { email, firstName, lastName, telephone, phoneCallScheduledAt, ... }
  │
  ▼
CallbacksController.Book()
  │
  ├── Validate ModelState (400 if invalid)
  ├── Inject DateTimeProvider (for testability)
  ├── request.Candidate → CreateCandidate()
  │     ├── new Candidate { Id, Email, FirstName, LastName, AddressTelephone }
  │     ├── ConfigureChannel(channel, source, service)
  │     ├── SchedulePhoneCall() → candidate.PhoneCall
  │     └── AcceptPrivacyPolicy() → candidate.PrivacyPolicy
  ├── .SerializeChangeTracked() → compact JSON
  ├── Enqueue<UpsertCandidateJob>(json)
  │
  └── 204 No Content (immediate response)
         │
         │  [time passes — Hangfire picks up the job]
         │
         ▼
      UpsertCandidateJob.Run(json)
         │
         ├── DeserializeChangeTracked<Candidate>()
         ├── Deduplicate? → skip if identical job queued
         ├── CRM paused? → throw (retry)
         ├── Last attempt? → send failure email via GOV.UK Notify
         │
         └── CandidateUpserter.Upsert(candidate)
               │
               ├── Clear child entities into local variables
               ├── _crm.GetCandidate(id) ───────────┐
               │    (preserve email, visa, etc.)      │
               ├── _crm.Save(candidate) ─────────────┤
               │    (writes Contact to Dynamics 365)  │
               ├── _crm.Save(qualification) ──────────┤ DYNAMICS 365
               │    (writes qualification)             │ (Dataverse)
               ├── Enqueue background jobs ───────────┤
               │    (past teaching positions,          │
               │     event registrations,              │
               │     phone call, privacy policy,       │
               │     school experiences,               │
               │     contact channel creations,        │
               │     callback quota booking)           │
               └── Prometheus metrics ────────────────┘
```

---

### Database usage

| Database | Operation | When |
|----------|-----------|------|
| **Postgres** (local cache) | None | The endpoint and job never write to the local Postgres database. The Postgres cache is populated only by the periodic `CrmSyncJob` which pulls data FROM CRM. |
| **Dynamics 365** (Dataverse) | `_crm.Save()` | The Contact entity (and optionally qualifications) are written synchronously. All other child entities are written asynchronously via additional Hangfire jobs. |
| **Postgres** (Hangfire queue) | Job enqueue | Hangfire stores its job queue in Postgres. The initial `Enqueue` writes a row to Hangfire's tables. This is infrastructure, not application data. |

---

### Auth

| Attribute | Value |
|-----------|-------|
| Controller-level | `[Authorize(Roles = "Admin,GetIntoTeaching")]` |
| Endpoint-level | None (inherits controller) |

The caller must present a valid JWT belonging to either the `Admin` or `GetIntoTeaching` role.

---

### Request body (`GetIntoTeachingCallback`)

| Field | Type | Write-only | Description |
|-------|------|-----------|-------------|
| `CandidateId` | `Guid?` | | Existing candidate ID (null for new candidates) |
| `AcceptedPolicyId` | `Guid?` | Yes | Privacy policy version the user consented to |
| `Email` | `string` | | |
| `FirstName` | `string` | | |
| `LastName` | `string` | | |
| `AddressTelephone` | `string` | | Callback phone number |
| `PhoneCallScheduledAt` | `DateTime?` | Yes | When to call the candidate |
| `TalkingPoints` | `string` | | Free-text notes about what to discuss |
| `CreationChannelSourceId` | `int?` | Yes | Override channel source (defaults to `GITWebsite`) |
| `CreationChannelServiceId` | `int?` | Yes | Override channel service (defaults to `MailingList`) |
| `CreationChannelActivityId` | `int?` | Yes | Override channel activity (defaults to null) |

---

## 2. `POST /api/get_into_teaching/callbacks/exchange_access_token/{accessToken}` — `ExchangeAccessToken()`

**Purpose:** Verify a candidate's identity via a TOTP PIN code and return a pre-populated callback form.

```csharp
request.Reference ??= User.Identity.Name;
var candidate = _crm.MatchCandidate(request);

if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
    return Unauthorized();

return Ok(new GetIntoTeachingCallback(candidate));
```

| Step | What happens |
|------|-------------|
| 1 | Sets `request.Reference` from the authenticated JWT identity (if not already set by the caller) |
| 2 | `_crm.MatchCandidate(request)` — CRM builds a `QueryExpression` with an OR filter on `emailaddress1`/`emailaddress2`, requiring at least 3 matching attributes. Finds the candidate by email + partial match. |
| 3 | `_tokenService.IsValid(accessToken, request, candidateId)` — reconstructs the TOTP from the request + secret key, verifies against a 15-minute window of time steps |
| 4 | If no candidate found or PIN invalid → `401 Unauthorized` (same response for both to avoid leaking whether the email exists) |
| 5 | On success: `new GetIntoTeachingCallback(candidate)` — copies `Id`, `Email`, `FirstName`, `LastName`, applies `StripExitCode()` to `AddressTelephone` |
| 6 | Returns `200 OK` with the pre-populated callback form |

---

## 3. `POST /api/get_into_teaching/callbacks/matchback` — `Matchback()`

**Purpose:** Find an existing candidate by partial attribute matching and return a pre-populated callback form. No PIN required.

```csharp
request.Reference ??= User.Identity.Name;

if (!ModelState.IsValid)
    return BadRequest(ModelState);

var candidate = _crm.MatchCandidate(request);

if (candidate == null)
    return NotFound();

return Ok(new GetIntoTeachingCallback(candidate));
```

| Step | What happens |
|------|-------------|
| 1 | Sets `request.Reference` from the authenticated JWT identity (if not already set) |
| 2 | Validates `ModelState` |
| 3 | `_crm.MatchCandidate(request)` — same CRM matching query as the access token endpoint |
| 4 | If no candidate found → `404 Not Found` |
| 5 | On match: constructs `GetIntoTeachingCallback(candidate)` with `StripExitCode()` on telephone |
| 6 | Returns `200 OK` with the pre-populated callback form |

---

## Key differences between the 3 endpoints

| | `Book()` | `ExchangeAccessToken()` | `Matchback()` |
|---|---|---|---|
| Writes to CRM? | Yes (async via Hangfire job) | No | No |
| Requires PIN? | No | Yes (TOTP) | No |
| Returns candidate data? | No (204) | Yes (pre-filled form) | Yes (pre-filled form) |
| CRM calls on request path? | No | Yes (1 read: MatchCandidate) | Yes (1 read: MatchCandidate) |
| Creates child entities? | PhoneCall + PrivacyPolicy | No | No |
| Retries on failure? | Yes (up to 24x Hangfire) | N/A (no write) | N/A (no write) |

---

## Dependencies

| Service | Used by | Purpose |
|---------|---------|---------|
| `ICrmService` | `ExchangeAccessToken`, `Matchback`, `UpsertCandidateJob` | CRM reads (`MatchCandidate`, `GetCandidate`, guard checks) and writes (`Save`) |
| `IDateTimeProvider` | `Book` | Injected into DTO so `CreateCandidate()` can timestamp privacy policy acceptance (also enables time-freezing in contract tests) |
| `ICandidateAccessTokenService` | `ExchangeAccessToken` | TOTP PIN generation and validation |
| `IBackgroundJobClient` | `Book`, `UpsertCandidateJob` | Enqueues Hangfire jobs |
| `ICandidateUpserter` | `UpsertCandidateJob` | Orchestrates the multi-step CRM upsert (prevent overwrite, save candidate, save qualifications, enqueue child entity jobs, increment quota) |
| `INotifyService` | `UpsertCandidateJob` | Sends failure email on last retry attempt via GOV.UK Notify |
| `IMetricService` | `UpsertCandidateJob` | Records Prometheus `HangfireJobQueueDuration` histogram |
| `IAppSettings` | `UpsertCandidateJob` | Checks `IsCrmIntegrationPaused` flag |
| `IRedisService` | `UpsertCandidateJob` (via `BaseJob`) | Job deduplication (checks if same signature already queued) |
