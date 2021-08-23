# Get into Teaching API 
[![Build and Deploy](https://github.com/DFE-Digital/get-into-teaching-api/actions/workflows/devops.yml/badge.svg)](https://github.com/DFE-Digital/get-into-teaching-api/actions/workflows/devops.yml)

> Provides a RESTful API for integrating with the Get into Teaching CRM.

The Get into Teaching (GIT) API sits in front of the GIT CRM, which uses the [Microsoft Dynamics365](https://docs.microsoft.com/en-us/dynamics365/) platform (the [Customer Engagement](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/overview) module is used for storing Candidate information and the [Marketing](https://docs.microsoft.com/en-us/dynamics365/marketing/developer/using-events-api) module for managing Events).

The GIT API aims to provide:

- Simple, task-based RESTful APIs.
- Message queueing (while the GIT CRM is offline for updates).
- Validation to ensure consistency across services writing to the GIT CRM.

## API Clients

### Adding a New Client

The API can be integrated into a new service by creating a 'client' in `Fixtures/clients.yml`:

```
name: My Service
description: "An example service description"
api_key_prefix: MY_SERVICE
role: MyService
```

### Setting API Keys

Once a client has been added, you will need to ensure that the relevant variables containing the API keys are defined in Azure for each environment. In the above case this would be:

```
MY_SERVICE_API_KEY=<a long secure value>
```

It is important that you use a unique value for the service API key as the client/role is derived from this; if the API keys are not unique your service may not have access to the endpoints you expect.

To make development easier you can define a short, memerable API key in `GetIntoTeachingApi/Properties/launchSettings.json` and commit it. **Do not commit keys for non-development environments!**.

### Authorizing Roles

The role associated with the client should be added to any controller/actions that the service requires access to:

```
[Authorize(Roles = "ServiceRole")]
public class CandidatesController : ControllerBase
{
    ...
}
```

If a controller/action is decorated with an `[Authorize]` attribute without a role specified it will be accessible to all clients.

Multiple clients can utilise the same role, if applicable.

### Rate Limiting

Endpoints that are potentially exploitable have global rate limits applied. If you need more than the base-line quota (or less) for a service you can override them by editing the `ClientRules` in `GetIntoTeachingApi/appsettings.json`:

```
"ClientRules": [{
    "ClientId": "<client_api_key_prefix>",
    "Rules": [
        {
            "Endpoint": "POST:/api/candidates/access_tokens",
            "Period": "1m",
            "Limit": 500
        }
    ]
}]
```

More information on rate limiting can be found [below](#rate-limiting).


## Getting Started

The API is an ASP.NET Core web application; to get up and running clone the repository and open `GetIntoTeachingApi.sln` in Visual Studio.

You will need to set up the environment (see the `Environment` section below) before booting up the dependent services in Docker with `docker-compose up`.

When the application runs in development it will open the Swagger documentation by default (the development shared secret for the admin client is `secret-admin`).

On the first run it will do a long sync of UK postcode information into the Postgres instance running in Docker - you can monitor the progress via the Hangfire dashboard (subsequent start ups should be quicker).

### Environment

If you want to run the API locally end-to-end you will need to set some environment variables (you can specify these in `.env.development`):

```
# CRM credentials
CRM_SERVICE_URL=****
CRM_CLIENT_ID=****
CRM_CLIENT_SECRET=****
CRM_TENANT_ID=****

# GOV.UK Notify Service credentials
NOTIFY_API_KEY=****
```

A number of non-secret, default development environment variables are pre-set in `GetIntoTeachingApi/Properties/launchSettings.json` (such as a development `ADMIN_API_KEY` of `admin-secret` and the `VCAP_SERVICES` setup for the Postgres instance running in Docker).

Other environment variables are available (see the `IEnv` interface) but not necessary to run the bare-bones application.

The Postgres connections (for Hangfire and our database) are setup dynamically from the `VCAP_SERVICES` environment variable provided by GOV.UK PaaS. If you want to connect to a Postgres instance running in PaaS instead of the one in Docker - such as the test environment instance - you can do so by creating a conduit to it using Cloud Foundry:

```
cf conduit get-into-teaching-api-dev-pg-svc
```

You then need to update the `VCAP_SERVICES` environment variable (in `launchSettings.json`) to reflect the connection details for your conduit session:

```
{\"postgres\": [{\"instance_name\": \"rdsbroker_277c8858_eb3a_427b_99ed_0f4f4171701e\",\"credentials\": {\"host\": \"127.0.0.1\",\"name\": \"rdsbroker_277c8858_eb3a_427b_99ed_0f4f4171701e\",\"username\": \"******\",\"password\": \"******\",\"port\": \"7080\"}}]}

```

### Documentation

[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) is used for generating Swagger documentation. We use the swagger-ui middleware to expose interactive documentation when the application runs.

We also use the [MicroElements.Swashbuckle.FluentValidation](https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation) package to make Swashbuckle aware of the FluentValidation classes, so that additional validation meta data gets displayed alongside model attributes.

You can hit the API endpoints directly from the Swagger UI - hit the `Authorize` button at the top and enter the development `Authorization` header value `Bearer <shared_secret>`. You can then open up an endpoint and 'Try it out' to execute a request.

### Validation

[FluentValidation](https://fluentvalidation.net/) is used for validating the various models. The validators are registered at startup, so validating incoming payloads that have a corresponding validator is as simple as calling `ModelState.IsValid`.

The majority of the validation should be performed against the core models linked to Dynamics entities (any model that inherits from `BaseModel`). The validation in these models should make sure that the data is correct, but remain loose around which fields are required; often a model will be reused in different contexts and the required fields will change. `Candidate` is a good example of this; the request models `MailingListAddMember`, `TeacherTrainingAdviserSignUp` and `TeachingEventAddAttendee` all map onto `Candidate`, however the required fields are different for each.

We also call registered validators for subclasses of `BaseModel` when mapping CRM entities into our API models. If the CRM returns an invalid value according to our validation logic it will be nullified. An example of where this can happen is with postcodes; if the CRM returns an invalid postcode we will nullify it (otherwise the client may re-post the invalid postcode back to the API without checking it and receive a 400 bad request response unexpectedly).

Property names in request models should be consistent with any hidden `BaseModel` models they encapsulate and map to. When consistent, we can intercept validation results in order surface these back to the original request model. For example, the `MailingListAddMember.UkDegreeGradeId` is consistent with `MailingListAddMember.Candidate.Qualifications[0].UkDegreeGradeId`. Any errors that appear on the `MailingListAddMember.Candidate.Qualifications[0].UkDegreeGradeId` property will be intercepted in the `MailingListAddMemberValidator` mapped back to `MailingListAddMember.UkDegreeGradeId`.

### Testing

[XUnit](https://xunit.net/) is used for testing; all tests can be found in the `GetIntoTeachingTests` project. We also use [Moq](https://github.com/Moq/moq4/wiki/Quickstart) for mocking any dependencies and [FluentAssertions](https://fluentassertions.com) for assertions.

The unit tests take the convention:

```
public void UnitOfWork_StateUnderTest_ExpectedBehavior()
{
    // arrange

    // act

    // assert
}
```

#### Contract Testing

> :warning: **Development of the contract tests is currently in-progress**

We use a variation of 'contract testing' to achieve end-to-end integration tests across all services (API Clients -> API -> CRM).

The API consumes the output of API client service contract tests, which are snapshots of the calls made to the API during test scenarios. Off the back of these requests the API makes calls to the CRM, which are saved as output snapshots of the API contract tests (later consumed by the CRM contract tests).

If a change to the API codebase results in a different call/payload sent to the CRM, then the snapshots will not match and the test will fail. If the change is expected the snapshot should be replaced with the updated version and committed.

A difficulty with these tests is ensuring that the services all have a consistent, global view of the service data state prior to executing the contract tests. We currently maintain the service data in `Contracts/Data` (to be centralised in a GitHub repospitory, but currently duplicated in each service).

Eventually, the `Contracts/Output` will be 'published' to a separate GitHub repository, which will enable other services to pull in their test fixtures from the upstream service. This will enable us to develop features in each service independently and publish only when the change is ready to be reflected in another service.

### Emails

We send emails using the [GOV.UK Notify](https://www.notifications.service.gov.uk/) service; leveraging the [.Net Client](https://github.com/alphagov/notifications-net-client).

### Background Jobs

[Hangfire](https://www.hangfire.io/) is used for queueing and processing background jobs; an in-memory storage is used for development and PostgreSQL is used in production (the PRO version is required to use Redis as the storage provider). Failed jobs get retries on a 60 minute interval a maximum of 24 times before they are deleted (in development this is reduced to 1 minute interval a maximum of 5 times) - if this happens we attempt to inform the user by sending them an email.

The Hangfire web dashboard can be accessed at `/hangfire` in development.

### Database

We run Entity Framework Core in order to persist models/data to a Postgres database.

Migrations are applied from code when the application starts (see `DbConfiguration.cs`). You can add a migration by modifying the models and running `Add-Migration MyNewMigration` from the package manager console.

If you need to apply/rollback a migration manually in production, the process is not as straight-forward due to `dotnet ef` CLI requiring the application source code to run (in production we only deploy the compiled DLL). Instead, you need to:

- If you are rolling back, ensure your local development database matches production _before_ proceeding
- Generate the SQL for the migration manually, which can be done with `dotnet ef migrations script <from> <to>`
    - To generate SQL that rolls back you would put `<from>` as the _current_ latest migration and `<to>` as the migration you want to rollback to
    - Example: `dotnet ef migrations script DropTypeEntity AddPickListItem`
- [Create a conduit](https://docs.cloud.service.gov.uk/deploying_services/postgresql/#connect-to-a-postgresql-service-from-your-local-machine) into the production database (currently only DevOps have permission to do this).
    - `cf conduit <database>`
- If you are rolling back, ensure that you have reverted the code change that introduced the migration _before_ proceeding
- Execute the SQL

### Rate Limiting

We use [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) to rate limit clients based on their access token (the value passed in the `Authorization` header). Currently both our clients share the same access token, but we envisage splitting that up in the future.

It is the responsibility of the API client to rate limit on a per-user basis to ensure the global rate limiting of their access token is not exceeded.

We apply the same rate limits irrespective of client at the moment, but going forward we could offer per-client rate limiting using the library.

The rate limit counters are currently stored in memory, but we will change this going forward to use Redis so that they are shared between instances.

### Deployment

Deployment is via Terraform and the key will be stored in Azure.

### Logs

Logs are available by loggging into [logit.io](https://logit.io).

### Metrics

Metrics are exposed to Prometheus on the `/metrics` endpoint; [prometheus-net](https://github.com/prometheus-net/prometheus-net) is used for collecting and exposing the metrics.

Prometheous and Grafana have been added to gather and display Metric information

### Error Monitoring

[Sentry](https://sentry.io) is used for error monitoring.
[Logit](https://logit.io) is used to capture log files

### HTTP Caching

The content that we cache in Postgres that originates from the CRM is served with HTTP ETag cache headers. There is a `CrmETag` annotation that can be added to an action to apply the appropriate cache headers to the response and check the request for the `If-None-Match` header. The ETag value changes when we sync new content from the CRM, meaning if the `If-None-Match` header matches the current `ETag` value we don't even need to fulfill the request, instead returning `304 Not Modified' immediately.

### Postcode Geocoding

We use postcodes to support searching for events around a given location. The application pulls a [free CSV data set](https://www.freemaptools.com/download-uk-postcode-lat-lng.htm) weekly and caches it in Postgres; this makes geocoding 99% of postcodes very fast. For the other 1% of postcodes (recent housing developments, for example) we fallback to Google APIs for geocoding.

### JSON Serializers

The application uses two Json serializers; `System.Text.Json` for everything apart from serializing Hangfire payloads (changed tracked objects) and redacting personally identifiable information from our logs, for which we use `Newtonsoft.Json`. The reasons for this are:

1. `System.Text.Json` does not support conditional serialization and we want to serialize `ChangedPropertyNames` _only_ when serializing a Hangfire payload (omitting the attribute in API responses).
2. `System.Text.Json` supports on deserializing/ed but not our particular use case of pausing change tracking during deserialization. `Newtonsoft.Json` supports `System.Runtime.Serialization.OnDeserializing/OnDeserialized` which can be used for this purpose.
3. `System.Text.Json` does not support JSONPath, which we use to redact PII from logged payloads.

In an attempt to isolate `Newtonsoft.Json` there are extensions for serializing/deserializing changed tracked objects:

```
var myChangeTrackedObject = json.DeserializeChangeTracked<ChangeTrackedObject>();
var json = myChangeTrackedObject.SerializeChangeTracked();
```

### Feature Switches

We have basic support for feature switching in the API. The `IEnv` interface provides methods to check if a feature is on or off:

```
bool IsFeatureOn(string feature);
bool IsFeatureOff(string feature);
```

It expects features to be present in the environment variables in the format `<feature_name>_FEATURE`. For example, with the environment variable:

```
APPLY_API_FEATURE=on
```

We can check for the feature with `env.IsFeatureOn("APPLY_API")`.

## CRM Changes

The application is designed to make supporting new attribtues and entities in the CRM as easy as possible. All of the 'heavy lifting' is done in the `BaseModel` class using reflection to inspect model attributes and relevant `Entity*` annotations.

### Supporting a New Entity

If there is a new entity in the CRM that you want to support you need to create a corresponding model that inherits from `BaseModel`. It should have a class annotation that contains the entity `LogicalName` so that we can associate this model with a certain entity type:

```
[Entity("phonecall")]
```

It's also important that the model has an empty constructor and a constructor that accepts an instance of `Entity` and `CrmService`. These are required by the mapping logic in `BaseModel` - a test will fail if you forget to add these in:

```
public PhoneCall(): base() { }
public PhoneCall(Entity entity, ICrmService crm): base(entity, crm) { }
```

### Adding Attributes

To support mapping to/from CRM entity attributes to properties of your model you need to add a property with the `EntityField` annotation, which can be configured to support three different types of fields that the CRM uses.

#### Primitive Fields

You can support most primitive types using just an `EntityField` annotation and the corresponding attribute name:

```
[EntityField("phonenumber")]
public string Telephone { get; set; }
```

This example maps the `phonenumber` attribute from this entity in the CRM to the `Telephone` property of the model.

#### OptionSet/PickList Fields

An `OptionSet` is a CRM type that is similar in behavior to an `enum` - a set list of values for a field. We model these by storing the `id` of the associated `OptionSetValue` in the model:

```
[EntityField("dfe_channelcreation", typeof(OptionSetValue))]
public int? ChannelId { get; set; }
```

This example maps the `dfe_channelcreation` of the entity in the CRM to the `ChannelId` property of the model. We also specify the type of the field as `OptionSetValue`.

> **If you add a new `OptionSet` type you should ensure that it is synced to the `Store` and exposed to clients via the `TypesController`.**

#### EntityReference Fields

An `EntityReference` is how the CRM establishes a reference to another entity from an attribute - essentially a foreign key if this were a database:

```
[EntityField("dfe_preferredteachingsubject01", typeof(EntityReference), "dfe_teachingsubjectlist")]
public Guid? PreferredTeachingSubjectId { get; set; }
```

This example creates a reference to the preferred teaching subject, which is another entity of type `dfe_teachingsubjectlist`.

> **If you add a new `EntityReference` type you should ensure that it is synced to the `Store` and exposed to clients via the `TypesController`.**

#### Ignoring Fields by Environment

If a new field is not yet available in one of the CRM environments you can still develop and deploy code against the attribute as long as you mark it as ignored for any environments that it is not available in. For example, to map a field in dev but _not_ test (staging) or production:

```
[EntityField("dfe_brand_new_field", null, null, new string[] { "Staging", "Production" })]
public string NewField { get; set; }
```

#### Hidden Fields

Some fields are set by the API rather than passed in as part of a client request. An example is the `Telephone` property of `PhoneCall`, which is set to whatever value is within the `Candidate` `Telephone` property. In this case we don't want to expose the property details externally, so they should be tagged with the `JsonIgnore` annotation:

```
[JsonIgnore]
[EntityField("phonenumber")]
public string Telephone { get; set; }
```

### Adding Relationships

Relationships (unlike `EntityReference` fields) contain the attributes of the related entity or entities. 

They can be established using the `EntityRelationship` annotation, passing in the CRM relationship name and the type of the model we map that related entity to:

```
[EntityRelationship("dfe_contact_phonecall_contactid", typeof(PhoneCall))]
public PhoneCall PhoneCall { get; set; }
```

Supporting to-many relationships is as simple as making sure the property is a `List` type:

```
[EntityRelationship("dfe_contact_dfe_candidatequalification_ContactId", typeof(CandidateQualification))]
public List<CandidateQualification> Qualifications { get; set; }
```

> **Currently, the `BaseModel` does not create links between related entities when mapping to an `Entity` type. Instead, the relationship must be defined by assigning a value to the foreign key property, and saving each model independently. See `CandidateUpserter`**

### Customising the Mapping Behaviour

Occasionally it can be useful to hook into the mapping behaviour that is encapsulated in the `BaseModel`. An example of this may be to prevent mapping to an entity if we know that it will create a duplicate in the CRM. We do this as part of `TeachingEventRegistration` to ensure we don't register the same candidate as an attendee of the event more than once:

```
protected override bool ShouldMap(ICrmService crm)
{
    return crm.CandidateYetToRegisterForTeachingEvent(CandidateId, EventId);
}
```
