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
        private readonly ILogger<ApiClientHandler> _logger;

        public ApiClientHandler(
            IClientManager clientManager,
            IOptionsMonitor<ApiClientSchemaOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            _clientManager = clientManager;
            _logger = loggerFactory.CreateLogger<ApiClientHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var client = _clientManager.GetClient(token);

            if (client == null || string.IsNullOrWhiteSpace(client.ApiKey))
            {
                _logger.LogWarning("ApiClientHandler - Token is not valid");
                return Task.FromResult(AuthenticateResult.Fail("Token is not valid"));
            }

            var claims = new[] { new Claim("token", token), new Claim(ClaimTypes.Role, client.Role) };
            var identity = new ClaimsIdentity(claims, nameof(ApiClientHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}