# Get into Teaching API

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>

[![Build](https://github.com/DFE-Digital/get-into-teaching-api/actions/workflows/build.yml/badge.svg)](https://github.com/DFE-Digital/get-into-teaching-api/actions/workflows/build.yml)

> Provides a RESTful API for integrating with the Get into Teaching CRM.

* [API Clients](#api-clients)
* [Getting Started](#getting-started)
* [Useful Links](#useful-links)
* [Deployment](#deployment)
* [Technical Details](#technical-details)
* [Monitoring](#monitoring)
* [CRM Integration](#crm-integration)

The Get into Teaching (GIT) API sits in front of the GIT CRM, which uses the [Microsoft Dynamics365](https://docs.microsoft.com/en-us/dynamics365/) platform (the [Customer Engagement](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/overview) module is used for storing Candidate information and the [Marketing](https://docs.microsoft.com/en-us/dynamics365/marketing/developer/using-events-api) module for managing Events).

The GIT API aims to provide:

- Simple, task-based RESTful APIs.
- Message queueing (while the GIT CRM is offline for updates).
- Validation to ensure consistency across services writing to the GIT CRM.

## API Clients

### Adding a New Client

The API can be integrated into a new service by creating a 'client' in `Config/clients.yml`:

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

To make development easier you can define a short, memorable API key in `GetIntoTeachingApi/Properties/launchSettings.json` and commit it. **Do not commit keys for non-development environments!**.

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

### Client Configuration

When the client is configured to point to the API, we need to ensure it uses the `https` version and does not rely on the insecure redirect of `http` traffic. We are looking into ways to disable `http` traffic entirely in the API, however it doesn't appear to be trivial to do and whilst we only have internal API clients that we have full control over we should instead be enforcing the policy of directly accessing the `https` version of the API.

## Getting Started

The API is an ASP.NET Core web application; to get up and running clone the repository and open `GetIntoTeachingApi.sln` in Visual Studio.

You will need to set up the environment before booting up the dependent services in Docker with `docker-compose up`.

When the application runs in development it will open the Swagger documentation by default (the development shared secret for the admin client is `secret-admin`).

On the first run it will do a long sync of UK postcode information into the Postgres instance running in Docker - you can monitor the progress via the Hangfire dashboard accessible at `/hangfire` (subsequent start ups should be quicker).

Quick start steps:

- `az login`
- `make setup-local-env`
- Set properties of the created env.local to "Always copy"
- `docker-compose up`
- Run the application in Visual Studio
- Open the Swagger UI at `/swagger/index/html` or view the job queue at `/hangfire`

### Building and testing locally

This project is known to run in Mono, Microsoft Visual Studio and JetBrains Rider, including on a macos platform. The project must be built for .NET version 7.0. When running locally, be sure to start the docker-compose container prior to running the project.

Configure a local instance of the Get Into Teaching application to connect to a local instance of the API by setting the application's environment variables to match the API, e.g.

```bash
export GIT_API_ENDPOINT=https://localhost:5001/api
export GIT_API_TOKEN=secret-git
```
The application will skip self-certified SSL certificate validation if communicating with a local API _and_ running in development mode.

Be aware that any change to the API will affect multiple applications and **all changes must be non-breaking**.

## Useful Links

As the API is service-facing it has no user interface, but in non-production environments you can access a [dashboard for Hangfire](https://getintoteachingapi-development.test.teacherservices.cloud/hangfire/) and the [Swagger UI](https://getintoteachingapi-development.test.teacherservices.cloud/swagger/index.html). You will need the basic auth credentials to access these dashboards.

## Deployment

### Environments

The API is deployed to [AKS](https://github.com/DFE-Digital/teacher-services-cloud/). We currently have three hosted environments; `development`, `test` and `production`. This can get confusing because our ASP.NET Core environments are `development`, `staging`, `test` and `production`. Here is a table to try and make sense of the combinations:

| Environment             | ASP.NET Core Environment | URL                                                               |
| ----------------------- | ------------------------ | ----------------------------------------------------------------- |
| development (AKS)       | Staging                  | https://getintoteachingapi-development.test.teacherservices.cloud/|
| test (AKS)              | Staging                  | https://getintoteachingapi-test.test.teacherservices.cloud/       |
| production (AKS)        | Production               | https://getintoteachingapi-production.teacherservices.cloud/      |
| development (local)     | Development              | localhost                                                         |
| test (local)            | Test                     | n/a                                                               |

### Process

When you merge a branch to `master` it will automatically be deployed to the [development](#environments) and [test](#environments) environments via GitHub Actions and a tagged release will be created (the tag will use the PR number). You can then test the changes using the corresponding dev/test environments of the other GiT services. Once you're happy and want to ship to [production](#environments) you need to note the tag of your release and go to the `Manual Release` GitHub Action; from there you can select `Run workflow`, choose the `production_aks` environment and enter your release number.

### Rollbacks

If you make a deployment and need to roll it back the quickest way is to `Manual Release` a previous version and revert your changes.

### Deploying to Test/Dev Manually

It can be useful on occasion to test changes in a hosted dev/test environment prior to merging. If you want to do this you need to raise a PR with your changes and manually create a tagged release (be sure to use something other than your PR number as the release tag) that can then be deployed to the dev/test environment as in the above process.

## Technical Details

### Environment

If you want to run the API locally end-to-end you will need to populate the local development environment variables. Run:

```
az login
make local setup-local-env
```

Then **set properties of the created env.local to "Always copy"**.

Other environment variables are available (see the `IEnv` interface) but are not necessary to run the bare-bones application.

### Secrets

Secrets are stored in Azure keyvaults. There is a Makefile that should be used to view or edit the secrets, for example:

```
make development print-app-secrets
make test edit-app-secrets
```
If you want to edit the local secrets, you can run:

```
make edit-local-app-secrets
```

### Documentation

[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) is used for generating Swagger documentation. We use the swagger-ui middleware to expose interactive documentation when the application runs.

We also use the [MicroElements.Swashbuckle.FluentValidation](https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation) package to make Swashbuckle aware of the FluentValidation classes, so that additional validation meta data gets displayed alongside model attributes.

You can hit the API endpoints directly from the Swagger UI - hit the `Authorize` button at the top and enter the development `Authorization` header value `Bearer <shared_secret>`. You can then open up an endpoint and 'Try it out' to execute a request.

### Validation

[FluentValidation](https://fluentvalidation.net) is used for validating the various models. The validators are registered at startup, so validating incoming payloads that have a corresponding validator is as simple as calling `ModelState.IsValid`.

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

A difficulty with these tests is ensuring that the services all have a consistent, global view of the service data state prior to executing the contract tests. We currently maintain the service data in `Contracts/Data` (to be centralised in a GitHub repository, but currently duplicated in each service).

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

### HTTP Caching

The content that we cache in Postgres that originates from the CRM is served with HTTP cache headers. There is a `PrivateShortTermResponseCache` annotation that can be added to an action to apply the appropriate cache headers.

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
APPLY_CANDIDATE_API_FEATURE=on
```

We can check for the feature with `env.IsFeatureOn("APPLY_CANDIDATE_API")`.

## Monitoring

### Logs

We use [logit.io](https://kibana.logit.io/app/kibana) to host a Kibana instance for our logs. The logs persist for **14 days** and contain logs for all our production and test instances. You can filter to a specific instance using the `cf.app` field.

### Metrics

We use Prometheus to collect our metrics into an InfluxDB instance.  Metrics are exposed to Prometheus on the `/metrics` endpoint; [prometheus-net](https://github.com/prometheus-net/prometheus-net) is used for collecting and exposing the metrics.

The metrics are presented using Grafana. All the configuration/infrastructure is currently configured in the terraform files.

Note that if you change the Grafana dashboard **it will not persist** and you need to instead export the dashboard and [updated it in the GitHub repository](https://github.com/DFE-Digital/get-into-teaching-api/tree/master/monitoring/grafana/dashboards). These are re-applied on API deployment.

### Alerts

We use Prometheus Alert Manager to notify us when something has gone wrong. It will post to the relevant Slack channel and contain a link to the appropriate Grafana dashboard and/or runbook.

You can add/configure alerts in the [alert.rules file](https://github.com/DFE-Digital/get-into-teaching-api/blob/master/monitoring/prometheus/alert.rules).

All the runbooks are also [hosted in GitHub](https://github.com/DFE-Digital/get-into-teaching-api/tree/master/docs/runbooks).

### Error reporting

We use [Sentry](sentry.io) to capture application errors. They will be posted to the relvant Slack channel when they first occur.

## CRM Integration

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

#### Including Fields Behind a Feature Flag

If a new field is not yet available in one of the CRM environments you can still develop and deploy code against the attribute as long as you put it behind a feature switch. For example, to map a field behind the `MY_FEATURE` feature you can use:

```
[EntityField("dfe_brand_new_field", null, null, new string[] { "MY_FEATURE" })]
public string NewField { get; set; }
```

You would then only turn on the feature for CRM environments that support it, preventing the mapper trying to map the field before it is available.

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

Occasionally it can be useful to hook into the mapping behaviour that is encapsulated in the `BaseModel`. An example of this may be to delete a related entity from the CRM, for example:

```
protected override void FinaliseEntity(Entity source, ICrmService crm, OrganizationServiceContext context)
{
    DeleteLink(source, crm, context, someModel, nameof(someModel));
}
```
