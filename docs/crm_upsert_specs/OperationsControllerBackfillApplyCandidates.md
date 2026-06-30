## POST `/api/operations/backfill_apply_candidates`

Please check existing code and swagger doc for reference. I might have made mistakes or missed something here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/OperationsController.cs:128`

Triggers a bulk backfill to sync Apply candidates into CRM. Queries the Apply API for all candidates updated since a given timestamp, fetches them page by page, and schedules individual sync jobs (delayed by 2 hours per candidate). The process is gated by a Redis-backed lock to prevent concurrent backfills, and is admin-only.

## What it does (step by step)

1. **Authorization** — requires `Admin` role
2. **Guard** — checks `IAppSettings.IsApplyBackfillInProgress` (Redis-backed flag); returns `400 Bad Request` with `"Backfill already in progress"` if a backfill is already running
3. **Enqueues backfill job** — enqueues `ApplyBackfillJob.RunAsync(updatedSince, startPage: 1, candidateIds: null)` via Hangfire
4. **Returns immediately** — `204 No Content`; all work happens asynchronously in the background

### ApplyBackfillJob (`Jobs/ApplyBackfillJob.cs:47`)

1. **Sets lock** — `_appSettings.IsApplyBackfillInProgress = true` (persisted in Redis)
2. **Fetches candidates from Apply API** via `QueueCandidateSyncJobsUpdatedSince(updatedSince, startPage)`:
   - Builds a Flurl request to `{ApplyCandidateApiUrl}/candidates?updated_since=<value>` with Bearer token auth
   - Creates a `PaginatorClient<Response<IEnumerable<Candidate>>>` starting at `startPage`
   - Iterates pages (up to `PagesPerJob = 10`):
     - Calls `GET {ApplyApiUrl}/candidates?updated_since=<value>&page=<N>`
     - Apply API returns paginated candidates with `Total-Pages` and `Current-Page` response headers
     - For each candidate in the response: `_jobClient.Schedule<ApplyCandidateSyncJob>(x => x.Run(c), TimeSpan.FromHours(2))` — scheduled **2 hours in the future**
   - **Re-enqueues self** if more pages remain: enqueues another `ApplyBackfillJob` for the next page batch
3. **On completion** — sets `_appSettings.IsApplyBackfillInProgress = false`

### ApplyCandidateSyncJob (`Jobs/ApplyCandidateSyncJob.cs:34`)

Runs once per candidate (triggered by the scheduled time):

1. **CRM pause check** — if CRM integration is paused (`IAppSettings.IsCrmIntegrationPaused`), throws `InvalidOperationException`; Hangfire will retry
2. **Converts Apply → CRM model** — calls `applyCandidate.ToCrmModel()`:
   - Maps: `Email`, `ApplyId`, `ApplyCreatedAt`, `ApplyUpdatedAt`
   - Maps `ApplicationForms` via nested `ToCrmModel()` (forms → choices → references)
   - Determines `ApplyStatusId`:
     - If no application forms: sets to `NeverSignedIn` (222750000)
     - Otherwise: sets to `latestForm.StatusId` (from `application_status` → PascalCase → enum parse)
   - Determines `ApplyPhaseId`: sets to `latestForm.PhaseId` (from `application_phase` → PascalCase → enum parse)
3. **Wraps in channel** — creates `ContactChannelCandidateWrapper` with:
   - `DefaultContactCreationChannel` = `ApplyForTeacherTraining` (222750025)
   - `DefaultCreationChannelSourceId` = `Apply`
   - `DefaultCreationChannelServiceId` = `CreatedOnApply`
   - `DefaultCreationChannelActivityId` = null
   - Sets `CreationChannelSourceId = Apply`
4. **Matches against CRM** — calls `ICrmService.MatchCandidate(email, applyId)` to find existing CRM record
5. **If match found**:
   - Copies the CRM candidate `Id` onto the Apply candidate
   - If the Apply email differs from the match email **and** the match has no `SecondaryEmail` already: writes the Apply email to `SecondaryEmail` on the candidate
6. **Configures channel** — calls `candidate.ConfigureChannel(candidateId, wrappedCandidate)` which creates a `ContactChannelCreation` entity with `creationChannel: true` (if new candidate and no existing channel creations)
7. **Serializes** — `candidate.SerializeChangeTracked()` (serializes with changed property tracking)
8. **Enqueues upsert** — `_jobClient.Enqueue<UpsertCandidateJob>(x => x.Run(json, null))`

### UpsertCandidateJob (`Jobs/UpsertCandidateJob.cs:43`)

1. **Deserializes** — `json.DeserializeChangeTracked<Candidate>()`
2. **Deduplication** — if a job with the same signature (`candidate.Id + Email + changed properties`) is already queued, silently drops the duplicate
3. **CRM pause check** — throws if CRM integration is paused (Hangfire retry will fire)
4. **Last attempt handling** — on the final Hangfire retry attempt: sends a failure notification email via GOV.UK Notify (`CandidateRegistrationFailedEmailTemplateId`) and succeeds (fire-and-forget)
5. **Upsert** — calls `ICandidateUpserter.Upsert(candidate)` to persist candidate + all related entities (application forms, choices, references, contact channel creations) to Dynamics 365 CRM
6. **Metrics** — records Hangfire job queue duration for `UpsertCandidateJob`

## Request

`POST /api/operations/backfill_apply_candidates?updatedSince=2024-01-01T00:00:00Z`

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `updatedSince` | `DateTime` | **Yes** | Query param – ISO 8601 timestamp; only candidates updated after this time will be fetched |

## Responses

### `204 No Content` — backfill job queued

The backfill has been enqueued (or is already in progress — check is done server-side).

### `400 Bad Request` — backfill already in progress. This is a new proposed error format

```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "Backfill already in progress"
        }
    ]
}
```

Returned when `IAppSettings.IsApplyBackfillInProgress` is `true` (Redis-backed flag).

## What happens next (async job pipeline)

The backfill triggers a chain of Hangfire jobs:

### Level 1: ApplyBackfillJob
| Aspect | Detail |
|--------|--------|
| Concurrent execution | `[DisableConcurrentExecution(3600s)]` – only one instance can run at a time |
| Retry | `[AutomaticRetry(Attempts = 0)]` – no retries; if it fails, the lock remains set |
| Pages per job | `PagesPerJob = 10` |
| Candidate scheduling delay | 2 hours per candidate to prevent overwhelming the CRM |
| Lock | Sets `IsApplyBackfillInProgress = true` in Redis at start, `false` at end |

### Level 2: ApplyCandidateSyncJob (per candidate)
| Aspect | Detail |
|--------|--------|
| Model conversion | Apply `Candidate.{Id, Attributes.Email, CreatedAt, UpdatedAt, ApplicationForms}` → CRM `Candidate.{Email, ApplyId, ApplyCreatedAt, ApplyUpdatedAt, ApplyStatusId, ApplyPhaseId, ApplicationForms}` |
| CRM matching | `ICrmService.MatchCandidate(email, applyId)` |
| Channel | `ContactChannelCandidateWrapper` → `ChannelId = ApplyForTeacherTraining`, `CreationChannelSourceId = Apply`, `CreationChannelServiceId = CreatedOnApply` |
| Serialization | `SerializeChangeTracked()` — only changed properties are serialized for CRM upsert |

### Level 3: UpsertCandidateJob (final CRM write)
| Aspect | Detail |
|--------|--------|
| Deduplication | By signature: `{candidate.Id}-{Email}-{changed properties}` |
| CRM pause check | Throws if CRM integration is paused |
| Failure notification | On final retry, sends email via GOV.UK Notify (`CandidateRegistrationFailedEmailTemplateId`) |
| CRM upsert | Calls `ICandidateUpserter.Upsert(candidate)` — persists candidate and all nested entities to Dynamics 365 |

## Flow

```mermaid
flowchart TD
    A["Client (Admin)"]
    B["POST /api/operations/backfill_apply_candidates\n?updatedSince=<time>"]
    C["OperationsController\nBackfillApplyCandidates()"]
    G["Check\nIsApplyBackfillInProgress\n(Redis)"]
    J1["Enqueue\nApplyBackfillJob.RunAsync\n(updatedSince, 1, null)"]
    R["204 No Content"]

    A --> B --> C --> G
    G -->|in progress| 400
    G -->|not in progress| J1 --> R

    subgraph ApplyBackfillJob
        D["Set IsApplyBackfillInProgress = true\n(Redis)"]
        subgraph QueueCandidateSyncJobsUpdatedSince
            H["Flurl GET\n{ApplyApi}/candidates\n?updated_since=<time>&page={N}"]
            I["PaginatorClient\n(Total-Pages / Current-Page headers)"]
            K["For each candidate:\nSchedule ApplyCandidateSyncJob\n(+2 hours delay)"]
            M{"More pages?"}
            MN["Re-enqueue\nApplyBackfillJob\nfor next page batch"]

            H --> I --> K --> M
            M -->|yes| MN
            M -->|no| DONE1["Done"]
        end
        L["Set IsApplyBackfillInProgress = false\n(Redis)"]

        D --> QueueCandidateSyncJobsUpdatedSince
        QueueCandidateSyncJobsUpdatedSince --> L
    end

    subgraph ApplyCandidateSyncJob
        V{"CRM integration\npaused?"}
        ER["Throw\n(Job will retry)"]
        W["applyCandidate.ToCrmModel()\n━━━━━━━━━━━━━━━━━━━\n• Map scalars: Email, ApplyId\n• Convert ApplicationForms\n• Determine ApplyStatusId / ApplyPhaseId"]
        X["ContactChannelCandidateWrapper\n(Channel = Apply for Teacher Training)"]
        Y["ICrmService.MatchCandidate\n(email, applyId)"]
        Z{"Match found?"}
        ZY["Copy CRM Id\n& handle SecondaryEmail"]
        ZN["Candidate is new\n(no Id set)"]
        CC["ConfigureChannel()\n(adds ContactChannelCreation)"]
        UC["SerializeChangeTracked\n+ Enqueue UpsertCandidateJob"]

        V -->|paused| ER
        V -->|not paused| W --> X --> Y --> Z
        Z -->|yes| ZY --> CC
        Z -->|no| ZN --> CC
        CC --> UC
    end

    subgraph UpsertCandidateJob
        DC["DeserializeChangeTracked"]
        DU["Deduplicate\n(by candidate signature)"]
        VP{"CRM integration\npaused?"}
        ER2["Throw\n(Job will retry)"]
        LA{"Is last\nattempt?"}
        NF["Send failure email\n(GOV.UK Notify)"]
        U["ICandidateUpserter.Upsert(candidate)\n→ Dynamics 365 CRM"]

        DC --> DU
        DU -->|duplicate| SKIP["Silently drop"]
        DU -->|new| VP
        VP -->|paused| ER2
        VP -->|not paused| LA
        LA -->|yes| NF
        LA -->|no| U
    end

    J1 --> D
    K -.-> V
    UC -.-> DC
