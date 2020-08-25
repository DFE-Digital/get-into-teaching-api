using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.Auth
{
    public class SharedSecretHandler : AuthenticationHandler<SharedSecretSchemeOptions>
    {
        private readonly IEnv _env;
        private readonly ILogger<SharedSecretHandler> _logger;

        public SharedSecretHandler(
            IEnv env,
            IOptionsMonitor<SharedSecretSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            _env = env;
            _logger = loggerFactory.CreateLogger<SharedSecretHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            var secrets = new[] { _env.SharedSecret, _env.PenTestSharedSecret };

            if (string.IsNullOrWhiteSpace(token) || !secrets.Contains(token))
            {
                _logger.LogWarning("SharedSecretHandler - Token is not valid");
                return Task.FromResult(AuthenticateResult.Fail("Token is not valid"));
            }

            var claims = new[] { new Claim("token", token) };
            var identity = new ClaimsIdentity(claims, nameof(SharedSecretHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
