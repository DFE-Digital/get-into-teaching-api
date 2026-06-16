# CRM API Spec

## **Endpoint:** `GET /api/pick_list_items/candidate/initial_teacher_training_years`

**Description:** Returns the list of initial teacher training year options from the `contact` entity's `dfe_ittyear` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                | CRM API response field |
|--------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_ittyear`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_ittyear`)   | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "2024/2025" },
  { "id": 2, "value": "2025/2026" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/preferred_education_phases`

**Description:** Returns the list of preferred education phase options from the `contact` entity's `dfe_preferrededucationphase01` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                        | CRM API response field |
|--------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_preferrededucationphase01`)       | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_preferrededucationphase01`)         | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Primary" },
  { "id": 2, "value": "Secondary" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/channels`

**Description:** Returns the list of candidate channel options (the channel through which a candidate was created) from the `contact` entity's `dfe_channelcreation` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                            | CRM API response field |
|--------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_channelcreation`)     | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_channelcreation`)       | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching" },
  { "id": 2, "value": "Get an Adviser" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/mailing_list_subscription_channels`

**Description:** Returns the list of mailing list subscription channel options from the `contact` entity's `dfe_gitismlservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                  | CRM API response field |
|------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitismlservicesubscriptionchannel`)         | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitismlservicesubscriptionchannel`)           | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching Website" },
  { "id": 2, "value": "Events" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/event_subscription_channels`

**Description:** Returns the list of event subscription channel options from the `contact` entity's `dfe_gitiseventsservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                       | CRM API response field |
|-----------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitiseventsservicesubscriptionchannel`)          | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitiseventsservicesubscriptionchannel`)            | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching Website" },
  { "id": 2, "value": "Events" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/teacher_training_adviser_subscription_channels`

**Description:** Returns the list of Teacher Training Adviser subscription channel options from the `contact` entity's `dfe_gitisttaservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                   | CRM API response field |
|-------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitisttaservicesubscriptionchannel`)         | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitisttaservicesubscriptionchannel`)           | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching Website" },
  { "id": 2, "value": "Get an Adviser" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/gcse_status`

**Description:** Returns the list of GCSE status options from the `contact` entity's `dfe_websitehasgcseenglish` field in Dynamics. Used to indicate whether a candidate has (or is planning to retake) their GCSE English/Maths.

**CRM-side mapping:**

| Dynamics source                                                                                       | CRM API response field |
|-------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websitehasgcseenglish`)          | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websitehasgcseenglish`)            | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Yes" },
  { "id": 2, "value": "No" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.
- The same pick-list values are used for multiple GCSE fields on the contact (`dfe_websitehasgcseenglish`, `dfe_websitehasgcsemaths`, `dfe_websitehasgcsescience`). Only the English field's metadata is queried here; the CRM team should confirm all three attributes share the same option set.


## **Endpoint:** `GET /api/pick_list_items/candidate/retake_gcse_status`

**Description:** Returns the list of retake GCSE status options from the `contact` entity's `dfe_websiteplanningretakeenglishgcse` field in Dynamics. Used to indicate whether a candidate is planning to retake their GCSE English/Maths.

**CRM-side mapping:**

| Dynamics source                                                                                              | CRM API response field |
|--------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websiteplanningretakeenglishgcse`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websiteplanningretakeenglishgcse`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Yes" },
  { "id": 2, "value": "No" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.
- As with `gcse_status`, the same option set is reused across multiple retake fields (`dfe_websiteplanningretakeenglishgcse`, `dfe_websiteplanningretakemathsgcse`, `dfe_websiteplanningretakesciencegcse`). Only the English field's metadata is queried here.


## **Endpoint:** `GET /api/pick_list_items/candidate/consideration_journey_stages`

**Description:** Returns the list of consideration journey stage options from the `contact` entity's `dfe_websitewhereinconsiderationjourney` field in Dynamics. Captures where a candidate is in their decision to pursue teaching.

**CRM-side mapping:**

| Dynamics source                                                                                                     | CRM API response field |
|---------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websitewhereinconsiderationjourney`)           | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websitewhereinconsiderationjourney`)             | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "I'm not sure and finding out more" },
  { "id": 2, "value": "It's something I'm seriously thinking about" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/adviser_eligibilities`

