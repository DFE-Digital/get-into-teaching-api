# Get into Teaching API 
![Build and Deploy](https://github.com/DFE-Digital/get-into-teaching-api/workflows/Build%20and%20Deploy/badge.svg)

> Provides a RESTful API for integrating with the Get into Teaching CRM.

The Get into Teaching (GIT) API sits in front of the GIT CRM, which uses the [Microsoft Dynamics365](https://docs.microsoft.com/en-us/dynamics365/) platform (the [Customer Engagement](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/overview) module is used for storing Candidate information and the [Marketing](https://docs.microsoft.com/en-us/dynamics365/marketing/developer/using-events-api) module for managing Events).

The GIT API aims to provide:

- Simple, task-based RESTful APIs.
- Message queueing (while the GIT CRM is offline for updates).
- Validation to ensure consistency across services writing to the GIT CRM.

## Getting Started

The API is an ASP.NET Core web application; to get up and running clone the repository and open `GetIntoTeachingApi.sln` in Visual Studio.

Before you build the app you will need to add a package source for the GOV.UK Notify service:

```
https://api.bintray.com/nuget/gov-uk-notify/nuget
```

Next you will need to set up the environment (see the `Environment` section below) before booting up the dependent services in Docker with `docker-compose up`.

When the application runs in development it will open the Swagger documentation by default (the development shared secret is `abc123`).

On the first run it will do a long sync of UK postcode information into the Postgres instance running in Docker - you can monitor the progress via the Hangfire dashboard (subsequent start ups should be quicker).

### Environment

If you want to run the API locally end-to-end you will need to set some environment variables (you can specify these in `GetIntoTeachingApi/Properties/launchSettings.json` under `environmentVariables`):

```
# CRM credentials
CRM_SERVICE_URL=****
CRM_CLIENT_ID=****
CRM_CLIENT_SECRET=****
CRM_TENANT_ID=****

# GOV.UK Notify Service credentials
NOTIFY_API_KEY=****
```

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

Property names in request models should be consistent with any hidden `BaseModel` models they encapsulate and map to; this way the client can resolve the validation error messages back to the original request attributes. For example, the `MailingListAddMember.UkDegreeGradeId` maps to and is consistent with `MailingListAddMember.Candidate.Qualifications[0].UkDegreeGradeId`.

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

### Emails

We send emails using the [GOV.UK Notify](https://www.notifications.service.gov.uk/) service; leveraging the [.Net Client](https://github.com/alphagov/notifications-net-client).

### Background Jobs

[Hangfire](https://www.hangfire.io/) is used for queueing and processing background jobs; an in-memory storage is used for development and PostgreSQL is used in production (the PRO version is required to use Redis as the storage provider). Failed jobs get retries on a 60 minute interval a maximum of 24 times before they are deleted (in development this is reduced to 1 minute interval a maximum of 5 times) - if this happens we attempt to inform the user by sending them an email.

The Hangfire web dashboard can be accessed at `/hangfire` in development.

### Database

We run Entity Framework Core in order to persist models/data to a Postgres database.

Migrations are applied from code when the application starts (see `DbConfiguration.cs`). You can add a migration by modifying the models and running `Add-Migration MyNewMigration` from the package manager console. As we run SQLite in development and Postgres in production we need to tell EF Core which provider to use at design-time; the `GetIntoTeachingDbContextFactory` takes care of this.

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

### Customising the Mapping Behaviour

Occasionally it can be useful to hook into the mapping behaviour that is encapsulated in the `BaseModel`. An example of this may be to prevent mapping to an entity if we know that it will create a duplicate in the CRM. We do this as part of `TeachingEventRegistration` to ensure we don't register the same candidate as an attendee of the event more than once:

```
protected override bool ShouldMap(ICrmService crm)
{
    return crm.CandidateYetToRegisterForTeachingEvent(CandidateId, EventId);
}
```
