using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LogRequestsAttribute : Attribute, IActionFilter
    {
        private readonly ILogger<LogRequestsAttribute> _logger;

        public LogRequestsAttribute()
        {
            var factory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = factory.CreateLogger<LogRequestsAttribute>();
        }

        public LogRequestsAttribute(ILogger<LogRequestsAttribute> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            var messages = context.ActionArguments.Select(a => LoggableMessageComposer.LogMessageForObject(a.Value));
            var message = LoggableMessageComposer.LogMessage("Request", descriptor, messages);
            _logger.LogInformation(message);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                var messages = new List<string> { LoggableMessageComposer.LogMessageForObject(result.Value) };
                var message = LoggableMessageComposer.LogMessage("Response", descriptor, messages);
                _logger.LogInformation(message);
            }
        }
    }
}
