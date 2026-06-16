# Endpoints Analysis

This document catalogues all API endpoints and identifies which ones do more than just proxy requests to the CRM (Microsoft Dynamics 365 / Dataverse).

---

## Pure Passthrough

These endpoints read data from a local Postgres cache (populated by periodic CRM sync via Hangfire jobs) or perform trivial operations. No CRM calls happen on the request path and no business logic is applied beyond cache reads.

### Lookup Items

#### `GET /api/lookup_items/countries`

**Controller:** `LookupItemsController.GetCountries()`  
**Service:** `IStore.GetCountries()` → `_dbContext.Countries.AsNoTracking().OrderBy(t => t.Id)`  
**What it does:** Reads all countries from the local Postgres cache, ordered by ID. The cache is populated by `CrmSyncJob` which calls `ICrmService.GetCountries()`. Returns `IEnumerable<Country>`.  
**Response:** `200 OK` with list of countries  
**Auth:** `[Authorize]` (any authenticated client)  
**Caching:** `[PrivateShortTermResponseCache]`

---

#### `GET /api/lookup_items/degree_countries`

**Controller:** `LookupItemsController.GetDegreeCountries()`  
**Service:** `IStore.GetDegreeFilteredCountries()` → `_dbContext.Countries.Where(country => Country.DegreeCountriesList.Contains(country.Id.Value))`  
**What it does:** Reads countries that are filtered to a hardcoded list (`Country.DegreeCountriesList`) of country IDs representing countries that can award degrees. Ordered by ID.  
**Response:** `200 OK` with filtered list of countries  
**Auth:** `[Authorize]`  
**Caching:** `[PrivateShortTermResponseCache]`

---

#### `GET /api/lookup_items/teaching_subjects`

**Controller:** `LookupItemsController.GetTeachingSubjects()`  
**Service:** `IStore.GetTeachingSubjects()` → `_dbContext.TeachingSubjects.AsNoTracking().OrderBy(t => t.Id)`  
**What it does:** Reads all teaching subjects from the local cache, ordered by ID.  
**Response:** `200 OK` with list of subjects  
**Auth:** `[Authorize]`  
**Caching:** `[PrivateShortTermResponseCache]`

---

### Pick List Items

All pick list item endpoints follow the same pattern:
- **Controller:** `PickListItemsController` with ~27 actions
- **Service:** `IStore.GetPickListItems(entityName, attributeName)` → `_dbContext.PickListItems.Where(t => t.EntityName == entityName && t.AttributeName == attributeName)`
- **What it does:** Reads pick list (option set) values for a specific CRM entity and attribute from the local cache
- **Response:** `200 OK` with list of `PickListItem` (each has `Id`, `Value`)
- **Auth:** `[Authorize]`
- **Caching:** `[PrivateShortTermResponseCache]`

| Route | CRM Entity | CRM Attribute |
|-------|-----------|--------------|
| `GET /api/pick_list_items/candidate/initial_teacher_training_years` | `contact` | `dfe_ittyear` |
| `GET /api/pick_list_items/candidate/preferred_education_phases` | `contact` | `dfe_preferrededucationphase01` |
| `GET /api/pick_list_items/candidate/channels` | `contact` | `dfe_channelcreation` |
| `GET /api/pick_list_items/candidate/mailing_list_subscription_channels` | `contact` | `dfe_gitismlservicesubscriptionchannel` |
| `GET /api/pick_list_items/candidate/event_subscription_channels` | `contact` | `dfe_gitiseventsservicesubscriptionchannel` |
| `GET /api/pick_list_items/candidate/teacher_training_adviser_subscription_channels` | `contact` | `dfe_gitisttaservicesubscriptionchannel` |
| `GET /api/pick_list_items/candidate/gcse_status` | `contact` | `dfe_websitehasgcseenglish` |
| `GET /api/pick_list_items/candidate/retake_gcse_status` | `contact` | `dfe_websiteplanningretakeenglishgcse` |
| `GET /api/pick_list_items/candidate/consideration_journey_stages` | `contact` | `dfe_websitewhereinconsiderationjourney` |
| `GET /api/pick_list_items/candidate/adviser_eligibilities` | `contact` | `dfe_iscandidateeligibleforadviser` |
| `GET /api/pick_list_items/candidate/adviser_requirements` | `contact` | `dfe_isadvisorrequiredos` |
| `GET /api/pick_list_items/candidate/types` | `contact` | `dfe_typeofcandidate` |
| `GET /api/pick_list_items/candidate/assignment_status` | `contact` | `dfe_candidatestatus` |
| `GET /api/pick_list_items/candidate/situations` | `contact` | `dfe_situation` |
| `GET /api/pick_list_items/candidate/citizenships` | `contact` | `dfe_citizenship` |
| `GET /api/pick_list_items/candidate/visa_statuses` | `contact` | `dfe_visastatus` |
| `GET /api/pick_list_items/candidate/locations` | `contact` | `dfe_location` |
| `GET /api/pick_list_items/candidate/has_qualified_teacher_statuses` | `contact` | `dfe_qtsstatus` |
| `GET /api/pick_list_items/qualification/degree_status` | `dfe_candidatequalification` | `dfe_degreestatus` |
| `GET /api/pick_list_items/qualification/types` | `dfe_candidatequalification` | `dfe_type` |
| `GET /api/pick_list_items/qualification/uk_degree_grades` | `dfe_candidatequalification` | `dfe_ukdegreegrade` |
| `GET /api/pick_list_items/past_teaching_position/education_phases` | `dfe_candidatepastteachingposition` | `dfe_educationphase` |
| `GET /api/pick_list_items/teaching_event/types` | `msevtmgt_event` | `dfe_event_type` |
| `GET /api/pick_list_items/teaching_event/regions` | `msevtmgt_event` | `dfe_eventregion` |
| `GET /api/pick_list_items/teaching_event/status` | `msevtmgt_event` | `dfe_eventstatus` |
| `GET /api/pick_list_items/teaching_event/accessibility_items` | `msevtmgt_event` | `dfe_accessibility` |
| `GET /api/pick_list_items/teaching_event_registration/channels` | `msevtmgt_eventregistration` | `dfe_channelcreation` |
| `GET /api/pick_list_items/phone_call/channels` | `phonecall` | `dfe_channelcreation` |
| `GET /api/pick_list_items/service_subscription/types` | `dfe_servicesubscription` | `dfe_servicesubscriptiontype` |
| `GET /api/pick_list_items/contact_creation_channel/sources` | `dfe_contactchannelcreation` | `dfe_creationchannelsource` |
| `GET /api/pick_list_items/contact_creation_channel/services` | `dfe_contactchannelcreation` | `dfe_creationchannelservice` |
| `GET /api/pick_list_items/contact_creation_channel/activities` | `dfe_contactchannelcreation` | `dfe_creationchannelactivities` |

