using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GetIntoTeachingApi.Auth
{
    public class SharedSecretSchemeOptions : AuthenticationSchemeOptions { }

    public class SharedSecretHandler : AuthenticationHandler<SharedSecretSchemeOptions>
    {
        private readonly IEnv _env;

        public SharedSecretHandler(IEnv env, IOptionsMonitor<SharedSecretSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _env = env;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not set."));
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (token != _env.SharedSecret)
            {
                return Task.FromResult(AuthenticateResult.Fail("Token is not valid."));
            }

            var claims = new[] { new Claim("token", token) };
            var identity = new ClaimsIdentity(claims, nameof(SharedSecretHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
