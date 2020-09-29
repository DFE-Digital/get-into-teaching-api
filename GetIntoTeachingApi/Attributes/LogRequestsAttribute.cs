using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            var messages = context.ActionArguments.Select(a => LogMessageForObject(a.Value));
            var message = LogMessage("Request", descriptor, messages);
            _logger.LogInformation(message);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                var messages = new List<string> { LogMessageForObject(result.Value) };
                var message = LogMessage("Response", descriptor, messages);
                _logger.LogInformation(message);
            }
        }

        private static string LogMessage(string type, ControllerActionDescriptor descriptor, IEnumerable<string> messages)
        {
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerName;
            var payload = string.Join("\n", messages.Where(m => !string.IsNullOrEmpty(m)));

            return $"{type} {controllerName}:{actionName} - {payload}";
        }

        private static string LogMessageForObject(object obj)
        {
            var type = obj.GetType().GetGenericArguments().FirstOrDefault();

            if (type == null)
            {
                type = obj.GetType();
            }

            if (type.GetCustomAttribute<LoggableAttribute>() == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(
                obj,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new FilterSensitiveDataContractResolver(),
                });
        }

        private class FilterSensitiveDataContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                property.ShouldSerialize = instance =>
                    instance.GetType().GetCustomAttribute<LoggableAttribute>() != null &&
                    member.GetCustomAttribute<SensitiveDataAttribute>() == null &&
                    member.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() == null;

                return property;
            }
        }
    }
}