---

### Privacy Policies

#### `GET /api/privacy_policies/latest`

**Controller:** `PrivacyPoliciesController.GetLatest()`  
**Service:** `IStore.GetLatestPrivacyPolicyAsync()` → `_dbContext.PrivacyPolicies.OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync()`  
**What it does:** Reads the most recently created privacy policy from the Postgres cache.  
**Response:** `200 OK` with `PrivacyPolicy`  
**Auth:** `[Authorize]`  
**Caching:** `[PrivateShortTermResponseCache]`

---

#### `GET /api/privacy_policies/{id}`

**Controller:** `PrivacyPoliciesController.Get(Guid id)`  
**Service:** `IStore.GetPrivacyPolicyAsync(id)` → `_dbContext.PrivacyPolicies.FirstOrDefaultAsync(p => p.Id == id)`  
**What it does:** Reads a specific privacy policy by GUID ID from the Postgres cache. Returns 404 if not found.  
**Response:** `200 OK` with `PrivacyPolicy`, or `404 Not Found`  
**Auth:** `[Authorize]`  
**Caching:** `[PrivateShortTermResponseCache]`

---

### Teaching Events

#### `GET /api/teaching_event_buildings`

**Controller:** `TeachingEventBuildingsController.GetTeachingEventBuildings()`  
**Service:** `IStore.GetTeachingEventBuildings()` → `_dbContext.TeachingEventBuildings.AsNoTracking()`  
**What it does:** Reads all teaching event buildings from the Postgres cache.  
**Response:** `200 OK` with list of `TeachingEventBuilding`  
**Auth:** `Admin, GetIntoTeaching`  
**Caching:** `[PrivateShortTermResponseCache]`

---

#### `GET /api/teaching_events/{readableId}`

**Controller:** `TeachingEventsController.Get(string readableId)`  
**Service:** `IStore.GetTeachingEventAsync(readableId)` → `_dbContext.TeachingEvents.Include(e => e.Building).FirstOrDefaultAsync(e => e.ReadableId == readableId)`  
**What it does:** Reads a single teaching event by its human-readable ID (e.g. `"123"`), including its building. Returns 404 if not found.  
**Response:** `200 OK` with `TeachingEvent`, or `404 Not Found`  
**Auth:** `Admin, GetIntoTeaching`  
**Caching:** `[PrivateShortTermResponseCache]`

---

### Operations

#### `GET /api/operations/generate_mapping_info`

**Controller:** `OperationsController.GenerateMappingInfo()`  
**What it does:** Uses reflection to find all types that inherit from `BaseModel` in the assembly and generates `MappingInfo` objects describing how each C# model maps to Dynamics 365 entities/fields. No CRM call.  
**Response:** `200 OK` with `IEnumerable<MappingInfo>`  
**Auth:** None (unauthenticated)

---

#### `GET /healthcheck`

**Defined in:** `Startup.cs` inline route  
**What it does:** Writes the literal string `"OK"` to the HTTP response body. The simplest possible health check.  
**Response:** `200 OK` with body `"OK"`  
**Auth:** None (unauthenticated)

---

---

## Doing More Than Passthrough

These endpoints contain extra business logic, state mutations, external service calls, background job orchestration, algorithmic processing, or non-trivial DTO transformations beyond a simple CRM call.

---

### 1. `GET /api/callback_booking_quotas`

**Controller:** `CallbackBookingQuotasController.GetAll()`  
**Service:** `ICallbackBookingService.GetCallbackBookingQuotas()` → `CallbackBookingService`  
**Auth:** `Admin, GetAnAdviser, GetIntoTeaching`

**Flow:**
1. Tries `ICrmService.GetCallbackBookingQuotas()` to retrieve quota slots from CRM.
2. **If CRM is unreachable** (exception thrown), logs the error and generates synthetic fallback quotas:
   - Looks ahead from tomorrow, skipping weekends
   - Generates 30-minute time slots from 09:00 to 16:30
   - Each slot has `Quota = 1` and `NumberOfBookings = 0`
   - Converts UTC times to local timezone for display
   - Continues until 5 weekdays' worth of quotas are generated
3. Returns the list (CRM or fallback).

**Why it does more:** Contains fallback business logic that invents CRM data when the CRM is offline — including weekday calculation, timezone conversion, and 30-minute interval scheduling. The fallback quotas are synthetic and have no relationship to actual CRM availability.

**Files:** `Controllers/CallbackBookingQuotasController.cs:30`, `Services/CallbackBookingService.cs:26`

---

### 2. `GET /api/operations/health_check`

**Controller:** `OperationsController.HealthCheck()`  
**Auth:** None (unauthenticated)

**What it does:** Aggregates the health status of **five separate services** into a single composite response:
1. **Hangfire** — `IHangfireService.CheckStatus()` (checks background job server)
2. **Database** — `IStore.CheckStatusAsync()` (opens/closes a Postgres connection)
3. **CRM** — `ICrmService.CheckStatus()` (pings Dynamics 365)
4. **Notify** — `INotifyService.CheckStatusAsync()` (pings GOV.UK Notify API)
5. **Redis** — `IRedisService.CheckStatusAsync()` (pings Redis)

Also includes `GitCommitSha` and `Environment` from `IEnv`.

**Why it does more:** Orchestrates health checks across 5 dependencies and aggregates them into one response. This is a composite pattern, not a simple CRM passthrough.

**Files:** `Controllers/OperationsController.cs:79`

---

### 3. `PUT /api/operations/pause_crm_integration`

**Controller:** `OperationsController.PauseCrmIntegration()`  
**Auth:** `Admin, Crm`