```

## Key business rules

| Rule | Detail |
|------|--------|
| **Concurrent backfill lock** | `IsApplyBackfillInProgress` is stored in Redis and prevents multiple backfills from running simultaneously |
| **Lock cleanup** | The lock is always set to `false` at the end of `ApplyBackfillJob.RunAsync`, even if pages were partially processed; a new backfill can resume from where it left off via the re-enqueue mechanism |
| **Candidate scheduling delay** | 2-hour delay prevents overwhelming the CRM with a sudden burst of upserts |
| **Email deduplication on match** | When a matching CRM record is found, the Apply candidate's email is written to `SecondaryEmail` only if it differs from the existing email **and** the match has no `SecondaryEmail` already set |
| **ApplyStatusId inference** | Candidates with no application forms get `NeverSignedIn`; otherwise the `StatusId` and `PhaseId` are derived from the latest application form's `application_status` and `application_phase` string values (converted via PascalCase → enum parse) |
| **No retry for ApplyBackfillJob** | `[AutomaticRetry(Attempts = 0)]` — the backfill job itself does not retry. If a page fetch fails the entire job fails, but the lock remains set. Manual intervention may be needed to clear the lock |
| **Deduplication in UpsertCandidateJob** | If multiple sync jobs for the same candidate (same ID + email + changed properties) are queued, duplicates are silently dropped |
| **Last-attempt email notification** | On the final Hangfire retry of `UpsertCandidateJob`, a failure notification email is sent via GOV.UK Notify and the job succeeds (fire-and-forget) |

## Proposed changes
- Can we return 403 status code when the backfill is already taking please? Rather than 400.
- Please use the v1.4 version of the Apply api. I think this is using v1.2 right now. The v1.4 spec is here https://www.apply-for-teacher-training.service.gov.uk/candidate-api
