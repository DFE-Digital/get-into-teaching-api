## POST `/api/teacher_training_adviser/candidates`

Please check existing code and swagger doc for reference. This is a complicated endpoint and I might have made mistakes or missed something here.
https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html

**File:** `Controllers/TeacherTrainingAdviser/CandidatesController.cs:54`

### Context

Signs up a candidate for the Teacher Training Adviser service. Validates the request, builds a `Candidate` with extensive business logic (GCSE defaults, qualification, past teaching positions, adviser eligibility, privacy policy, phone call scheduling, subscription), infers degree status from graduation year, serializes with change tracking. Returns the inferred degree status immediately — CRM upsert is async.

The `degreeStatusId` field has historically been set directly by clients (including Apply). However, the GIT website now sends `graduationYear` instead, and the C# API infers `degreeStatusId` from it. The degree status values `222750001` (Final year), `222750002` (Second year), and `222750003` (First year) are stale — they describe a snapshot in time that requires manual CRM updates as the student progresses. The new approach replaces these with `222750006` (Not yet, I'm studying for one) + a `graduationYear` from which the system infers the year/status.

**Clients using this endpoint:** GIT website (main consumer) and Apply.

### Proposal

1. **Reject stale degree statuses**: the API should reject `222750001` (Final year), `222750002` (Second year), and `222750003` (First year) with a validation error. Clients (including Apply) must send `222750006` + `graduationYear` instead.