**What it does:**
1. Sets `_appSettings.CrmIntegrationPausedUntil = DateTime.UtcNow.AddHours(6)`
2. This flag is checked by other endpoints (e.g. `CreateAccessToken`, `Matchback`) to short-circuit CRM calls and return 404
3. The pause auto-expires after 6 hours — this is a self-healing mechanism intended for CRM maintenance windows

**Why it does more:** Mutates application state with a 6-hour auto-resume timer. This is operational logic that configures how the rest of the application behaves.

**Files:** `Controllers/OperationsController.cs:106`

---

### 4. `PUT /api/operations/resume_crm_integration`

**Controller:** `OperationsController.ResumeCrmIntegration()`  
**Auth:** `Admin, Crm`

**What it does:**
1. Sets `_appSettings.CrmIntegrationPausedUntil = null`
2. Instantly re-enables all CRM calls

**Why it does more:** Mutates application state.

**Files:** `Controllers/OperationsController.cs:121`

---

### 5. `POST /api/operations/backfill_apply_candidates`

**Controller:** `OperationsController.BackfillApplyCandidates(DateTime updatedSince)`  
**Auth:** `Admin`

**What it does:**
1. Checks `_appSettings.IsApplyBackfillInProgress` — returns `400 Bad Request` with `"Backfill already in progress"` if true
2. Enqueues `ApplyBackfillJob.RunAsync(updatedSince, 1, null)` in Hangfire

**Why it does more:** Guard logic + background job orchestration.

**Files:** `Controllers/OperationsController.cs:139`

---

### 6. `POST /api/operations/backfill_apply_candidates_from_ids`

**Controller:** `OperationsController.BackfillApplyCandidatesFromIds(CandidateIdsRequest request)`  
**Auth:** `Admin`

**What it does:**
1. Same guard as above (`IsApplyBackfillInProgress`)
2. Enqueues `ApplyBackfillJob.RunAsync(DateTime.MinValue, 1, request.CandidateIds)` in Hangfire

**Why it does more:** Guard logic + background job orchestration with specific candidate IDs.

**Files:** `Controllers/OperationsController.cs:162`

---

### 7. `POST /api/candidates/access_tokens`

**Controller:** `CandidatesController.CreateAccessToken(ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching, GetAnAdviser, SchoolsExperience, Apply`

**Flow:**
1. Sets `request.Reference` from the authenticated user identity (if not already set)
2. Validates `ModelState`
3. **Checks CRM integration pause** — if `_appSettings.IsCrmIntegrationPaused`, returns `404 Not Found` and logs
4. Calls `_crm.MatchCandidate(request)` to find the candidate by email + at least 2 other matching attributes
5. Catches CRM exceptions, logs, and re-throws
6. If no candidate found, returns `404 Not Found`
7. **Generates a 6-digit TOTP PIN** via `CandidateAccessTokenService.GenerateToken()`:
   - Creates a compound secret key from `request.Slugify()` + `_env.TotpSecretKey`
   - Uses `OtpNet.Totp` with 30-second step, 6-digit length
   - The PIN is valid for 30 steps × 30 seconds = 15 minutes
   - Increments Prometheus counter `GeneratedTotps` with the client reference label
8. **Sends the PIN via GOV.UK Notify** email:
   - Template: `NewPinCodeEmailTemplateId` ("New Pin code")
   - Personalisation: `{ pin_code, first_name }`
   - Fire-and-forget: logs failure but doesn't block the response
9. Returns `204 No Content`

**Why it does more:** TOTP cryptographic PIN generation + external email sending via GOV.UK Notify + CRM pause handling + Prometheus metrics. This is the **only endpoint that sends an email on the request path**.

**Files:** `Controllers/CandidatesController.cs:52`, `Services/CandidateAccessTokenService.cs:28`, `Services/NotifyService.cs:31`

---

### 8. `POST /api/get_into_teaching/callbacks`

