using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GetIntoTeachingApi.Utils
{
    public class CommaSeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string _key;

        public CommaSeparatedQueryStringValueProviderFactory(string separator)
            : this(null, separator)
        {
        }

        public CommaSeparatedQueryStringValueProviderFactory(string key, string separator)
        {
            _key = key;
            _separator = separator;
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var provider = new CommaSeparatedQueryStringValueProvider(
                _key,
                context.ActionContext.HttpContext.Request.Query,
                _separator);

            context.ValueProviders.Insert(0, provider);

            return Task.CompletedTask;
        }
    }
}
