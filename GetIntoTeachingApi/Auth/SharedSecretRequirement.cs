using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApi.Auth
{
    public class SharedSecretRequirement : IAuthorizationRequirement
    {
        public string SharedSecret { get; set; }

        public SharedSecretRequirement(string sharedSecret)
        {
            SharedSecret = sharedSecret;
        }
    }
}
