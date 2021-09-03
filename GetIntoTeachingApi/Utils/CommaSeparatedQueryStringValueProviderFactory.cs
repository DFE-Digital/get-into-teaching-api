using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GetIntoTeachingApi.Utils
{
    public class CommaSeparatedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private readonly string _separator;
        private readonly string[] _keys;

        public CommaSeparatedQueryStringValueProviderFactory(string separator)
            : this(null, separator)
        {
        }

        public CommaSeparatedQueryStringValueProviderFactory(string[] keys, string separator)
        {
            _keys = keys;
            _separator = separator;
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var provider = new CommaSeparatedQueryStringValueProvider(
                _keys,
                context.ActionContext.HttpContext.Request.Query,
                _separator);

            context.ValueProviders.Insert(0, provider);

            return Task.CompletedTask;
        }
    }
}
