using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.RateLimiting
{
    public class ApiClientRateLimitConfiguration : RateLimitConfiguration
    {
        public ApiClientRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
            : base(httpContextAccessor, ipOptions, clientOptions)
        {
        }

        protected override void RegisterResolvers()
        {
            ClientResolvers.Add(new ApiClientResolveContributor(HttpContextAccessor, ClientRateLimitOptions.ClientIdHeader));
        }
    }
}
