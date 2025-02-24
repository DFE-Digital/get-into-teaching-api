using GetIntoTeachingApi.CrossCuttingConcerns.Logging;
using Moq;
using System;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.TestDoubles
{
    public static class HttpContextCorrelationIdProviderTestDouble
    {
        public static IHttpContextCorrelationIdProvider MockFor(Guid correlationId)
        {
            var httpContextCorrelationIdProvider = new Mock<IHttpContextCorrelationIdProvider>();

            httpContextCorrelationIdProvider
                .Setup(provider => provider.GetCorrelationId()).Returns(correlationId);

            return httpContextCorrelationIdProvider.Object;
        }
    }
}
