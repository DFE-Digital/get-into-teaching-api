using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GetIntoTeachingApi.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GetIntoTeachingApi.Utils
{
    public class LoggableMessageComposer
    {
        public static string LogMessage(string prefix, ControllerActionDescriptor descriptor, IEnumerable<string> messages)
        {
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerName;
            var payload = string.Join("\n", messages.Where(m => !string.IsNullOrEmpty(m)));

            return $"{prefix} {controllerName}:{actionName} - {payload}";
        }

        public static string LogMessageForObject(object obj)
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
