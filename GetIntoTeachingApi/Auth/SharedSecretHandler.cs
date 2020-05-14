using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Auth
{
    public class SharedSecretHandler : AuthorizationHandler<SharedSecretRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SharedSecretHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SharedSecretRequirement requirement)
        {
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authorizationHeader != null && authorizationHeader.Contains($"Bearer {requirement.SharedSecret}"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