**Description:** Returns the list of adviser eligibility options from the `contact` entity's `dfe_iscandidateeligibleforadviser` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                           | CRM API response field |
|-----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_iscandidateeligibleforadviser`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_iscandidateeligibleforadviser`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Yes" },
  { "id": 2, "value": "No" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/adviser_requirements`

**Description:** Returns the list of adviser requirement options from the `contact` entity's `dfe_isadvisorrequiredos` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                      | CRM API response field |
|------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_isadvisorrequiredos`)           | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_isadvisorrequiredos`)             | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Yes" },
  { "id": 2, "value": "No" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/types`

**Description:** Returns the list of candidate type options from the `contact` entity's `dfe_typeofcandidate` field in Dynamics. Indicates whether a candidate is interested in teaching for the first time or returning to teaching.

**CRM-side mapping:**

| Dynamics source                                                                               | CRM API response field |
|-----------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_typeofcandidate`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_typeofcandidate`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Interested in Teaching" },
  { "id": 2, "value": "Returning to Teaching" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/assignment_status`

**Description:** Returns the list of candidate assignment status options from the `contact` entity's `dfe_candidatestatus` field in Dynamics. Indicates the current adviser assignment state of a candidate.

**CRM-side mapping:**

| Dynamics source                                                                              | CRM API response field |
|----------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_candidatestatus`)       | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_candidatestatus`)         | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Waiting to be assigned" },
  { "id": 2, "value": "Assigned" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/situations`

**Description:** Returns the list of candidate situation options from the `contact` entity's `dfe_situation` field in Dynamics. Captures the candidate's current life/career stage.

**CRM-side mapping:**

| Dynamics source                                                                         | CRM API response field |
|-----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_situation`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_situation`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "At university" },
  { "id": 2, "value": "Working in a school" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/citizenships`

**Description:** Returns the list of citizenship options from the `contact` entity's `dfe_citizenship` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                           | CRM API response field |
|-------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_citizenship`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_citizenship`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "UK citizen" },
  { "id": 2, "value": "EU citizen" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/visa_statuses`

**Description:** Returns the list of visa status options from the `contact` entity's `dfe_visastatus` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                            | CRM API response field |
|--------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_visastatus`)          | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_visastatus`)            | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "I already have a visa" },
  { "id": 2, "value": "I need a visa" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/locations`

**Description:** Returns the list of location options from the `contact` entity's `dfe_location` field in Dynamics. Captures where the candidate is currently based.

**CRM-side mapping:**

| Dynamics source                                                                         | CRM API response field |
|-----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_location`)         | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_location`)           | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "UK" },
  { "id": 2, "value": "Overseas" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/candidate/has_qualified_teacher_statuses`

**Description:** Returns the list of Qualified Teacher Status (QTS) options from the `contact` entity's `dfe_qtsstatus` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                         | CRM API response field |
|-----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_qtsstatus`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_qtsstatus`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Yes" },
  { "id": 2, "value": "No" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/qualification/degree_status`

**Description:** Returns the list of degree status options from the `dfe_candidatequalification` entity's `dfe_degreestatus` field in Dynamics. Indicates the candidate's current stage of degree study.

**CRM-side mapping:**

| Dynamics source                                                                                         | CRM API response field |
|---------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_degreestatus`)  | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_degreestatus`)    | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Final year" },
  { "id": 2, "value": "Has a degree" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/qualification/types`

**Description:** Returns the list of qualification type options from the `dfe_candidatequalification` entity's `dfe_type` field in Dynamics (e.g. degree, degree equivalent).

**CRM-side mapping:**

| Dynamics source                                                                                 | CRM API response field |
|-------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_type`)  | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_type`)    | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Degree" },
  { "id": 2, "value": "Degree equivalent" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/qualification/uk_degree_grades`

**Description:** Returns the list of UK degree grade options from the `dfe_candidatequalification` entity's `dfe_ukdegreegrade` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                           | CRM API response field |
|-----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_ukdegreegrade`)   | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_ukdegreegrade`)     | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "First class" },
  { "id": 2, "value": "Upper second (2:1)" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.
- The current API applies an additional validation constraint (only `FirstClass`, `UpperSecond`, `LowerSecond`, `NotApplicable` are considered valid for TTA sign-up), but this filtering is done in the validator — not here. The endpoint returns all grades stored in Dynamics.


## **Endpoint:** `GET /api/pick_list_items/past_teaching_position/education_phases`

**Description:** Returns the list of education phase options from the `dfe_candidatepastteachingposition` entity's `dfe_educationphase` field in Dynamics. Used to describe the phase a returning teacher previously taught in.

**CRM-side mapping:**

| Dynamics source                                                                                                      | CRM API response field |
|----------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatepastteachingposition`, attribute `dfe_educationphase`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatepastteachingposition`, attribute `dfe_educationphase`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Primary" },
  { "id": 2, "value": "Secondary" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/teaching_event/types`

