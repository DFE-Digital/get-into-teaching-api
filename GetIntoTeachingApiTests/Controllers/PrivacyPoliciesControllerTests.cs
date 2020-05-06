using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class PrivacyPoliciesControllerTests
    {
        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(PrivacyPoliciesController));
        }
    }
}
