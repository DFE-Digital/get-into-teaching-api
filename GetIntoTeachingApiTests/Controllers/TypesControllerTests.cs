using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TypesControllerTests
    {
        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TypesController));
        }
    }
}