**Description:** Returns the list of teaching event type options from the `msevtmgt_event` entity's `dfe_event_type` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                               | CRM API response field |
|-----------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_event_type`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_event_type`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Online event" },
  { "id": 2, "value": "School or University event" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/teaching_event/regions`

**Description:** Returns the list of teaching event region options from the `msevtmgt_event` entity's `dfe_eventregion` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                | CRM API response field |
|------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventregion`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventregion`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "London" },
  { "id": 2, "value": "South East" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/teaching_event/status`

**Description:** Returns the list of teaching event status options from the `msevtmgt_event` entity's `dfe_eventstatus` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                | CRM API response field |
|------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventstatus`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventstatus`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Open" },
  { "id": 2, "value": "Closed" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/teaching_event_registration/channels`

**Description:** Returns the list of teaching event registration channel options from the `msevtmgt_eventregistration` entity's `dfe_channelcreation` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                              | CRM API response field |
|--------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_eventregistration`, attribute `dfe_channelcreation`)    | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_eventregistration`, attribute `dfe_channelcreation`)      | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching Website" },
  { "id": 2, "value": "Events" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.
- Note: the attribute name `dfe_channelcreation` is shared across multiple entities (`contact`, `msevtmgt_eventregistration`, `phonecall`). Each entity may have different option set values — these are the values for the event registration entity specifically.


## **Endpoint:** `GET /api/pick_list_items/phone_call/channels`

**Description:** Returns the list of phone call channel options from the `phonecall` entity's `dfe_channelcreation` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                               | CRM API response field |
|-----------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `phonecall`, attribute `dfe_channelcreation`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `phonecall`, attribute `dfe_channelcreation`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Callback Request" },
  { "id": 2, "value": "Get Into Teaching Website" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.
- Note: `dfe_channelcreation` is shared across multiple entities — these are the values for the `phonecall` entity specifically.


## **Endpoint:** `GET /api/pick_list_items/service_subscription/types`

**Description:** Returns the list of service subscription type options from the `dfe_servicesubscription` entity's `dfe_servicesubscriptiontype` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                   | CRM API response field |
|-------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_servicesubscription`, attribute `dfe_servicesubscriptiontype`)    | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_servicesubscription`, attribute `dfe_servicesubscriptiontype`)      | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Mailing List" },
  { "id": 2, "value": "Teacher Training Adviser" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/sources`

**Description:** Returns the list of contact creation channel source options from the `dfe_contactchannelcreation` entity's `dfe_creationchannelsource` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                        | CRM API response field |
|------------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelsource`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelsource`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Get Into Teaching Website" },
  { "id": 2, "value": "School Experience" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/services`

**Description:** Returns the list of contact creation channel service options from the `dfe_contactchannelcreation` entity's `dfe_creationchannelservice` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                         | CRM API response field |
|-------------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelservice`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelservice`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Teacher Training Adviser Service" },
  { "id": 2, "value": "Created on School Experience" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/activities`

**Description:** Returns the list of contact creation channel activity options from the `dfe_contactchannelcreation` entity's `dfe_creationchannelactivities` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                            | CRM API response field |
|----------------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelactivities`)        | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelactivities`)          | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Sign Up" },
  { "id": 2, "value": "Event Registration" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.


## **Endpoint:** `GET /api/pick_list_items/teaching_event/accessibility_items`

**Description:** Returns the list of teaching event accessibility status options from the `msevtmgt_event` entity's `dfe_accessibility` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                  | CRM API response field |
|--------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_accessibility`)      | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_accessibility`)        | `value` (string)       |

**Response format:**

```json
[
  { "id": 1, "value": "Accessible" },
  { "id": 2, "value": "Not accessible" }
]
```

**Ordering:** By `id` ascending.

**Notes:**
- This is a simple pass-through — no business logic, filtering, or computed fields.

