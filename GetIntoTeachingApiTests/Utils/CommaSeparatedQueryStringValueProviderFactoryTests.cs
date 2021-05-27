using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class CommaSeparatedQueryStringValueProviderFactoryTests
    {
        [Fact]
        public void CreateValueProviderAsync_InsertsCommaSepratedQueryStrinValueProvider()
        {
            var factory = new CommaSeparatedQueryStringValueProviderFactory("key", ",");
            var actionContext = new ActionContext();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?key=value");
            actionContext.HttpContext = httpContext;
            var context = new ValueProviderFactoryContext(actionContext);

            factory.CreateValueProviderAsync(context);

            var expectedProvider = new CommaSeparatedQueryStringValueProvider("key",
                context.ActionContext.HttpContext.Request.Query, ",");
            context.ValueProviders[0].Should().BeEquivalentTo(expectedProvider);
        }
    }
}
