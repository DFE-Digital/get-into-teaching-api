using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.Auth
{
    public class ApiClientHandler : AuthenticationHandler<ApiClientSchemaOptions>
    {
        private readonly IClientManager _clientManager;

        public ApiClientHandler(
            IClientManager clientManager,
            IOptionsMonitor<ApiClientSchemaOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            _clientManager = clientManager;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = GetApiKey();
            var claims = AuthenticateApiClient(apiKey);

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (string.IsNullOrWhiteSpace(apiKey) || !claims.Any())
            {
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