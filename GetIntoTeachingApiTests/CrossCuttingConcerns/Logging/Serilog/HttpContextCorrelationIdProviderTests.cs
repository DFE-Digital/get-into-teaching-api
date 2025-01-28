using GetIntoTeachingApi.CrossCuttingConcerns.Logging;
using GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common;
using GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.Shared.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Diagnostics;
using Xunit;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog
{
    public sealed class HttpContextCorrelationIdProviderTests
    {
        [Fact]
        public void GetCorrelationId_WithCorrelationIdAppliedToHttpContext_ReturnsConfiguredCorrelationId()
        {
            // arrange
            HttpContext httpContext = HttpContextTestDouble.DefaultMock();
            Guid correlationId = Guid.NewGuid();
            AddDefaultCorrelationId(httpContext, correlationId);

            IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider =
                new HttpContextCorrelationIdProvider(HttpContextAccessorTestDouble.MockFor(httpContext));

            // act
            Guid contextCorrelationId = httpContextCorrelationIdProvider.GetCorrelationId();

            // assert
            Assert.Equal(contextCorrelationId, correlationId);
        }

        [Fact]
        public void GetCorrelationId_WithNoCorrelationIdAppliedToHttpContext_ReturnsDefaultGuidEmptyCorrelationId()
        {
            // arrange
            IHttpContextCorrelationIdProvider httpContextCorrelationIdProvider =
                new HttpContextCorrelationIdProvider(HttpContextAccessorTestDouble.DefaultMock());

            // act
            Guid contextCorrelationId = httpContextCorrelationIdProvider.GetCorrelationId();

            // assert
            Assert.Equal(contextCorrelationId, Guid.Empty);
        }

        private void AddDefaultCorrelationId(HttpContext httpContext, Guid correlationId)
        {
            IHttpActivityFeature activityFeature =
                httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();
            Activity activity = activityFeature.Activity;
            activity.AddTag(CorrelationPropertyKeys.PerRequestCorrelationIdPropertyNameKey, correlationId);
        }
    }
}