2. **Graduation year required when studying**: if `degreeStatusId == 222750006` (Not yet, I'm studying for one), then `graduationYear` must be provided. Graduation year is not required and should not be provided for any other degree status (`222750000` Graduate/postgraduate, `222750004` No degree, `222750005` Other).

3. **Keep inference in C# API**: the logic that infers `degreeStatusId` from `graduationYear` (Final year / Second year / First year) stays in the C# API (not moved to CRM or Ruby API). The Ruby API remains "dumb" — it passes `graduationYear` through and lets the C# API handle inference.

4. **Validation stays in C# API**: rather than duplicating validation logic across Ruby and C# APIs, the C# API is the enforcement point for these rules. The behaviour should be clearly documented so the CRM's contract is well-defined.

5. **Handle re-registering candidates in C# API**: 

   1. The re-registration process should be handled within the C# API (and related CRM Actions) for candidates who are re-registering for the TTA service, i.e. candidates who have the following attributes:
      ```
      an existing CandidateID 
      HasTeacherTrainingAdviserSubscription / dfe_gitisttaserviceissubscriber = Yes
      dfe_candidateadviserstatus = Closed
      AdviserStatusId / dfe_candidateAdviserStatusReason with a _re-subscribable_ closed status 
      ```
   
   2. A re-subscribable `AdviserStatusId` / `dfe_candidateAdviserStatusReason` is any of the following:
   
       ```
       AcceptedIttOffer = 222750000
       AcceptedTeachingPositionOutOfRemitRtt = 222750031
       AcceptedTeachingPositionRtt = 222750004
       AlreadyHasQts = 222750005
       CandidateAskedToBeLeftAlone = 222750006
       CandidateWasIncorrectlyAllocated = 222750007
       DegreeNotEligible = 222750058
       DidntRespondToEnableSupportEmailOnly = 222750062
       DidntRespondToEnableSupportPhoneAndEmail = 222750057
       DoesNotWantToTrainInEngland = 222750010
       DuplicateRecordDuplicateCandidate = 222750028
       EnicForeignQualificationsNotEquivalent = 222750050
       FirstConversationWantsToApplyInAFutureIttYear = 222750061
       GainedIttPlaceBeforeAdviserAllocation = 222750034
       GcseOrEquivalentUnattainable = 222750064
       IncompleteContactDetailsUnableToTraceFurther = 222750012
       InterestedInTeachingPrimaryOnly = 222750065
       NoLongerPursuingIttEyRouteInstead = 222750063
       NoLongerPursuingIttFeRouteInstead = 222750048
       NoLongerPursuingIttFinanceReasons = 222750052
       NoLongerPursuingIttFollowingSchoolExperience = 222750055
       NoLongerPursuingIttFoundOtherEmploymentTraining = 222750053
       NoLongerPursuingIttInternationalFinanceReasons = 222750051
       NoLongerPursuingIttOtherTeachingRelatedReasons = 222750056
       NoLongerPursuingIttPersonalReasons = 222750054
       NoQtsRtt = 222750019
       ReferredToTeachingOutsideOfEnglandRtt = 222750024
       SeekingQtsConversionFromInternationalQualifications = 222750040
       StoppedRespondingToContactFromAdviser = 222750060
       Unsuccessful = 222750029
       VisaNoRightToStudyInTheUk = 222750059
       VisaNoRightToStudyInTheUkAndReceiveFunding = 222750025
       WantsToApplyInAFutureIttYear = 222750026
       ```
      
   3. Re-register the candidate by setting the following fields and values via the GIT API:
   
       ```ruby 
       AdviserEligibilityId / dfe_IsCandidateEligibleForAdviser = No (222750001)
       AdviserRequirementId / dfe_IsAdvisorRequiredOS = Yes (222750001)
       ```
      
   4. Trigger the CRM Re-register Action (`dfe_ReRegisterCandidate`) which will in turn set the folllowing fields and values directly within the CRM:
   
       ```
       dfe_CandidateStatus = Waiting to be Assigned (222750001) 
       dfe_CandidateAdviserStatus = Open (222750000)
       dfe_CandidateReregisterStatus = Re-registered (222750000) 
       dfe_CandidateAdviserStatusReason = null
       dfe_GITISTTAServiceStartDate = DateTime.Now
       ```      

### Validation changes needed

| Change | Detail |
|--------|--------|
| Reject `degreeStatusId` 222750001, 222750002, 222750003 | Return validation error if any of these are sent |
| Require `graduationYear` when `degreeStatusId == 222750006` | New `NotNull` rule gated on studying status |
| `graduationYear` not required for non-studying statuses | `graduationYear` can be null for graduate, no degree, other |
| Existing inference logic unchanged | `GraduationYear` → `DegreeStatusId` mapping remains in `DegreeStatusInference` |


## Request

```json
{
  "candidateId": null,
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1995-06-15",
  "addressTelephone": "07123456789",
  "addressPostcode": "TE5 1IN",
  "teacherId": null,
  "typeId": 222750000,
  "ukDegreeGradeId": 222750001,
  "degreeTypeId": 222750000,
  "degreeStatusId": 222750000,
  "degreeSubject": "Mathematics",
  "initialTeacherTrainingYearId": 223360001,
  "preferredEducationPhaseId": null,
  "preferredTeachingSubjectId": null,
  "hasGcseMathsAndEnglishId": null,
  "hasGcseScienceId": null,
  "planningToRetakeGcseMathsAndEnglishId": null,
  "planningToRetakeGcseScienceId": null,
  "adviserStatusId": null,
  "channelId": null,
  "acceptedPolicyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "countryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "graduationYear": 2024,
  "stageTaughtId": null,
  "subjectTaughtId": null,
  "pastTeachingPositionId": null,
  "qualificationId": null,
  "phoneCallScheduledAt": null,
  "situation": null,
  "citizenship": null,
  "visaStatus": null,
  "location": null,
  "degreeCountry": null
}
```

### Field details

| Param | Type | Required | Notes |
|-------|------|----------|-------|
| `email` | `string` | **Yes** | |
| `firstName` | `string` | **Yes** | |
| `lastName` | `string` | **Yes** | |
| `dateOfBirth` | `DateTime` | **Yes** | |
| `acceptedPolicyId` | `Guid` | **Yes** | |
| `countryId` | `Guid` | **Yes** | |
| `typeId` | `int` | **Yes** | `222750000` = InterestedInTeacherTraining, `222750001` = ReturningToTeacherTraining |
| `addressTelephone` | `string` | Conditional | Required if `phoneCallScheduledAt` is set |
| `addressPostcode` | `string` | Conditional | Required if `countryId` is UK, unless `degreeCountry` is "AnotherCountry" |
| `phoneCallScheduledAt` | `DateTime` | Conditional | Must be future; only allowed when `degreeTypeId == DegreeEquivalent` |
| `degreeTypeId` | `int` | Conditional | Required for InterestedInTeacherTraining — must be `Degree` or `DegreeEquivalent` (or just `Degree` if studying) |
| `degreeStatusId` | `int` | Conditional | Cannot be `NoDegree` for InterestedInTeacherTraining |
| `ukDegreeGradeId` | `int` | Conditional | Required when `degreeStatusId == HasDegree` and `degreeTypeId == Degree`; must be one of `FirstClass`, `UpperSecond`, `LowerSecond`, `NotApplicable` |
| `degreeSubject` | `string` | Conditional | Required unless `degreeTypeId == DegreeEquivalent` |
| `initialTeacherTrainingYearId` | `int` | Conditional | Required for InterestedInTeacherTraining with a degree (unless `degreeCountry` is AnotherCountry) |
| `preferredEducationPhaseId` | `int` | Conditional | Required for InterestedInTeacherTraining with a degree (unless degree is overseas) |
| `preferredTeachingSubjectId` | `Guid` | Conditional | Required when phase is Secondary (both new and returning candidates) |
| `subjectTaughtId` | `Guid` | Conditional | Required for returning to teaching when stage taught is Secondary/null |
| `stageTaughtId` | `int` | No | Primary/Secondary — determines past teaching position education phase |
| `hasGcseMathsAndEnglishId` | `int` | Conditional | Required (along with `ukDegreeGradeId`) when has a UK degree and phase is set — must be `HasOrIsPlanningOnRetaking` |
| `planningToRetakeGcseMathsAndEnglishId` | `int` | Alternative | Accepted as alternative to `hasGcseMathsAndEnglishId` for the GCSE requirement |
| `situation` | `int` | No | Validated against CRM `dfe_situation` picklist |
| `citizenship` | `int` | No | Validated against CRM `dfe_citizenship` picklist |
| `visaStatus` | `int` | No | Validated against CRM `dfe_visastatus` picklist |
| `location` | `int` | No | Validated against CRM `dfe_location` picklist |
| `degreeCountry` | `Guid` | No | Must be in the list of valid degree countries |
| `candidateId` | `Guid` | No | Set for existing candidates (matchback/exchange) — null for new sign-ups |
| `graduationYear` | `int` | No | Used to infer `degreeStatusId` and set `inferredGraduationDate` (Aug 31st) |
| `teacherId` | `string` | No | |
| `adviserStatusId` | `int` | No | If set to a resubscribable closed status, `HasReRegistered` becomes true |
| `channelId` | `int` | No | Write-only; overrides the default TTA channel ID |

## Responses

### `200 OK`

```json
{
  "degreeStatusId": 222750000
}
```

### `400 Bad Request` — validation failed - New proposed error format

```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "First Name must not be empty"
        }
    ]
}
```
