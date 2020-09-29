using System.Collections.Generic;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Filters
{
    public class LogRequestsAttributeTests
    {
        private readonly LogRequestsAttribute _filter;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly ActionExecutedContext _actionExecutedContext;
        private readonly Mock<ILogger<LogRequestsAttribute>> _mockLogger;

        public LogRequestsAttributeTests()
        {
            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary()
            )
            {
                ActionDescriptor = new ControllerActionDescriptor()
                {
                    ActionName = "Action",
                    ControllerName = "Controller"
                }
            };

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            _actionExecutedContext = new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                Mock.Of<Controller>()
            );

            _mockLogger = new Mock<ILogger<LogRequestsAttribute>>();

            _filter = new LogRequestsAttribute(_mockLogger.Object);
        }

        [Fact]
        public void OnActionExecuting_WithLoggable_LogsTheRequestWithPayload()
        {
            _actionExecutingContext.ActionArguments.Add("person", new StubLoggable());

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockLogger.VerifyInformationWasCalledExactly("Request Controller:Action - {\"Name\":\"Ross\"}");
        }

        [Fact]
        public void OnActionExecuting_WithMultipleArgumentsOfLoggable_LogsTheRequestWithPayloads()
        {
            _actionExecutingContext.ActionArguments.Add("person1", new StubLoggable("Ross"));
            _actionExecutingContext.ActionArguments.Add("person2", new StubLoggable("James"));
            _actionExecutingContext.ActionArguments.Add("person3", new StubUnloggable());

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockLogger.VerifyInformationWasCalledExactly("Request Controller:Action - {\"Name\":\"Ross\"}\n{\"Name\":\"James\"}");
        }

        [Fact]
        public void OnActionExecuting_WithArrayOfLoggable_LogsTheRequestWithPayload()
        {
            var people = new List<StubLoggable>() { new StubLoggable("Ross"), new StubLoggable("James") };
            _actionExecutingContext.ActionArguments.Add("people", people);

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockLogger.VerifyInformationWasCalledExactly("Request Controller:Action - [{\"Name\":\"Ross\"},{\"Name\":\"James\"}]");
        }

        [Fact]
        public void OnActionExecuting_WithUnloggable_LogsTheRequestWithoutPayload()
        {
            _actionExecutingContext.ActionArguments.Add("person", new StubUnloggable());

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockLogger.VerifyInformationWasCalledExactly("Request Controller:Action - ");
        }

        [Fact]
        public void OnActionExecuting_WithPrimitive_LogsTheRequestWithoutPayload()
        {
            _actionExecutingContext.ActionArguments.Add("password", 12345);

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockLogger.VerifyInformationWasCalledExactly("Request Controller:Action - ");
        }

        [Fact]
        public void OnActionExecuted_WithLoggable_LogsTheResponseWithPayload()
        {
            _actionExecutedContext.Result = new OkObjectResult(new StubLoggable());

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockLogger.VerifyInformationWasCalledExactly("Response Controller:Action - {\"Name\":\"Ross\"}");
        }

        [Fact]
        public void OnActionExecuted_WithArrayOfLoggable_LogsTheResponseWithPayload()
        {
            var people = new List<StubLoggable>() { new StubLoggable("Ross"), new StubLoggable("James") };
            _actionExecutedContext.Result = new OkObjectResult(people);

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockLogger.VerifyInformationWasCalledExactly("Response Controller:Action - [{\"Name\":\"Ross\"},{\"Name\":\"James\"}]");
        }

        [Fact]
        public void OnActionExecuted_WithUnloggable_LogsTheRequestWithoutPayload()
        {
            _actionExecutedContext.Result = new OkObjectResult(new StubUnloggable());

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockLogger.VerifyInformationWasCalledExactly("Response Controller:Action - ");
        }

        [Fact]
        public void OnActionExecuted_WithPrimitive_LogsTheRequestWithoutPayload()
        {
            _actionExecutedContext.Result = new OkObjectResult(12345);

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockLogger.VerifyInformationWasCalledExactly("Response Controller:Action - ");
        }

        [Loggable]
        private class StubLoggable
        {
            public string Name { get; set; } = "Ross";
            [SensitiveData]
            public string Password { get; set; } = "sensitive";
            [JsonIgnore]
            public string Ignored { get; set; } = "ignored";

            public StubLoggable() { }

            public StubLoggable(string name)
            {
                Name = name;
            }
        }

        private class StubUnloggable
        {
            public string Name { get; set; } = "Ross";
        }
    }
}
