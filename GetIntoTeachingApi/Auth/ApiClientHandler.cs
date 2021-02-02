using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.Auth
{
    public class ApiClientHandler : AuthenticationHandler<ApiClientSchemaOptions>
    {
        private readonly IClientManager _clientManager;
        private readonly IEnv _env;
        private readonly ILogger<ApiClientHandler> _logger;

        public ApiClientHandler(
            IEnv env,
            IClientManager clientManager,
            IOptionsMonitor<ApiClientSchemaOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            _env = env;
            _clientManager = clientManager;
            _logger = loggerFactory.CreateLogger<ApiClientHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = GetApiKey();
            var claims = AuthenticateApiClient(apiKey).Concat(AuthenticateSharedSecret(apiKey));

            if (string.IsNullOrWhiteSpace(apiKey) || !claims.Any())
            {
                _logger.LogWarning("ApiClientHandler - API key is not valid");
                return Task.FromResult(AuthenticateResult.Fail("API key is not valid"));
            }

            var identity = new ClaimsIdentity(claims, nameof(ApiClientHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private string GetApiKey()
        {
            return Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
        }

        private IEnumerable<Claim> AuthenticateSharedSecret(string token)
        {
            var secrets = new[] { _env.SharedSecret };

            if (!secrets.Contains(token))
            {
                return new Claim[0];
            }

            return new[] { new Claim("token", token), new Claim(ClaimTypes.Role, "Admin") };
        }

        private IEnumerable<Claim> AuthenticateApiClient(string token)
        {
            var client = _clientManager.GetClient(token);

            if (client == null)
            {
                return new Claim[0];
            }

            return new[] { new Claim("token", token), new Claim(ClaimTypes.Role, client.Role) };
        }
    }
}