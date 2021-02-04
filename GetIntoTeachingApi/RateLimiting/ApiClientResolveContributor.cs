using AspNetCoreRateLimit;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;

namespace GetIntoTeachingApi.RateLimiting
{
    public class ApiClientResolveContributor : IClientResolveContributor
    {
        private readonly IClientManager _clientManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _clientIdHeader;

        public ApiClientResolveContributor(IHttpContextAccessor contextAccessor, string clientIdHeader)
        {
            _clientManager = new ClientManager(new Env());
            _contextAccessor = contextAccessor;
            _clientIdHeader = clientIdHeader;
        }

        public string ResolveClient()
        {
            string clientId = null;

            if (_contextAccessor.HttpContext.Request.Headers.TryGetValue(_clientIdHeader, out var value))
            {
                var apiKey = value.ToString().Replace("Bearer ", string.Empty);
                clientId = _clientManager.GetClient(apiKey)?.ApiKeyPrefix;
            }

            return clientId;
        }
    }
}
