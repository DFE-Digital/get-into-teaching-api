# Get into Teaching API

![Build and Deploy](https://github.com/DFE-Digital/get-into-teaching-api/workflows/Build%20and%20Deploy/badge.svg)

> Provides a RESTful API for integrating with the Get into Teaching CRM.

The Get into Teaching (GIT) API sits in front of the GIT CRM, which uses the [Microsoft Dynamics365](https://docs.microsoft.com/en-us/dynamics365/) platform (the [Customer Engagement](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/overview) module is used for storing Candidate information and the [Marketing](https://docs.microsoft.com/en-us/dynamics365/marketing/developer/using-events-api) module for managing Events).

The GIT API aims to provide:

- Simple, task-based RESTful APIs.
- Message queueing (while the GIT CRM is offline for updates).
- Validation to ensure consistency across services writing to the GIT CRM.

## Architecture

The API is a .NET Core web app that interacts with Dynamics365 using the PowerPlatform [CdsServiceClient](https://github.com/microsoft/PowerPlatform-CdsServiceClient) wrapped in an [OrganizationServiceContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.client.organizationservicecontext?view=dynamics-general-ce-9). The Dynamics entities are modelled using plain C# objects decorated with custom attributes to describe how they map to/from [Xrm Entity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.entity?view=dynamics-general-ce-9) objects. The mapping is performed by the `BaseModel` using reflection and the custom attributes. All interactions with Dynamics funnel through the `CrmService` and, ultimately, the `OrganizationServiceAdapter` which is a minimal abstraction of the `CdsServiceClient` and enables mocking the Dynamics boundry in the test suite. The web app also integrates with the [GOV.UK Notify Service](https://www.notifications.service.gov.uk/) to send emails to candidates.

Other methods of integrating with the Dynamics instance were considered:

- [Common Data Service Web API](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/webapi/overview)
  - Microsoft suggest generating an OData client from the Dynamics metadata to interact with their Web API. When we attempted this we found the resulting client did not build (perhaps due to the size/complexity of our Dynamics instance).
- [CrmServiceClient](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient?view=dynamics-xrmtooling-ce-9)
  - This is the .NET Framework compatible SDK and it was discounted as we were keen to use .NET Core.

The `CdsServiceClient` we are using is currently in ALPHA, however it mirrors the `CmsServiceClient` interface very closely so we have the option of switching to that if we find it is not stable enough. We could also swap to using the OData API (without a generated client) relatively easily by updating the `BaseModel` to map to JSON instead of [Xrm Entity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.entity?view=dynamics-general-ce-9) objects.

With hindsight, the OData API may have been a better choice initially as the SDK - whilst giving us some functionality more easily (such as authentication) - has ended up being harder to work with in other areas (doing a 'deep insert', for example, required exposing the `OrganizationServiceContext` and passing down the mapping chain with wasn't ideal).

## Getting Started

The API is an ASP.NET Core web application; to get up and running clone the repository and open `GetIntoTeachingApi.sln` in Visual Studio.

When the application runs in development it will open the Swagger documentation by default.

### Environment

If you want to run the API locally end-to-end you will need to set some environment variables:

```
# Secret shared between the API and client.
SHARED_SECRET=****

# Secret used when generating Timed One Time Passwords
TOTP_SECRET_KEY=****

# CRM credentials
CRM_SERVICE_URL=****
CRM_CLIENT_ID=****
CRM_CLIENT_SECRET=****

# GOV.UK Notify Service credentials
NOTIFY_API_KEY=****
```

### Documentation

[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) is used for generating Swagger documentation. We use the swagger-ui middleware to expose interactive documentation when the application runs.

We also use the [MicroElements.Swashbuckle.FluentValidation](https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation) package to make Swashbuckle aware of the FluentValidation classes, so that additional validation meta data gets displayed alongside model attributes.

You can hit the API endpoints directly from the Swagger UI - hit the `Authorize` button at the top and enter the development `Authorization` header value `Bearer <shared_secret>`. You can then open up an endpoint and 'Try it out' to execute a request.

### Validation

[FluentValidation](https://fluentvalidation.net/) is used for validating the various models. The validators are registered at startup, so validating incoming payloads that have a corresponding validator is as simple as calling `ModelState.IsValid`.

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