**Controller:** `CallbacksController.Book(GetIntoTeachingCallback request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Validates `ModelState`
2. Injects `IDateTimeProvider` into the request (for testability — allows freezing time in contract tests)
3. Serializes the request's inner `Candidate` via `request.Candidate.SerializeChangeTracked()` — this serializes only the properties that have changed (using INotifyPropertyChanged tracking) into JSON
4. **Enqueues `UpsertCandidateJob.Run(json, null)`** in Hangfire for asynchronous processing
5. Returns `204 No Content`

**The Hangfire job does:**
- Deserializes the candidate JSON
- Attempts the upsert on each retry (up to 24 times in production, 5 in dev)
- On the **last retry attempt only**: sends a GOV.UK Notify failure email using `CandidateRegistrationFailedEmailTemplateId` (skips the upsert, job exits cleanly to end the retry cycle)

**Why it does more:** Change-tracking serialization + Hangfire background job orchestration. The actual CRM write happens asynchronously.

**Files:** `Controllers/GetIntoTeaching/CallbacksController.cs:46`, `Jobs/UpsertCandidateJob.cs`

---

### 9. `POST /api/get_into_teaching/callbacks/exchange_access_token/{accessToken}`

**Controller:** `CallbacksController.ExchangeAccessToken(string accessToken, ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to find the candidate
3. **Validates the TOTP PIN** via `_tokenService.IsValid(accessToken, request, candidateId)`:
   - Reconstructs the same TOTP using the request + secret key
   - Verifies against a window of 30 previous time steps (15 minutes)
   - Increments Prometheus counter `VerifiedTotps` with valid/invalid label + reference
4. If candidate not found or PIN invalid, returns `401 Unauthorized`
5. On success, wraps the candidate in `GetIntoTeachingCallback`:
   - Copies `Id`, `Email`, `FirstName`, `LastName`
   - Calls `candidate.AddressTelephone.StripExitCode()` — a regex `^00` → "" replacement
6. Returns `200 OK` with the pre-populated callback

**Why it does more:** TOTP cryptographic validation + Prometheus metrics + DTO constructor with `StripExitCode()`.

**Files:** `Controllers/GetIntoTeaching/CallbacksController.cs:74`, `Models/GetIntoTeaching/GetIntoTeachingCallback.cs:67`

---

### 10. `POST /api/get_into_teaching/callbacks/matchback`

**Controller:** `CallbacksController.Matchback(ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Validates `ModelState`
3. Calls `_crm.MatchCandidate(request)` — CRM builds a `QueryExpression` with an OR filter matching `emailaddress1` OR `emailaddress2`, plus additional attributes (at least 3 total must match)
4. If no candidate found, returns `404 Not Found`
5. Wraps in `GetIntoTeachingCallback` (which calls `StripExitCode()` on telephone)
6. Returns `200 OK`

**Why it does more:** The matchback itself is a CRM query with complex matching logic, plus DTO transformation. However, this is the **least complex** of the "doing more" endpoints.

**Files:** `Controllers/GetIntoTeaching/CallbacksController.cs:100`

---

### 11. `POST /api/mailing_list/members`

**Controller:** `MailingListController.AddMember(MailingListAddMember request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Validates `ModelState`
2. **Infers degree status from graduation year:**
   - Calls `request.InferDegreeStatus(_degreeStatusDomainService, _currentYearProvider)`
   - If `GraduationYear` is set, creates a `DegreeStatusInferenceRequest` with a `GraduationYear` value object
   - Calls `IDegreeStatusDomainService.GetInferredDegreeStatusFromGraduationYear(...)` to map the year to a degree status ID
   - Sets `InferredGraduationDate` to August 31st of the graduation year
3. Injects `IDateTimeProvider` into the request
4. Serializes the candidate via `request.Candidate.SerializeChangeTracked()`
5. **Enqueues `UpsertCandidateJob`** in Hangfire
6. Returns `200 OK` with `DegreeStatusResponse { DegreeStatusId }` — the inferred degree status is returned to the caller

**The `MailingListAddMember` DTO constructor (when later reading candidates):**
- Queries `candidate.Qualifications` with LINQ: `.OrderByDescending(q => q.CreatedAt).FirstOrDefault()`
- Conditionally sets `QualificationId` and `DegreeStatusId`
- Copies basic fields (Id, Email, Name, Postcode, Situation, etc.)
- Checks `candidate.HasMailingListSubscription == true` (nullable boolean to bool)
- Checks `candidate.HasEventsSubscription == true`
- Calls `candidate.HasTeacherTrainingAdviser()` — checks if `HasTeacherTrainingAdviserSubscription == true` OR `OwningBusinessUnitId == AdviserBusinessUnitId`

**Why it does more:** Degree status inference heuristic (business logic mapping graduation years to degree statuses) + Hangfire job orchestration + DTO constructor with LINQ query and conditional business logic.

**Files:** `Controllers/GetIntoTeaching/MailingListController.cs:60`, `Models/GetIntoTeaching/MailingListAddMember.cs:108`, `Models/Crm/DegreeStatusInference/DegreeStatusInference.cs`

---

### 12. `POST /api/mailing_list/members/exchange_access_token/{accessToken}`

**Controller:** `MailingListController.ExchangeAccessTokenForMember(string accessToken, ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to find the candidate
3. Validates the TOTP PIN via `_accessTokenService.IsValid()`
4. Returns `401 Unauthorized` if no match or invalid PIN
5. Returns `200 OK` with pre-populated `MailingListAddMember`:
   - Constructor does: LINQ query over qualifications (ordered by CreatedAt desc, first only), conditional assignment, `HasTeacherTrainingAdviser()`, `== true` checks on subscription booleans

**Why it does more:** TOTP validation + DTO constructor with business logic.

**Files:** `Controllers/GetIntoTeaching/MailingListController.cs:98`, `Models/GetIntoTeaching/MailingListAddMember.cs:113`

---

### 13. `GET /api/mailing_list/members/exchange_magic_link_token/{magicLinkToken}`

**Controller:** `MailingListController.ExchangeMagicLinkTokenForMember(string magicLinkToken)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Calls `_magicLinkTokenService.Exchange(magicLinkToken)`:
   - Queries CRM via `_crm.MatchCandidates(token)` to find candidates where `dfe_magiclinktoken == token`
   - **Requires exactly 1 match** — returns failure for 0 matches or 2+ (duplicate token, extremely unlikely)
   - Updates the matched candidate's `MagicLinkTokenStatusId` to `Exchanged` (status tracking)
   - Returns `CandidateMagicLinkExchangeResult` with the candidate
2. If `result.Success` is false, returns `401 Unauthorized` with the result
3. **On success, serializes the candidate** and enqueues `UpsertCandidateJob` — this persists the token status change to CRM
4. Returns `200 OK` with pre-populated `MailingListAddMember`

**Why it does more:** Cryptographic magic link token exchange (generated by a background job using `RandomNumberGenerator`) with CRM lookup + dual-constraint uniqueness check + status tracking auto-upsert. This is the only endpoint that exchanges a magic link token (as opposed to a PIN code).

**Files:** `Controllers/GetIntoTeaching/MailingListController.cs:125`, `Services/CandidateMagicLinkTokenService.cs:28`

---

### 14. `GET /api/teaching_events/search`

**Controller:** `TeachingEventsController.Search(TeachingEventSearchRequest request, int quantity)`  
**Auth:** `Admin, GetIntoTeaching`  
**Caching:** `[PrivateShortTermResponseCache]`

**Flow:**
1. Validates `ModelState`
2. Logs the postcode if provided
3. Calls `_store.SearchTeachingEventsAsync(request)` which applies filters:
   - **Type IDs**: `teachingEvents.Where(te => request.TypeIds.Contains(te.TypeId))`
   - **Date range**: `StartAfter` / `StartBefore` filtering
   - **Status IDs**: `request.StatusIds.Contains(te.StatusId)`
   - **Accessibility options**: Uses `TeachingEventFilterBuilder.BuildAccessibilityFilter()` — dynamically builds an expression tree that checks if accessibility option IDs are contained within the comma-separated `AccessibilityOptionId` string field using `string.Concat(",", field, ",").Contains(",id,")`. This avoids client-side evaluation.
   - **Online/In-person**: `te.IsOnline == request.Online`
   - **Radius + Postcode**: If both provided, calls `FilterTeachingEventsByRadius()`:
     - Geocodes the postcode via `CoordinateForPostcode()`:
       1. Sanitizes the postcode
       2. Checks a static `FailedPostcodeLookupCache` (in-memory set) to avoid repeating failed lookups
       3. Tries a local Postgres lookup in the `Locations` table
       4. Falls back to `IGeocodeClientAdapter.GeocodePostcodeAsync()` — calls the **Google Geocoding API**
       5. On success, caches the result in Postgres
       6. On failure, adds to the `FailedPostcodeLookupCache`
     - Filters events to those with a building that has a coordinate
     - Uses `NetTopologySuite.Geometries.Point.Distance()` for spatial filtering: `building.Coordinate.Distance(origin) < request.RadiusInKm() * 1000`
     - Includes matching **online events** alongside in-person results via `OnlineEventsMatchingRequest()` (recursive call without radius)
4. Truncates results to `quantity` (default: 10)
5. Records **Prometheus histograms**:
   - `TeachingEventSearchResults` — labels: type IDs, radius — observes total count
   - `InPersonTeachingEventResults` — labels: type IDs, radius — observes in-person count
6. Returns `200 OK` with the events

**Why it does more:** Calls Google Geocoding API on the request path, uses spatial types for distance computation, builds dynamic expression trees for multi-select filtering, records Prometheus metrics with multiple label dimensions. This is the **only endpoint that calls an external API (Google Geocoding)** on the request path.

**Files:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:66`, `Services/Store.cs:112`, `Services/Store.cs:203` (`FilterTeachingEventsByRadius`), `Services/Store.cs:431` (`CoordinateForPostcode`), `Adapters/GeocodeClientAdapter.cs`

---

### 15. `POST /api/teaching_events/attendees`

**Controller:** `TeachingEventsController.AddAttendee(TeachingEventAddAttendee request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Validates `ModelState`
2. Injects `IDateTimeProvider` into the request
3. Serializes `request.Candidate` via `SerializeChangeTracked()`
4. Enqueues `UpsertCandidateJob` in Hangfire
5. Returns `204 No Content`

**Why it does more:** Change-tracking serialization + Hangfire background job.

**Files:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:129`

---

### 16. `POST /api/teaching_events/attendees/exchange_unverified_request`

**Controller:** `TeachingEventsController.ExchangeUnverifiedRequestForAttendee(ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to match the candidate
3. If no match, returns `404 Not Found`
4. Creates a `TeachingEventAddAttendee` from the candidate with `IsVerified = false`
5. Calls `attendee.ClearAttributesForUnverifiedAccess()` — strips sensitive attributes that shouldn't be returned for unverified access
6. The `TeachingEventAddAttendee` constructor does: LINQ query over qualifications, `HasTeacherTrainingAdviser()`, `StripExitCode()` on telephone, `== true` checks on subscription booleans
7. Returns `200 OK` with the pre-populated attendee

**Why it does more:** Introduces the concept of "unverified" access — a first-class access level with different data visibility than verified/token-based access. The `ClearAttributesForUnverifiedAccess()` method actively removes data from the response based on the access level.

**Files:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:157`, `Models/GetIntoTeaching/TeachingEventAddAttendee.cs:107`

---

### 17. `POST /api/teaching_events/attendees/exchange_access_token/{accessToken}`

**Controller:** `TeachingEventsController.ExchangeAccessTokenForAttendee(string accessToken, ExistingCandidateRequest request)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to find the candidate
3. Validates the TOTP PIN via `_tokenService.IsValid()`
4. Returns `401 Unauthorized` if no match or invalid PIN
5. Returns `200 OK` with pre-populated `TeachingEventAddAttendee`:
   - Constructor does: LINQ query over qualifications (ordered by CreatedAt desc, first only), conditional assignment, `HasTeacherTrainingAdviser()`, `StripExitCode()` on telephone, `== true` checks

**Why it does more:** TOTP validation + DTO constructor with business logic.

**Files:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:191`, `Models/GetIntoTeaching/TeachingEventAddAttendee.cs:107`

---

### 18. `POST /api/teaching_events`

**Controller:** `TeachingEventsController.Upsert(TeachingEvent teachingEvent)`  
**Auth:** `Admin, GetIntoTeaching`

**Flow:**
1. **Custom validation** via `ValidateForUpsertOperation(teachingEvent)`:
   - Creates `TeachingEventUpsertOperation` wrapping the event + a CRM-adapter-based validator
   - Validates the event entity against CRM (e.g., checks if referenced building exists)
   - Adds validation results to `ModelState`
2. If invalid, returns `400` with model state errors
3. **Persists building (if present):**
   - `_crm.Save(teachingEvent.Building)` — saves the building entity to Dynamics 365
   - `await _store.SaveAsync(teachingEvent.Building)` — saves to Postgres cache
   - Sets `teachingEvent.BuildingId = teachingEvent.Building.Id` — so the event references the building
4. **Persists event:**
   - Temporarily removes the building navigation property (`teachingEvent.Building = null`) to prevent a circular reference error in CRM
   - `_crm.Save(teachingEvent)` — saves to Dynamics 365
   - Restores the building (`teachingEvent.Building = tempBuilding`)
   - `await _store.SaveAsync(teachingEvent)` — saves to Postgres cache
5. Returns `201 Created` with the event and a `Location` header pointing to `GET /api/teaching_events/{readableId}`

**Why it does more:** Custom FluentValidation against CRM data + dual persistence (CRM + Postgres cache) + order-dependent saving (building before event) + temporary relationship manipulation to avoid CRM errors. Makes **two CRM save calls + two cache saves** in sequence.

**Files:** `Controllers/GetIntoTeaching/TeachingEventsController.cs:217`, `Models/Validators/TeachingEventUpsertOperationValidator.cs`

---

### 19. `POST /api/schools_experience/candidates`

**Controller:** `SchoolsExperience.CandidatesController.SignUp(SchoolsExperienceSignUp request)`  
**Auth:** `Admin, SchoolsExperience`

**Flow:**
1. Validates `ModelState`
2. Injects `IDateTimeProvider` into the request
3. Extracts `candidate = request.Candidate`
4. **If CRM integration is paused** (`appSettings.IsCrmIntegrationPaused`):
   - Calls `QueueUpsert(candidate)` which:
     - Calls `candidate.GenerateUpfrontId()` — generates a deterministic GUID upfront (normally CRM generates sequential GUIDs for better SQL performance, but the Schools Experience app needs the ID immediately)
     - Serializes via `SerializeChangeTracked()`
     - Enqueues `UpsertCandidateJob` in Hangfire
5. **If CRM is available:**
   - Calls `_upserter.Upsert(candidate)` which orchestrates a complex multi-step save:
     a. **Clears relationships** into local lists: teaching event registrations, phone call, privacy policy, qualifications, past teaching positions, application forms, school experiences, contact channel creations
     b. **Prevents data overwrite** (`PreventCandidateDataFromBeingOverwritten`):
        - If candidate has an existing CRM record, loads it via `_crm.GetCandidate(id)`
        - Preserves: `Email` (always overwritten from existing), `VisaStatus`, `Citizenship`, `Location`, `Situation` (only if existing values are non-null)
     c. **Updates event subscription type** (`UpdateEventSubscriptionType`):
        - If changing subscription type and candidate already has a local event subscription → never downgrades to "single event"
     d. **Saves candidate** (`SaveCandidate`):
        - Sets `IsNewRegistrant` flag
        - If `HasReRegistered` and ID exists, executes CRM custom action `dfe_ReRegisterCandidate` via `OrganizationRequest`
        - Calls `_crm.Save(candidate)` — saves the Contact entity
     e. **Saves qualifications** — each one via `_crm.Save(qualification)` with dedup check: `CandidateHasDegreeQualification(candidateId, DegreeType.Degree, subject)` — only saves if it doesn't already exist
     f. **Saves past teaching positions** — each enqueued as `UpsertModelWithCandidateIdJob<CandidatePastTeachingPosition>` in Hangfire
     g. **Saves application forms** — each enqueued as `UpsertApplicationFormJob` in Hangfire
     h. **Saves teaching event registrations** — with dedup check: `CandidateYetToRegisterForTeachingEvent(candidateId, eventId)`, enqueued as `UpsertModelWithCandidateIdJob<TeachingEventRegistration>`
     i. **Saves privacy policy** — with dedup check: `CandidateYetToAcceptPrivacyPolicy(candidateId, policyId)`, enqueued as background job
     j. **Saves phone call** — enqueued as `UpsertModelWithCandidateIdJob<PhoneCall>` in Hangfire
     k. **Saves school experiences** — each enqueued as background job
     l. **Saves contact channel creations** — enqueued as `UpsertContactCreationChannelsJob`
     m. **Increments callback booking quota** — enqueues `ClaimCallbackBookingSlotJob` with the phone call's scheduled time
   - **Catches exceptions**: If `Upsert()` throws, falls back to `QueueUpsert(candidate)` (graceful degradation)
6. Returns `201 Created` with:
   - `Location` header pointing to `GET /api/schools_experience/candidates/{id}`
   - Body: `SchoolsExperienceSignUp` DTO constructed from the candidate

**The `SchoolsExperienceSignUp` DTO constructor does:**
- Copies basic fields (Id, Name, Email, Address, etc.)
- **Telephone fallback chain**: `candidate.AddressTelephone.StripExitCode() ?? Array.Find([Telephone, MobileTelephone, SecondaryTelephone], n => !string.IsNullOrWhiteSpace(n)).StripExitCode()` — tries 4 phone numbers in precedence with `StripExitCode()` on each
- **LINQ query #1**: `candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault()` — sets `QualificationId`, `DegreeSubject`, `UkDegreeGradeId`, `DegreeStatusId`, `DegreeTypeId`
- **LINQ query #2**: `candidate.ContactChannelCreations.OrderByDescending(c => c.CreatedAt).FirstOrDefault()` — sets `CreationChannelSourceId`, `CreationChannelServiceId`, `CreationChannelActivityId`

**Why it does more:** The most complex endpoint in the application. Orchestrates 10+ individual save operations with dedup checks, data overwrite prevention rules, CRM custom action execution (re-registration), graceful degradation, and upfront ID generation for offline scenarios.

**Files:** `Controllers/SchoolsExperience/CandidatesController.cs:54`, `Services/CandidateUpserter.cs:30`, `Models/SchoolsExperience/SchoolsExperienceSignUp.cs:81`

---

### 20. `GET /api/schools_experience/candidates/{id}`

**Controller:** `SchoolsExperience.CandidatesController.Get(Guid id)`  
**Auth:** `Admin, SchoolsExperience`

**Flow:**
1. Calls `_crm.GetCandidate(id)` — retrieves the full candidate with all relationships (qualifications, past teaching positions, event registrations, etc.) from Dynamics 365
2. If null, returns `404 Not Found`
3. Wraps in `SchoolsExperienceSignUp` (same non-trivial constructor as above: telephone fallback chain + 2 LINQ queries)
4. Returns `200 OK`

**Why it does more:** The CRM call loads the full candidate with all related entities, and the DTO constructor performs significant data transformation (telephone fallback, LINQ sorting/filtering, conditional logic).

**Files:** `Controllers/SchoolsExperience/CandidatesController.cs:99`, `Models/SchoolsExperience/SchoolsExperienceSignUp.cs:86`

---

### 21. `GET /api/schools_experience/candidates`

**Controller:** `SchoolsExperience.CandidatesController.GetMultiple(IEnumerable<Guid> ids)`  
**Auth:** `Admin, SchoolsExperience`

**Flow:**
1. Takes a comma-separated list of candidate GUIDs from the query string (via `[CommaSeparated]` attribute)
2. Calls `_crm.GetCandidates(ids)` — retrieves multiple candidates from CRM
3. Wraps each in `SchoolsExperienceSignUp` (same non-trivial constructor)
4. Returns `200 OK`

**Why it does more:** Same as above, but for multiple candidates — each goes through the non-trivial DTO transformation.

**Files:** `Controllers/SchoolsExperience/CandidatesController.cs:119`, `Models/SchoolsExperience/SchoolsExperienceSignUp.cs:86`

---

### 22. `POST /api/schools_experience/candidates/exchange_access_token/{accessToken}`

**Controller:** `SchoolsExperience.CandidatesController.ExchangeAccessToken(string accessToken, ExistingCandidateRequest request)`  
**Auth:** `Admin, SchoolsExperience`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to find the candidate
3. Validates TOTP PIN via `_tokenService.IsValid()`
4. Returns `401 Unauthorized` if no match or invalid PIN
5. Returns `200 OK` with pre-populated `SchoolsExperienceSignUp` (telephone fallback + 2 LINQ queries)

**Why it does more:** TOTP validation + DTO constructor with business logic.

**Files:** `Controllers/SchoolsExperience/CandidatesController.cs:138`, `Models/SchoolsExperience/SchoolsExperienceSignUp.cs:86`

---

### 23. `POST /api/schools_experience/candidates/{id}/school_experience`

**Controller:** `SchoolsExperience.CandidatesController.AddSchoolExperience(Guid id, CandidateSchoolExperience candidateSchoolExperience)`  
**Auth:** `Admin, SchoolsExperience`

**Flow:**
1. Constructs a new `Candidate { Id = id }` and adds the `candidateSchoolExperience` to its `SchoolExperiences` collection
2. Validates `ModelState`
3. Serializes via `candidate.SerializeChangeTracked()` — this serializes the candidate with the new school experience as a change-tracked JSON payload
4. Enqueues `UpsertCandidateJob` in Hangfire
5. Returns `204 No Content`

**Why it does more:** Constructs an in-memory candidate object + change-tracking serialization + Hangfire background job.

**Files:** `Controllers/SchoolsExperience/CandidatesController.cs:163`

---

### 24. `POST /api/teacher_training_adviser/candidates`

**Controller:** `TeacherTrainingAdviser.CandidatesController.SignUp(TeacherTrainingAdviserSignUp request)`  
**Auth:** `Admin, GetAnAdviser, Apply, GetIntoTeaching`

**Flow:**
1. Validates `ModelState`
2. **Infers degree status from graduation year** (identical logic to mailing list):
   - Calls `request.InferDegreeStatus(_degreeStatusDomainService, _currentYearProvider)`
3. Injects `IDateTimeProvider` into the request
4. Serializes `request.Candidate` via `SerializeChangeTracked()`
5. **Enqueues `UpsertCandidateJob`** in Hangfire
6. Logs the signup with the authenticated client name
7. Returns `200 OK` with `DegreeStatusResponse { DegreeStatusId }`

**The `TeacherTrainingAdviserSignUp` DTO constructor (when later reading candidates) does:**
- Copies basic fields (Id, Subject, Country, ITT Year, etc.)
- **GCSE checks:**
  - `candidate.HasGcseMathsAndEnglish()` — checks if `GcseStatus` array values all equal `HasOrIsPlanningOnRetaking`
  - `candidate.IsPlanningToRetakeGcseMathsAndEnglish()` — similar check
  - If true, sets `HasGcseMathsAndEnglishId` / `PlanningToRetakeGcseMathsAndEnglishId` to the `HasOrIsPlanningOnRetaking` integer value
- Calls `candidate.AddressTelephone.StripExitCode()`
- Calls `CanSubscribe(candidate)`:
  1. If `!candidate.HasTeacherTrainingAdviser()` → return true
  2. If `candidate.AdviserStatusId == null` → return false
  3. Check if `AdviserStatusId` is defined in the `ResubscribableAdviserStatus` enum → return true/false
- **LINQ query #1**: `candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault()` — sets `QualificationId`, `DegreeSubject`, `UkDegreeGradeId`, `DegreeStatusId`, `DegreeTypeId`, `DegreeCountry`
- **LINQ query #2**: `candidate.PastTeachingPositions.OrderByDescending(q => q.CreatedAt).FirstOrDefault()` — sets `PastTeachingPositionId`, `SubjectTaughtId`

**Why it does more:** Degree status inference + `TeacherTrainingAdviserSignUp` DTO with GCSE checks (array `All()` comparisons), `CanSubscribe()` (3-branch logic calling `HasTeacherTrainingAdviser()` which is an OR condition), 2 LINQ queries over CRM-loaded collections, `StripExitCode()`, Hangfire background job.

**Files:** `Controllers/TeacherTrainingAdviser/CandidatesController.cs:62`, `Models/TeacherTrainingAdviser/TeacherTrainingAdviserSignUp.cs:214`

---

### 25. `POST /api/teacher_training_adviser/candidates/exchange_access_token/{accessToken}`

**Controller:** `TeacherTrainingAdviser.CandidatesController.ExchangeAccessToken(string accessToken, ExistingCandidateRequest request)`  
**Auth:** `Admin, GetAnAdviser, Apply, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Calls `_crm.MatchCandidate(request)` to find the candidate
3. Validates TOTP PIN via `_tokenService.IsValid()`
4. Returns `401 Unauthorized` if no match or invalid PIN
5. Returns `200 OK` with pre-populated `TeacherTrainingAdviserSignUp`:
   - GCSE checks (`HasGcseMathsAndEnglish`, `IsPlanningToRetake`)
   - `CanSubscribe()` (3 branches + `HasTeacherTrainingAdviser`)
   - 2 LINQ queries (qualifications + past teaching positions)
   - `StripExitCode()` on telephone

**Why it does more:** TOTP validation + DTO constructor with business logic.

**Files:** `Controllers/TeacherTrainingAdviser/CandidatesController.cs:101`, `Models/TeacherTrainingAdviser/TeacherTrainingAdviserSignUp.cs:214`

---

### 26. `POST /api/teacher_training_adviser/candidates/matchback`

**Controller:** `TeacherTrainingAdviser.CandidatesController.Matchback(ExistingCandidateRequest request)`  
**Auth:** `Admin, GetAnAdviser, Apply, GetIntoTeaching`

**Flow:**
1. Sets `request.Reference` from authenticated user identity
2. Validates `ModelState`
3. **Checks CRM integration pause** — if `_appSettings.IsCrmIntegrationPaused`, logs and returns `404 Not Found`
4. Calls `_crm.MatchCandidate(request)` — wrapped in try/catch:
   - On exception, logs and re-throws
5. If no candidate found, returns `404 Not Found`
6. Returns `200 OK` with pre-populated `TeacherTrainingAdviserSignUp` (GCSE checks, `CanSubscribe()`, 2 LINQ queries)

**Why it does more:** CRM pause handling + matchback + `TeacherTrainingAdviserSignUp` DTO constructor with GCSE checks, `CanSubscribe()`, 2 LINQ queries.

**Files:** `Controllers/TeacherTrainingAdviser/CandidatesController.cs:127`, `Models/TeacherTrainingAdviser/TeacherTrainingAdviserSignUp.cs:214`

---

---

## Summary

### Pure Passthrough (41 endpoints)

All read from the Postgres cache (populated by periodic `CrmSyncJob`) or use reflection — no CRM call on the request path, no business logic:

| Category | Count | Endpoints |
|----------|-------|-----------|
| Lookup items | 3 | `GET /api/lookup_items/countries`, `degree_countries`, `teaching_subjects` |
| Pick list items | 32 | `GET /api/pick_list_items/*` (32 CRM entity/attribute combinations) |
| Privacy policies | 2 | `GET /api/privacy_policies/latest`, `GET /api/privacy_policies/{id}` |
| Teaching events | 2 | `GET /api/teaching_event_buildings`, `GET /api/teaching_events/{readableId}` |
| Operations | 1 | `GET /api/operations/generate_mapping_info` (reflection) |
| Health | 1 | `GET /healthcheck` (inline "OK") |

### Doing More Than Passthrough (26 endpoints) — ranked by complexity

| Rank | Endpoint | Extraneous logic |
|------|----------|-----------------|
| 1 | `POST /api/schools_experience/candidates` | CandidateUpserter: 10+ save steps, data overwrite prevention, re-registration CRM action, dedup checks, quota tracking, graceful degradation, upfront ID generation; DTO: telephone fallback chain + 2 LINQ queries |
| 2 | `POST /api/teaching_events` | Dual persistence (CRM + cache), custom validation, order-dependent save (building before event), relationship nulling hack |
| 3 | `GET /api/teaching_events/search` | Google Geocoding API, NetTopologySuite spatial queries, dynamic expression trees, Prometheus histograms |
| 4 | `POST /api/candidates/access_tokens` | TOTP cryptographic PIN generation + GOV.UK Notify email + CRM pause handling + Prometheus metrics |
| 5 | `GET /api/callback_booking_quotas` | Fallback quota generation (5 weekdays, 30-min slots, timezone conversion) when CRM offline |
| 6 | `POST /api/teacher_training_adviser/candidates` | Degree status inference + DTO: GCSE checks (`HasGcseMathsAndEnglish`, `IsPlanningToRetake`), `CanSubscribe()` (3 branches + `HasTeacherTrainingAdviser`), 2 LINQ queries, `StripExitCode()` |
| 7 | `POST /api/mailing_list/members` | Degree status inference + DTO: LINQ query, `HasTeacherTrainingAdviser()` |
| 8 | `GET /api/mailing_list/members/exchange_magic_link_token/{magicLinkToken}` | Crypto token exchange with CRM lookup, uniqueness constraint (exactly 1 match), status tracking auto-upsert |
| 9 | `GET /api/schools_experience/candidates` / `{id}` | CRM call + DTO: telephone fallback chain (4 numbers with precedence), 2 LINQ queries, conditional assignments |
| 10 | `POST /api/schools_experience/candidates/{id}/school_experience` | Object construction + serialization + Hangfire |
| 11 | `POST /api/teacher_training_adviser/candidates/matchback` | CRM pause handling + DTO: GCSE checks, `CanSubscribe()`, 2 LINQ queries |
| 12 | `POST /api/teacher_training_adviser/candidates/exchange_access_token/{accessToken}` | TOTP validation + DTO: GCSE checks, `CanSubscribe()`, 2 LINQ queries |
| 13 | `POST /api/schools_experience/candidates/exchange_access_token/{accessToken}` | TOTP validation + DTO: telephone fallback + 2 LINQ queries |
| 14 | `POST /api/teaching_events/attendees/exchange_access_token/{accessToken}` | TOTP validation + DTO: LINQ query, `HasTeacherTrainingAdviser()`, `StripExitCode()` |
| 15 | `POST /api/teaching_events/attendees/exchange_unverified_request` | Unverified access mode + attribute sanitization + DTO with LINQ query |
| 16 | `POST /api/teaching_events/attendees` | Serialization + Hangfire |
| 17 | `POST /api/get_into_teaching/callbacks` | Serialization + Hangfire |
| 18 | `POST /api/get_into_teaching/callbacks/exchange_access_token/{accessToken}` | TOTP validation + DTO with `StripExitCode()` |
| 19 | `POST /api/get_into_teaching/callbacks/matchback` | Matchback + DTO with `StripExitCode()` |
| 20 | `POST /api/mailing_list/members/exchange_access_token/{accessToken}` | TOTP validation + DTO with LINQ query |
| 21 | `POST /api/operations/backfill_apply_candidates` | Guard check + Hangfire |
| 22 | `POST /api/operations/backfill_apply_candidates_from_ids` | Guard check + Hangfire |
| 23 | `GET /api/operations/health_check` | Composite health check across 5 services |
| 24 | `PUT /api/operations/pause_crm_integration` | State mutation with 6-hour auto-resume timer |
| 25 | `PUT /api/operations/resume_crm_integration` | State mutation |

### Key observations

- **The DTO constructors are doing real work.** `SchoolsExperienceSignUp`, `TeacherTrainingAdviserSignUp`, `MailingListAddMember`, and `TeachingEventAddAttendee` all contain LINQ-to-Object queries over CRM-loaded collections, conditional business logic, and subscription status checks. Three of the four (all except `MailingListAddMember`) also apply telephone number sanitization via `StripExitCode()`. This transformation logic is coupled to the response DTOs rather than being in a dedicated mapping/transformation layer.

- **Degree status inference** appears in two POST endpoints (`POST /api/mailing_list/members` and `POST /api/teacher_training_adviser/candidates`) and is explicitly described in comments as "a short-medium term solution" — temporary business logic that infers a degree status ID from a graduation year.

- **Background job orchestration** is used by 10+ endpoints. The controllers serialize candidates as change-tracked JSON and enqueue Hangfire jobs, decoupling the HTTP response from the actual CRM write. The `SchoolsExperience/CandidatesController.SignUp()` is the only endpoint that attempts a synchronous CRM write first (via `CandidateUpserter`), falling back to async queuing on failure.

- **CRM integration pause** is handled in 4 endpoints (`CreateAccessToken`, `TeacherTrainingAdviserMatchback`, `SchoolsExperienceSignUp`, and the pause/resume operations themselves), introducing conditional branching based on application state.
