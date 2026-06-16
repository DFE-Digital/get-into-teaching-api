# `GET /api/privacy_policies`

**File:** `Controllers/PrivacyPoliciesController.cs`

Two read-only endpoints that return privacy policies from the local PostgreSQL database (synced from CRM by a background job, not queried live).

---

## `GET /api/privacy_policies/latest`

Returns the most recently created privacy policy (ordered by `CreatedAt` desc).

**Params:** None

**Response:** `200 OK`

```json
{
  "id": "3a7e8f9c-...",
  "text": "We collect and process your personal data...",
  "createdAt": "2025-01-15T10:30:00Z"
}
```

---

## `GET /api/privacy_policies/{id}`

Returns a specific privacy policy by GUID.

**Params:**

| Param | Location | Type | Required |
|-------|----------|------|----------|
| `id` | URL path | `Guid` | Yes |

**Response:**

**Response:** `200 OK`

```json
{
  "id": "3a7e8f9c-...",
  "text": "We collect and process your personal data...",
  "createdAt": "2025-01-15T10:30:00Z"
}
```

**Response:** `404 NotFound`

```json
{
    "errors": [
        {
            "error": "NotFound",
            "message": "Privacy policy with #{id} not found"
        }
    ]
}
```

---

## Notes

- Other errors can be returned, like 401 when checking authentication. But please use the same error format
- Both endpoints return privacy policies from the local PostgreSQL database (synced from CRM by a background job, not queried live).
