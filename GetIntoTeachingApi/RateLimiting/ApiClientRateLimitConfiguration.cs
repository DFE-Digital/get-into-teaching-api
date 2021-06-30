using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.RateLimiting
{
    public class ApiClientRateLimitConfiguration : RateLimitConfiguration
    {
        public ApiClientRateLimitConfiguration(
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
            : base(ipOptions, clientOptions)
        {
        }

        public override void RegisterResolvers()
        {
            ClientResolvers.Add(new ApiClientResolveContributor(ClientRateLimitOptions.ClientIdHeader));
        }
    }
}