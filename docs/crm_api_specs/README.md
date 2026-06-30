## Please read this first.

- The documents that are here are just a ruby developer's interpretation of what is happening in this API app. Mistakes or injuries are almost guaranteed. So please check existing code and swagger doc for reference. https://getintoteachingapi-test.test.teacherservices.cloud/swagger/index.html
- If some documents format a 200 response differently then what the actual CRM api returns. It means we made a mistake. Generally whatever the endpoint returns now, we want it to still return after the refactor. Unless we specify in proposed changes
- At the end of some files, there are proposed changes which we would like to be implemented if it's possible. This can be discussed further.

## API versioning
- API versioning doesn't need to be implemented now. But if there will be a need for it in the future. The ruby client that will call the new C# API will pass a version number in the headers of the request. We don't really want the version of the API to be in the URL.

api version header `"API-Version" => "1.0"`

## Error messages
- Can we please have consistent error messages? We don't really mind what status code is returned but we would really appreciate if all the error messages, 404, 400, 403. Whatever status code we returned to be in this format? Currently the format is not very consistent.
- Attribute can be optional. Some error messages don't need an attribute. E.g "Backfill is in progress"

### `400 Bad Request` — validation failed. New proposed error format
```json
{
    "errors": [
        {
            "error": "BadRequest",
            "message": "Email must not be empty",
            "attribute": "Email",
        }
    ]
}
```
