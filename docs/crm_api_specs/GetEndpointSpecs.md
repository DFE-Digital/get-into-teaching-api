# CRM API Spec

## **Endpoint:** `GET /api/pick_list_items/candidate/initial_teacher_training_years`

**Description:** Returns the list of initial teacher training year options from the `contact` entity's `dfe_ittyear`
field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                | CRM API response field |
|--------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_ittyear`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_ittyear`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "2024/2025"
  },
  {
    "id": 2,
    "value": "2025/2026"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/preferred_education_phases`

**Description:** Returns the list of preferred education phase options from the `contact` entity's
`dfe_preferrededucationphase01` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                  | CRM API response field |
|--------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_preferrededucationphase01`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_preferrededucationphase01`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Primary"
  },
  {
    "id": 2,
    "value": "Secondary"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/channels`

**Description:** Returns the list of candidate channel options (the channel through which a candidate was created) from
the `contact` entity's `dfe_channelcreation` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                        | CRM API response field |
|----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_channelcreation`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_channelcreation`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching"
  },
  {
    "id": 2,
    "value": "Get an Adviser"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/mailing_list_subscription_channels`

**Description:** Returns the list of mailing list subscription channel options from the `contact` entity's
`dfe_gitismlservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                          | CRM API response field |
|----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitismlservicesubscriptionchannel`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitismlservicesubscriptionchannel`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching Website"
  },
  {
    "id": 2,
    "value": "Events"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/event_subscription_channels`

**Description:** Returns the list of event subscription channel options from the `contact` entity's
`dfe_gitiseventsservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                              | CRM API response field |
|--------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitiseventsservicesubscriptionchannel`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitiseventsservicesubscriptionchannel`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching Website"
  },
  {
    "id": 2,
    "value": "Events"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/teacher_training_adviser_subscription_channels`

**Description:** Returns the list of Teacher Training Adviser subscription channel options from the `contact` entity's
`dfe_gitisttaservicesubscriptionchannel` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                           | CRM API response field |
|-----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_gitisttaservicesubscriptionchannel`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_gitisttaservicesubscriptionchannel`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching Website"
  },
  {
    "id": 2,
    "value": "Get an Adviser"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/gcse_statuses`

**Description:** Returns the list of GCSE status options from the `contact` entity's `dfe_websitehasgcseenglish` field
in Dynamics. Used to indicate whether a candidate has (or is planning to retake) their GCSE English/Maths.

**CRM-side mapping:**

| Dynamics source                                                                              | CRM API response field |
|----------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websitehasgcseenglish`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websitehasgcseenglish`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Yes"
  },
  {
    "id": 2,
    "value": "No"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.
- The same pick-list values are used for multiple GCSE fields on the contact (`dfe_websitehasgcseenglish`,
  `dfe_websitehasgcsemaths`, `dfe_websitehasgcsescience`). Only the English field's metadata is queried here; the CRM
  team should confirm all three attributes share the same option set.

## **Endpoint:** `GET /api/pick_list_items/candidate/retake_gcse_statuses`

**Description:** Returns the list of retake GCSE status options from the `contact` entity's
`dfe_websiteplanningretakeenglishgcse` field in Dynamics. Used to indicate whether a candidate is planning to retake
their GCSE English/Maths.

**CRM-side mapping:**

| Dynamics source                                                                                         | CRM API response field |
|---------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websiteplanningretakeenglishgcse`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websiteplanningretakeenglishgcse`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Yes"
  },
  {
    "id": 2,
    "value": "No"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.
- As with `gcse_status`, the same option set is reused across multiple retake fields (
  `dfe_websiteplanningretakeenglishgcse`, `dfe_websiteplanningretakemathsgcse`, `dfe_websiteplanningretakesciencegcse`).
  Only the English field's metadata is queried here.

## **Endpoint:** `GET /api/pick_list_items/candidate/consideration_journey_stages`

**Description:** Returns the list of consideration journey stage options from the `contact` entity's
`dfe_websitewhereinconsiderationjourney` field in Dynamics. Captures where a candidate is in their decision to pursue
teaching.

**CRM-side mapping:**

| Dynamics source                                                                                           | CRM API response field |
|-----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_websitewhereinconsiderationjourney`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_websitewhereinconsiderationjourney`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "I'm not sure and finding out more"
  },
  {
    "id": 2,
    "value": "It's something I'm seriously thinking about"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/adviser_eligibilities`

**Description:** Returns the list of adviser eligibility options from the `contact` entity's
`dfe_iscandidateeligibleforadviser` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                      | CRM API response field |
|------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_iscandidateeligibleforadviser`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_iscandidateeligibleforadviser`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Yes"
  },
  {
    "id": 2,
    "value": "No"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/adviser_requirements`

**Description:** Returns the list of adviser requirement options from the `contact` entity's `dfe_isadvisorrequiredos`
field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                            | CRM API response field |
|--------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_isadvisorrequiredos`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_isadvisorrequiredos`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Yes"
  },
  {
    "id": 2,
    "value": "No"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/types`

**Description:** Returns the list of candidate type options from the `contact` entity's `dfe_typeofcandidate` field in
Dynamics. Indicates whether a candidate is interested in teaching for the first time or returning to teaching.

**CRM-side mapping:**

| Dynamics source                                                                        | CRM API response field |
|----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_typeofcandidate`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_typeofcandidate`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Interested in Teaching"
  },
  {
    "id": 2,
    "value": "Returning to Teaching"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/assignment_statuses`

**Description:** Returns the list of candidate assignment status options from the `contact` entity's
`dfe_candidatestatus` field in Dynamics. Indicates the current adviser assignment state of a candidate.

**CRM-side mapping:**

| Dynamics source                                                                        | CRM API response field |
|----------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_candidatestatus`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_candidatestatus`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Waiting to be assigned"
  },
  {
    "id": 2,
    "value": "Assigned"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/situations`

**Description:** Returns the list of candidate situation options from the `contact` entity's `dfe_situation` field in
Dynamics. Captures the candidate's current life/career stage.

**CRM-side mapping:**

| Dynamics source                                                                  | CRM API response field |
|----------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_situation`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_situation`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "At university"
  },
  {
    "id": 2,
    "value": "Working in a school"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/citizenships`

**Description:** Returns the list of citizenship options from the `contact` entity's `dfe_citizenship` field in
Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                    | CRM API response field |
|------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_citizenship`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_citizenship`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "UK citizen"
  },
  {
    "id": 2,
    "value": "EU citizen"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/visa_statuses`

**Description:** Returns the list of visa status options from the `contact` entity's `dfe_visastatus` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                   | CRM API response field |
|-----------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_visastatus`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_visastatus`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "I already have a visa"
  },
  {
    "id": 2,
    "value": "I need a visa"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/locations`

**Description:** Returns the list of location options from the `contact` entity's `dfe_location` field in Dynamics.
Captures where the candidate is currently based.

**CRM-side mapping:**

| Dynamics source                                                                 | CRM API response field |
|---------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_location`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_location`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "UK"
  },
  {
    "id": 2,
    "value": "Overseas"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/candidate/has_qualified_teacher_statuses`

**Description:** Returns the list of Qualified Teacher Status (QTS) options from the `contact` entity's `dfe_qtsstatus`
field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                  | CRM API response field |
|----------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `contact`, attribute `dfe_qtsstatus`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `contact`, attribute `dfe_qtsstatus`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Yes"
  },
  {
    "id": 2,
    "value": "No"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/qualification/degree_statuses`

**Description:** Returns the list of degree status options from the `dfe_candidatequalification` entity's
`dfe_degreestatus` field in Dynamics. Indicates the candidate's current stage of degree study.

**CRM-side mapping:**

| Dynamics source                                                                                        | CRM API response field |
|--------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_degreestatus`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_degreestatus`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 222750000,
    "value": "Graduate or postgraduate"
  },
  {
    "id": 222750004,
    "value": "I don't have a degree and am not studying for one"
  },
  {
    "id": 222750005,
    "value": "Other"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

-- This is a simple pass-through — no business logic, filtering, or computed fields.

**Proposed changes:**
- The degree statuses `222750001` (Final year), `222750002` (Second year), and `222750003` (First year) will been retired in favour of `222750006` (Not yet, I'm studying for one) + `graduationYear` (see proposed changes in the sign-up TTA POST endpoint). These retired statuses are filtered out of the response.

## **Endpoint:** `GET /api/pick_list_items/qualification/types`

**Description:** Returns the list of qualification type options from the `dfe_candidatequalification` entity's`dfe_type`
field in Dynamics (e.g. degree, degree equivalent).

**CRM-side mapping:**

| Dynamics source                                                                                | CRM API response field |
|------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_type`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_type`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Degree"
  },
  {
    "id": 2,
    "value": "Degree equivalent"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/qualification/uk_degree_grades`

**Description:** Returns the list of UK degree grade options from the `dfe_candidatequalification` entity's
`dfe_ukdegreegrade` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                         | CRM API response field |
|---------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_ukdegreegrade`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatequalification`, attribute `dfe_ukdegreegrade`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "First class"
  },
  {
    "id": 2,
    "value": "Upper second (2:1)"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.
- The current API applies an additional validation constraint (only `FirstClass`, `UpperSecond`, `LowerSecond`,
  `NotApplicable` are considered valid for TTA sign-up), but this filtering is done in the validator — not here. The
  endpoint returns all grades stored in Dynamics.

## **Endpoint:** `GET /api/pick_list_items/past_teaching_position/education_phases`

**Description:** Returns the list of education phase options from the `dfe_candidatepastteachingposition` entity's
`dfe_educationphase` field in Dynamics. Used to describe the phase a returning teacher previously taught in.

**CRM-side mapping:**

| Dynamics source                                                                                                 | CRM API response field |
|-----------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_candidatepastteachingposition`, attribute `dfe_educationphase`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_candidatepastteachingposition`, attribute `dfe_educationphase`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Primary"
  },
  {
    "id": 2,
    "value": "Secondary"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/teaching_event/types`

**Description:** Returns the list of teaching event type options from the `msevtmgt_event` entity's `dfe_event_type`
field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                          | CRM API response field |
|------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_event_type`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_event_type`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Online event"
  },
  {
    "id": 2,
    "value": "School or University event"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/teaching_event/regions`

**Description:** Returns the list of teaching event region options from the `msevtmgt_event` entity's `dfe_eventregion`
field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                           | CRM API response field |
|-------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventregion`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventregion`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "London"
  },
  {
    "id": 2,
    "value": "South East"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/teaching_event/statuses`

**Description:** Returns the list of teaching event status options from the `msevtmgt_event` entity's `dfe_eventstatus`
field in Dynamics.


**CRM-side mapping:**

| Dynamics source                                                                           | CRM API response field |
|-------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventstatus`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_eventstatus`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Open"
  },
  {
    "id": 2,
    "value": "Closed"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/teaching_event_registration/channels`

**Description:** Returns the list of teaching event registration channel options from the `msevtmgt_eventregistration`
entity's `dfe_channelcreation` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                           | CRM API response field |
|-----------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_eventregistration`, attribute `dfe_channelcreation`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_eventregistration`, attribute `dfe_channelcreation`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching Website"
  },
  {
    "id": 2,
    "value": "Events"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.
- Note: the attribute name `dfe_channelcreation` is shared across multiple entities (`contact`,
  `msevtmgt_eventregistration`, `phonecall`). Each entity may have different option set values — these are the values
  for the event registration entity specifically.

## **Endpoint:** `GET /api/pick_list_items/phone_call/channels`

**Description:** Returns the list of phone call channel options from the `phonecall` entity's `dfe_channelcreation`field
in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                          | CRM API response field |
|------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `phonecall`, attribute `dfe_channelcreation`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `phonecall`, attribute `dfe_channelcreation`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Callback Request"
  },
  {
    "id": 2,
    "value": "Get Into Teaching Website"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.
- Note: `dfe_channelcreation` is shared across multiple entities — these are the values for the `phonecall` entity
  specifically.

## **Endpoint:** `GET /api/pick_list_items/service_subscription/types`

**Description:** Returns the list of service subscription type options from the `dfe_servicesubscription` entity's
`dfe_servicesubscriptiontype` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                | CRM API response field |
|----------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_servicesubscription`, attribute `dfe_servicesubscriptiontype`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_servicesubscription`, attribute `dfe_servicesubscriptiontype`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Mailing List"
  },
  {
    "id": 2,
    "value": "Teacher Training Adviser"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/sources`

**Description:** Returns the list of contact creation channel source options from the `dfe_contactchannelcreation`
entity's `dfe_creationchannelsource` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                 | CRM API response field |
|-----------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelsource`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelsource`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Get Into Teaching Website"
  },
  {
    "id": 2,
    "value": "School Experience"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/services`

**Description:** Returns the list of contact creation channel service options from the `dfe_contactchannelcreation`
entity's `dfe_creationchannelservice` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                  | CRM API response field |
|------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelservice`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelservice`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Teacher Training Adviser Service"
  },
  {
    "id": 2,
    "value": "Created on School Experience"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/contact_creation_channel/activities`

**Description:** Returns the list of contact creation channel activity options from the `dfe_contactchannelcreation`
entity's `dfe_creationchannelactivities` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                                                     | CRM API response field |
|---------------------------------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelactivities`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `dfe_contactchannelcreation`, attribute `dfe_creationchannelactivities`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Sign Up"
  },
  {
    "id": 2,
    "value": "Event Registration"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/pick_list_items/teaching_event/accessibility_items`

**Description:** Returns the list of teaching event accessibility status options from the `msevtmgt_event` entity's
`dfe_accessibility` field in Dynamics.

**CRM-side mapping:**

| Dynamics source                                                                             | CRM API response field |
|---------------------------------------------------------------------------------------------|------------------------|
| `PickListItemId` (from metadata for entity `msevtmgt_event`, attribute `dfe_accessibility`) | `id` (integer)         |
| `DisplayLabel` (from metadata for entity `msevtmgt_event`, attribute `dfe_accessibility`)   | `value` (string)       |

**Response format:**

```json
[
  {
    "id": 1,
    "value": "Accessible"
  },
  {
    "id": 2,
    "value": "Not accessible"
  }
]
```

**Ordering:** By `id` ascending.

**Notes:**

- This is a simple pass-through — no business logic, filtering, or computed fields.

## **Endpoint:** `GET /api/callback_booking_quotas`

**Description:** Returns all available callback booking time slots directly from the CRM (`dfe_callbackbookingquota`
entity). Unlike most read endpoints this is a **live CRM call at request time** — data is not cached locally. If the CRM
is unreachable, a synthetic fallback list of quotas is generated server-side (weekdays only, 09:00–17:00 in 30-minute
slots, for the next 5 working days).

**CRM-side mapping:**

| Dynamics field                | Response field           | Notes                                            |
|-------------------------------|--------------------------|--------------------------------------------------|
| `dfe_name`                    | `timeSlot` (string)      | Human-readable time slot label                   |
| `dfe_workingdayname`          | `day` (string)           | Human-readable day label                         |
| `dfe_starttime`               | `startAt` (datetime)     |                                                  |
| `dfe_endtime`                 | `endAt` (datetime)       |                                                  |
| `dfe_websitenumberofbookings` | `numberOfBookings` (int) |                                                  |
| `dfe_websitequota`            | `quota` (int)            |                                                  |
| *(computed)*                  | `isAvailable` (bool)     | `numberOfBookings < quota` — derived, not in CRM |

**Response format:**

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "timeSlot": "09:00 - 09:30",
    "day": "Monday, 16 June 2026",
    "startAt": "2026-06-16T09:00:00Z",
    "endAt": "2026-06-16T09:30:00Z",
    "numberOfBookings": 2,
    "quota": 5,
    "isAvailable": true
  }
]
```

**Ordering:** As returned by Dynamics (no explicit ordering applied in current code).

**Notes:**

- `isAvailable` is computed (`numberOfBookings < quota`) and is not stored in Dynamics. The CRM API should either expose
  this computed field directly or leave it to the Rails API to derive.
- This endpoint calls Dynamics at request time — there is no local cache. If CRM is unreachable, the service generates a
  synthetic fallback list of open slots (09:00–17:00, 30-minute intervals, next 5 weekdays). The CRM API should define
  whether this fallback behaviour moves there or stays in Rails.
- Requires `Admin`, `GetAnAdviser`, or `GetIntoTeaching` role.

## **Endpoint:** `GET /api/schools_experience/candidates`

**Description:** Retrieves multiple Schools Experience candidate profiles by a comma-separated list of candidate IDs.
Calls the CRM directly at request time (not cached).

**Query parameters:**

| Parameter | Type           | Description                            |
|-----------|----------------|----------------------------------------|
| `ids`     | `Guid[]` (CSV) | Comma-separated candidate IDs to fetch |

**CRM-side mapping** (entity: `contact` + related `dfe_candidatequalification` and `dfe_contactchannelcreation`):

| Dynamics field                                                                                | Response field                                | Notes                                                     |
|-----------------------------------------------------------------------------------------------|-----------------------------------------------|-----------------------------------------------------------|
| `contact` Entity `Id`                                                                         | `candidateId` (Guid)                          |                                                           |
| `emailaddress1`                                                                               | `email` (string)                              |                                                           |
| `firstname`                                                                                   | `firstName` (string)                          |                                                           |
| `lastname`                                                                                    | `lastName` (string)                           |                                                           |
| `firstname` + `lastname` *(computed)*                                                         | `fullName` (string)                           | Concatenated — not a CRM field                            |
| `address1_line1`                                                                              | `addressLine1` (string)                       |                                                           |
| `address1_line2`                                                                              | `addressLine2` (string)                       |                                                           |
| `address1_line3`                                                                              | `addressLine3` (string)                       |                                                           |
| `address1_city`                                                                               | `addressCity` (string)                        |                                                           |
| `address1_stateorprovince`                                                                    | `addressStateOrProvince` (string)             |                                                           |
| `address1_postalcode`                                                                         | `addressPostcode` (string)                    |                                                           |
| `address1_telephone1`, falling back to `telephone1`, `mobilephone`, `telephone2`              | `telephone` (string)                          | First non-blank value, stripped of leading `00` exit code |
| `dfe_hasdbscertificate`                                                                       | `hasDbsCertificate` (bool?)                   |                                                           |
| `dfe_dateofissueofdbscertificate`                                                             | `dbsCertificateIssuedAt` (datetime?)          |                                                           |
| `dfe_preferredteachingsubject01` (EntityRef → `dfe_teachingsubjectlist`)                      | `preferredTeachingSubjectId` (Guid?)          |                                                           |
| `dfe_preferredteachingsubject02` (EntityRef → `dfe_teachingsubjectlist`)                      | `secondaryPreferredTeachingSubjectId` (Guid?) |                                                           |
| `masterid` (EntityRef → `contact`)                                                            | `masterId` (Guid?)                            | Read-only                                                 |
| `merged`                                                                                      | `merged` (bool)                               | Read-only                                                 |
| Latest `dfe_candidatequalification` (by `createdon`): Entity `Id`                             | `qualificationId` (Guid?)                     |                                                           |
| Latest `dfe_candidatequalification`: `dfe_degreesubject`                                      | `degreeSubject` (string)                      |                                                           |
| Latest `dfe_candidatequalification`: `dfe_ukdegreegrade` (OptionSet)                          | `ukDegreeGradeId` (int?)                      |                                                           |
| Latest `dfe_candidatequalification`: `dfe_degreestatus` (OptionSet)                           | `degreeStatusId` (int?)                       |                                                           |
| Latest `dfe_candidatequalification`: `dfe_type` (OptionSet)                                   | `degreeTypeId` (int?)                         |                                                           |
| Latest `dfe_contactchannelcreation` (by `createdon`): `dfe_creationchannelsource` (OptionSet) | `creationChannelSourceId` (int?)              | Read-only                                                 |
| Latest `dfe_contactchannelcreation`: `dfe_creationchannelservice` (OptionSet)                 | `creationChannelServiceId` (int?)             | Read-only                                                 |
| Latest `dfe_contactchannelcreation`: `dfe_creationchannelactivities` (OptionSet)              | `creationChannelActivityId` (int?)            | Read-only                                                 |

**Response format:**

```json
[
  {
    "candidateId": "11111111-1111-1111-1111-111111111111",
    "email": "jane.doe@example.com",
    "firstName": "Jane",
    "lastName": "Doe",
    "fullName": "Jane Doe",
    "telephone": "07700900000",
    "addressLine1": "1 Example Street",
    "addressLine2": null,
    "addressLine3": null,
    "addressCity": "London",
    "addressStateOrProvince": "Greater London",
    "addressPostcode": "TE1 1ST",
    "preferredTeachingSubjectId": "22222222-2222-2222-2222-222222222222",
    "secondaryPreferredTeachingSubjectId": null,
    "hasDbsCertificate": true,
    "dbsCertificateIssuedAt": "2023-01-01T00:00:00Z",
    "masterId": null,
    "merged": false,
    "qualificationId": "33333333-3333-3333-3333-333333333333",
    "degreeSubject": "Mathematics",
    "ukDegreeGradeId": 1,
    "degreeStatusId": 2,
    "degreeTypeId": 1,
    "creationChannelSourceId": 1,
    "creationChannelServiceId": 2,
    "creationChannelActivityId": null
  }
]
```

**Ordering:** As returned by CRM.

**Notes:**

- Calls `ICrmService.GetCandidates(ids)` at request time — live CRM call, no local cache.
- The telephone field applies the same fallback logic as the single-candidate endpoint: `AddressTelephone` with
  `StripExitCode()`, falling back through `Telephone`, `MobileTelephone`, `SecondaryTelephone` (whichever is first
  non-blank).
- Requires `Admin` or `SchoolsExperience` role.

## **Endpoint:** `GET /api/schools_experience/candidates/{id}`

**Description:** Retrieves a single Schools Experience candidate profile by ID. Calls the CRM directly at request time.

**Path parameters:**

| Parameter | Type   | Description      |
|-----------|--------|------------------|
| `id`      | `Guid` | The candidate ID |

**CRM-side mapping:** Same field mapping as `GET /api/schools_experience/candidates` above (entity: `contact` + related
`dfe_candidatequalification` and `dfe_contactchannelcreation`). Returns `404 Not Found` if no candidate exists with the
given ID.

**Response format:** Single `SchoolsExperienceSignUp` object — same shape as one item from the collection endpoint
above.

**Notes:**

- Calls `ICrmService.GetCandidate(id)` at request time — live CRM call, no local cache.
- Requires `Admin` or `SchoolsExperience` role.

## **Endpoint:** `GET /api/lookup_items/countries`

**Description:** Returns the full list of countries from the local PostgreSQL cache (synced from Dynamics `dfe_country`
entity), sorted alphabetically by name.

**CRM-side mapping:**

| Dynamics field   | Response field     | Notes                |
|------------------|--------------------|----------------------|
| Entity `Id`      | `id` (Guid)        | Dynamics record GUID |
| `dfe_name`       | `value` (string)   | Country name         |
| `dfe_countrykey` | `isoCode` (string) | ISO country code     |

**Response format:**

```json
[
  {
    "id": "72f5c2e6-74f9-e811-a97a-000d3a2760f2",
    "value": "United Kingdom",
    "isoCode": "GB"
  },
  {
    "id": "33333333-3333-3333-3333-333333333333",
    "value": "France",
    "isoCode": "FR"
  }
]
```

**Ordering:** By `value` (country name) ascending — alphabetical. Note: the store initially queries by `id`, but the
controller re-sorts by `value` before returning.

**Notes:**

- Unlike pick list items, countries are lookup entities (full records with GUIDs, not metadata option-set integers), and
  include an `isoCode` field (`dfe_countrykey`) not present in pick list responses.
- Data is served from the local PostgreSQL cache (synced periodically from Dynamics), not a live CRM call.

## **Endpoint:** `GET /api/lookup_items/degree_countries`

**Description:** Returns a filtered subset of countries — only those valid for degree country selection. Currently
hardcoded to two entries: United Kingdom and a generic "Another Country" option.

**CRM-side mapping:** Same field mapping as `/api/lookup_items/countries`.

**Response format:**

```json
[
  {
    "id": "72f5c2e6-74f9-e811-a97a-000d3a2760f2",
    "value": "United Kingdom",
    "isoCode": "GB"
  },
  {
    "id": "6f9e7b81-e44d-f011-877a-00224886d23e",
    "value": "Another Country",
    "isoCode": null
  }
]
```

**Ordering:** By `value` ascending.

**Notes:**

- **Transformation:** The store filters countries to only those whose ID is in a hardcoded allowlist (
  `Country.DegreeCountriesList = [UnitedKingdomCountryId, AnotherCountryId]`). This is not a Dynamics-side filter — it
  is applied in the application code. The CRM API should replicate or own this filtering logic.
- The two allowed GUIDs are constants in the codebase: `UnitedKingdomCountryId = 72f5c2e6-74f9-e811-a97a-000d3a2760f2`,
  `AnotherCountryId = 6f9e7b81-e44d-f011-877a-00224886d23e`.

## **Endpoint:** `GET /api/lookup_items/teaching_subjects`

**Description:** Returns the full list of teaching subjects from the local PostgreSQL cache (synced from Dynamics
`dfe_teachingsubjectlist` entity), sorted alphabetically by name.

**CRM-side mapping:**

| Dynamics field | Response field   | Notes                |
|----------------|------------------|----------------------|
| Entity `Id`    | `id` (Guid)      | Dynamics record GUID |
| `dfe_name`     | `value` (string) | Subject name         |

**Response format:**

```json
[
  {
    "id": "b02655a1-2afa-e811-a981-000d3a276620",
    "value": "Primary"
  },
  {
    "id": "44444444-4444-4444-4444-444444444444",
    "value": "Biology"
  }
]
```

**Ordering:** By `value` (subject name) ascending — alphabetical.

**Notes:**

- Like countries, teaching subjects are lookup entities (GUID + name), not pick list integers.
- The well-known "Primary" subject GUID (`b02655a1-2afa-e811-a981-000d3a276620`) is a constant used in business logic
  elsewhere (e.g. to default teaching subject when phase is Primary).
- Data is served from the local PostgreSQL cache, not a live CRM call.

## **Endpoint:** `GET /api/mailing_list/members/exchange_magic_link_token/{magicLinkToken}`

**Description:** Validates a one-time magic link token against the CRM and, if valid, returns a pre-populated
`MailingListAddMember` for the matched candidate. Also triggers a side effect: the candidate's token status is marked as
`Exchanged` in the CRM (via a background upsert job).

**Path parameters:**

| Parameter        | Type   | Description                                                                             |
|------------------|--------|-----------------------------------------------------------------------------------------|
| `magicLinkToken` | string | A 32-character hex token previously issued via `POST /api/candidates/magic_link_tokens` |

**Token resolution:**

| Step                    | What happens                                                                                                                                |
|-------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| Token lookup            | `ICrmService.MatchCandidates(token)` — queries Dynamics for a `contact` where `dfe_websitemltoken` matches the provided token               |
| Must be exactly 1 match | 0 matches → `401 Unauthorized`. More than 1 match (token collision) → `401 Unauthorized`                                                    |
| Mark token used         | Sets `dfe_websitemltokenstatus = Exchanged` on the matched candidate, then enqueues `UpsertCandidateJob` to persist this change to Dynamics |
| Response mapping        | Maps `Candidate` → `MailingListAddMember` (see table below)                                                                                 |

**CRM-side mapping** (entity: `contact` + related `dfe_candidatequalification`):

| Dynamics field                                                                                          | Response field                                     | Notes                        |
|---------------------------------------------------------------------------------------------------------|----------------------------------------------------|------------------------------|
| `contact` Entity `Id`                                                                                   | `candidateId` (Guid?)                              |                              |
| `emailaddress1`                                                                                         | `email` (string)                                   |                              |
| `firstname`                                                                                             | `firstName` (string)                               |                              |
| `lastname`                                                                                              | `lastName` (string)                                |                              |
| `address1_postalcode`                                                                                   | `addressPostcode` (string)                         |                              |
| `dfe_preferredteachingsubject01` (EntityRef → `dfe_teachingsubjectlist`)                                | `preferredTeachingSubjectId` (Guid?)               |                              |
| `dfe_websitewhereinconsiderationjourney` (OptionSet)                                                    | `considerationJourneyStageId` (int?)               |                              |
| `dfe_welcomeguidestring`                                                                                | `welcomeGuideVariant` (string)                     |                              |
| `dfe_situation` (OptionSet)                                                                             | `situation` (int?)                                 |                              |
| Latest `dfe_candidatequalification` (by `createdon`): Entity `Id`                                       | `qualificationId` (Guid?)                          |                              |
| Latest `dfe_candidatequalification`: `dfe_degreestatus` (OptionSet)                                     | `degreeStatusId` (int?)                            |                              |
| `dfe_gitismlserviceissubscriber` (bool)                                                                 | `alreadySubscribedToMailingList` (bool)            | Direct field read            |
| `dfe_gitiseventsserviceissubscriber` (bool)                                                             | `alreadySubscribedToEvents` (bool)                 | Direct field read            |
| `dfe_gitisttaserviceissubscriber == true` OR `owningbusinessunit == AdviserBusinessUnitId` *(computed)* | `alreadySubscribedToTeacherTrainingAdviser` (bool) | Derived — not a single field |

**Response format (success):**

```json
{
  "candidateId": "11111111-1111-1111-1111-111111111111",
  "email": "jane.doe@example.com",
  "firstName": "Jane",
  "lastName": "Doe",
  "addressPostcode": "TE1 1ST",
  "preferredTeachingSubjectId": "22222222-2222-2222-2222-222222222222",
  "considerationJourneyStageId": 1,
  "welcomeGuideVariant": "A",
  "situation": 2,
  "qualificationId": "33333333-3333-3333-3333-333333333333",
  "degreeStatusId": 2,
  "alreadySubscribedToMailingList": false,
  "alreadySubscribedToEvents": false,
  "alreadySubscribedToTeacherTrainingAdviser": false
}
```

**Response (failure):** `401 Unauthorized` with a `CandidateMagicLinkExchangeResult` body indicating failure reason.

**Notes:**

- **Transformation:** `alreadySubscribedToMailingList`, `alreadySubscribedToEvents`,
  `alreadySubscribedToTeacherTrainingAdviser` are computed booleans derived from the candidate's current subscription
  state in the CRM — they are not direct field reads.
- **Side effect:** This GET has a write side effect — the token is invalidated (marked `Exchanged`) and a background
  upsert is enqueued. This is architecturally unusual for a GET endpoint.
- Magic link tokens are 48-hour one-time tokens (128-bit random hex), generated separately via
  `POST /api/candidates/magic_link_tokens`.
- Requires `Admin` or `GetIntoTeaching` role.

## **Endpoint:** `GET /api/operations/generate_mapping_info`

**Description:** Returns introspection data describing how each C# model class maps to a Dynamics 365 entity and its
fields. Intended as a developer/debugging aid — not a CRM data read.

**CRM-side mapping:** None. This endpoint uses .NET reflection to enumerate all classes that extend `BaseModel` and
reads their `[Entity]`, `[EntityField]`, and `[EntityRelationship]` attributes.

**Response format:**

```json
[
  {
    "class": "GetIntoTeachingApi.Models.Crm.Candidate",
    "logicalName": "contact",
    "fields": {
      "Email": {
        "name": "emailaddress1",
        "type": "System.String",
        "reference": null
      }
    },
    "relationships": {
      "Qualifications": {
        "name": "dfe_contact_dfe_candidatequalification_ContactId",
        "type": "GetIntoTeachingApi.Models.Crm.CandidateQualification"
      }
    }
  }
]
```

**Notes:**

- No CRM call is made — this is pure reflection over the assembled code. No authentication is required (no `[Authorize]`
  attribute on this action).
- This endpoint is internal tooling. It is unlikely to have a direct equivalent in the new CRM API, but may be useful as
  a reference for the CRM API team to verify their own field mappings.

## **Endpoint:** `GET /api/operations/health_check`

**Description:** Returns the operational status of all dependent services: PostgreSQL database, Hangfire job queue,
Dynamics CRM, GOV.UK Notify, and Redis.

**CRM-side mapping:** None — the CRM is pinged via `ICrmService.CheckStatus()` which performs a lightweight connectivity
check. The result is a plain string (`"ok"` or an error description).

**Response format:**

```json
{
  "gitCommitSha": "abc1234",
  "environment": "production",
  "database": "ok",
  "hangfire": "ok",
  "crm": "ok",
  "notify": "ok",
  "redis": "ok",
  "status": "healthy"
}
```

**Notes:**

- **Transformation:** The `status` field is computed from the individual service statuses: `"healthy"` if all services
  return `"ok"`, `"degraded"` if only non-critical services (`crm`, `notify`, `redis`) are unhealthy while critical
  services (`database`, `hangfire`) are ok, `"unhealthy"` if any critical service is down.
- No `[Authorize]` attribute — this endpoint is publicly accessible.
- This endpoint is internal infrastructure tooling and will likely remain owned by the Rails API rather than delegating
  to the CRM API.

## **Endpoint:** `GET /api/privacy_policies/latest`

**Description:** Returns the most recently created privacy policy from the local PostgreSQL cache.

**CRM-side mapping:**

| Dynamics field | Response field         | Notes                |
|----------------|------------------------|----------------------|
| Entity `Id`    | `id` (Guid)            | Dynamics record GUID |
| `dfe_details`  | `text` (string)        | Full policy text     |
| `createdon`    | `createdAt` (datetime) | Creation timestamp   |

**Response format:**

```json
{
  "id": "55555555-5555-5555-5555-555555555555",
  "text": "We collect your personal data...",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

**Notes:**

- **Transformation:** "Latest" is determined by sorting on `createdAt` descending and taking the first record — this
  ordering happens in the application, not in Dynamics.
- Data is served from the local PostgreSQL cache (`dfe_privacypolicy` entity synced periodically). The cache only stores
  `Type.Web` policies (type code `222750001`).
- Requires any authenticated role.

## **Endpoint:** `GET /api/privacy_policies/{id}`

**Description:** Returns a specific privacy policy by its Dynamics record GUID.

**Path parameters:**

| Parameter | Type   | Description             |
|-----------|--------|-------------------------|
| `id`      | `Guid` | The privacy policy GUID |

**CRM-side mapping:** Same field mapping as `GET /api/privacy_policies/latest`. Returns `404 Not Found` if no policy
with the given ID exists in the local cache.

**Response format:** Single `PrivacyPolicy` object (same shape as `/latest`).

**Notes:**

- Data served from local PostgreSQL cache — not a live CRM call.
- Requires any authenticated role.

## **Endpoint:** `GET /api/teaching_event_buildings`

**Description:** Returns all teaching event venue buildings from the local PostgreSQL cache.

**CRM-side mapping:**

| Dynamics field                     | Response field             | Notes                                                                  |
|------------------------------------|----------------------------|------------------------------------------------------------------------|
| Entity `Id`                        | `id` (Guid)                | Dynamics record GUID                                                   |
| `msevtmgt_name`                    | `venue` (string)           | Venue/building name                                                    |
| `msevtmgt_addressline1`            | `addressLine1` (string)    |                                                                        |
| `msevtmgt_addressline2`            | `addressLine2` (string)    |                                                                        |
| `msevtmgt_addressline3`            | `addressLine3` (string)    |                                                                        |
| `msevtmgt_city`                    | `addressCity` (string)     |                                                                        |
| `msevtmgt_postalcode`              | `addressPostcode` (string) |                                                                        |
| `dfe_eventvenueimageurl`           | `imageUrl` (string)        |                                                                        |
| `Coordinate` (PostGIS `geography`) | *(not exposed)*            | Stored in PostgreSQL for geo search but `[JsonIgnore]`-d from response |

**Response format:**

```json
[
  {
    "id": "66666666-6666-6666-6666-666666666666",
    "venue": "Manchester Central",
    "addressLine1": "Windmill Street",
    "addressLine2": null,
    "addressLine3": null,
    "addressCity": "Manchester",
    "addressPostcode": "M2 3GX",
    "imageUrl": "https://example.com/venue.jpg"
  }
]
```

**Ordering:** As stored in the local cache (no ordering applied in code).

**Notes:**

- **Transformation:** The `Coordinate` (PostGIS geography point) is stored in PostgreSQL and used for proximity
  searches (see `teaching_events/search`) but is excluded from the API response via `[JsonIgnore]`.
- Data served from local PostgreSQL cache — not a live CRM call.
- Requires `Admin` or `GetIntoTeaching` role.

## **Endpoint:** `GET /api/teaching_events/search`

**Description:** Searches the local PostgreSQL cache for teaching events matching the given filters. Optionally limits
results by proximity (radius in miles from a postcode).

**Query parameters:**

| Parameter              | Type        | Default                  | Description                                    |
|------------------------|-------------|--------------------------|------------------------------------------------|
| `postcode`             | string      | —                        | Centre of radius search                        |
| `radius`               | int         | —                        | Radius in miles (requires `postcode`)          |
| `typeIds`              | int[] (CSV) | —                        | Filter by event type pick list IDs             |
| `statusIds`            | int[] (CSV) | `[222750000, 222750001]` | Filter by status (defaults to Open + Closed)   |
| `online`               | bool        | —                        | `true` = online only, `false` = in-person only |
| `startAfter`           | datetime    | —                        | Exclude events starting on or before this date |
| `startBefore`          | datetime    | —                        | Exclude events starting on or after this date  |
| `accessibilityOptions` | int[] (CSV) | —                        | Filter by accessibility option IDs             |
| `quantity`             | int         | 10                       | Maximum number of results to return            |

**CRM-side mapping:** Events are stored in the local cache from Dynamics `msevtmgt_event` entity (see `TeachingEvent`
field mapping below).

| Dynamics field                                              | Response field                    | Notes                                                                                         |
|-------------------------------------------------------------|-----------------------------------|-----------------------------------------------------------------------------------------------|
| Entity `Id`                                                 | `id` (Guid)                       |                                                                                               |
| `dfe_event_type` (OptionSet)                                | `typeId` (int)                    |                                                                                               |
| `dfe_eventstatus` (OptionSet)                               | `statusId` (int)                  |                                                                                               |
| `dfe_eventregion` (OptionSet)                               | `regionId` (int?)                 |                                                                                               |
| `dfe_websiteeventpartialurl`                                | `readableId` (string)             | Human-readable URL slug                                                                       |
| `dfe_eventwebfeedid`                                        | `webFeedId` (string)              | If set, API accepts attendees directly; if null, use external sign-up                         |
| `dfe_isonlineevent`                                         | `isOnline` (bool)                 |                                                                                               |
| `dfe_externaleventtitle`                                    | `name` (string)                   | Setting `name` also writes `msevtmgt_name` (internal name) but that field is `[JsonIgnore]`-d |
| `dfe_eventsummary_ml`                                       | `summary` (string)                |                                                                                               |
| `dfe_miscellaneousmessage_ml`                               | `message` (string)                |                                                                                               |
| `msevtmgt_description`                                      | `description` (string)            |                                                                                               |
| `dfe_videolink`                                             | `videoUrl` (string)               |                                                                                               |
| `dfe_scribbleurl`                                           | `scribbleId` (string)             |                                                                                               |
| `dfe_providerwebsite`                                       | `providerWebsiteUrl` (string)     |                                                                                               |
| `dfe_providertargetaudience_ml`                             | `providerTargetAudience` (string) |                                                                                               |
| `dfe_providerorganiser`                                     | `providerOrganiser` (string)      |                                                                                               |
| `dfe_providercontactemailaddress`                           | `providerContactEmail` (string)   |                                                                                               |
| `msevtmgt_eventstartdate`                                   | `startAt` (datetime)              |                                                                                               |
| `msevtmgt_eventenddate`                                     | `endAt` (datetime)                |                                                                                               |
| `dfe_providerslist`                                         | `providersList` (string)          |                                                                                               |
| `dfe_accessibility` (multi-select OptionSetValueCollection) | `accessibilityOptions` (int[])    | Stored as CSV string in PostgreSQL, exposed as int array. Parsed/serialised by the model.     |
| `msevtmgt_building` (EntityRef)                             | `building` (object)               | Nested `TeachingEventBuilding` (see above)                                                    |
| `msevtmgt_eventtimezone`                                    | *(not exposed)*                   | Always GMT (code `85`), `[JsonIgnore]`-d                                                      |
| `msevtmgt_building` (GUID)                                  | *(not exposed)*                   | `buildingId` is `[JsonIgnore]`-d — building returned as nested object instead                 |
| *(computed)*                                                | `isVirtual` (bool)                | `isOnline && building.addressPostcode != null`                                                |
| *(computed)*                                                | `isInPerson` (bool)               | `!isOnline \|\| isVirtual` — department considers virtual events "in-person"                  |

**Response format:**

```json
[
  {
    "id": "77777777-7777-7777-7777-777777777777",
    "typeId": 222750001,
    "statusId": 222750000,
    "regionId": 1,
    "readableId": "train-to-teach-manchester-2026",
    "webFeedId": "abc123",
    "isOnline": false,
    "name": "Train to Teach — Manchester",
    "summary": "Come along to find out about teacher training.",
    "message": null,
    "description": "...",
    "startAt": "2026-09-15T18:00:00Z",
    "endAt": "2026-09-15T20:00:00Z",
    "accessibilityOptions": [
      1,
      3
    ],
    "isVirtual": false,
    "isInPerson": true,
    "building": {
      "id": "66666666-6666-6666-6666-666666666666",
      "venue": "Manchester Central",
      "addressPostcode": "M2 3GX"
    }
  }
]
```

**Ordering:** By `startAt` ascending, then limited to `quantity` results.

**Notes:**

- **Transformations:**
    - `accessibilityOptions` (int[]) is stored as a comma-separated string in PostgreSQL (`AccessibilityOptionId`) and
      parsed into an integer array on read. The Dynamics source is a multi-select `OptionSetValueCollection`.
    - `isVirtual` and `isInPerson` are computed booleans — not stored in Dynamics or PostgreSQL.
    - `name` (from `dfe_externaleventtitle`) is mirrored to `InternalName` (`msevtmgt_name`) on write, but`InternalName`
      is `[JsonIgnore]`-d from the response.
    - Radius search: converts `radius` (miles) to km (`* 1.60934`) and uses PostGIS geography queries against the
      building's `Coordinate` column.
    - `statusIds` defaults to `[Open, Closed]` (not Draft or Pending) if not provided.
- Data served from local PostgreSQL cache — not a live CRM call.
- Requires `Admin` or `GetIntoTeaching` role.

## **Endpoint:** `GET /api/teaching_events/{readableId}`

**Description:** Returns a single teaching event by its human-readable URL slug (`readableId`).

**Path parameters:**

| Parameter    | Type   | Description                                   |
|--------------|--------|-----------------------------------------------|
| `readableId` | string | The event's `dfe_websiteeventpartialurl` slug |

**CRM-side mapping:** Same field mapping as the search endpoint above. Returns `404 Not Found` if no event with the
given `readableId` exists in the local cache.

**Response format:** Single `TeachingEvent` object including its nested `Building` (same shape as a single item from the
search response).

**Notes:**

- Data served from local PostgreSQL cache — not a live CRM call.
- The `Building` is always eagerly loaded (joined) in this query.
- Requires `Admin` or `GetIntoTeaching` role.
