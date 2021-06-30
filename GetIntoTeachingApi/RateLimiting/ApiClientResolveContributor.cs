using System.Threading.Tasks;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;

namespace GetIntoTeachingApi.RateLimiting
{
    public class ApiClientResolveContributor : IClientResolveContributor
    {
        private readonly IClientManager _clientManager;
        private readonly string _clientIdHeader;

        public ApiClientResolveContributor(string clientIdHeader)
        {
            _clientManager = new ClientManager(new Env());
            _clientIdHeader = clientIdHeader;
        }

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            string clientId = null;

            if (httpContext.Request.Headers.TryGetValue(_clientIdHeader, out var value))
            {
                var apiKey = value.ToString().Replace("Bearer ", string.Empty);
                clientId = _clientManager.GetClient(apiKey)?.ApiKeyPrefix;
            }

            return Task.FromResult(clientId);
        }
    }
}
