using System;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CommaSeparatedQueryStringAttribute : Attribute, IResourceFilter
    {
        private readonly CommaSeparatedQueryStringValueProviderFactory _factory;

        public CommaSeparatedQueryStringAttribute()
            : this(",")
        {
        }

        public CommaSeparatedQueryStringAttribute(string separator)
        {
            _factory = new CommaSeparatedQueryStringValueProviderFactory(separator);
        }

        public CommaSeparatedQueryStringAttribute(string[] keys, string separator)
        {
            _factory = new CommaSeparatedQueryStringValueProviderFactory(keys, separator);
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Not required.
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.ValueProviderFactories.Insert(0, _factory);
        }
    }
}
