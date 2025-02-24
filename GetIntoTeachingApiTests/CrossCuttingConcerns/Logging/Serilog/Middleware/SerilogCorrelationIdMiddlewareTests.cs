using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.Middleware;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.Shared.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.Middleware
{
    public sealed class SerilogCorrelationIdMiddlewareTests
    {
        [Fact]
        public void InvokeAsync_AssignsUniqueCorrelationId()
        {
            // arrange
            SerilogCorrelationIdMiddleware serilogCorrelationIdMiddleware = new();

            // act
            HttpContext httpContext = HttpContextTestDouble.DefaultMock();

            serilogCorrelationIdMiddleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

            // assert
            Assert.NotEqual(ExtractCorrelationIdFromContext(httpContext), Guid.Empty);
        }

        private static Guid ExtractCorrelationIdFromContext(HttpContext httpContext)
        {
            IHttpActivityFeature httpActivityFeature =
                    httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();

            return (Guid)httpActivityFeature.Activity
                .GetTagItem(CorrelationPropertyKeys.PerRequestCorrelationIdPropertyNameKey);
        }
    }
}
