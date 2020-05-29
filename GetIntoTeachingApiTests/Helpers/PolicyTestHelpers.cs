using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System;

namespace GetIntoTeachingApiTests.Helpers
{
    class PolicyTestHelpers
    {
        public static void VerifyTypeIsAuthorizeWithSharedSecret(Type type) 
        {
            type.Should().BeDecoratedWith<AuthorizeAttribute>(attribute => (attribute.Policy.Contains("SharedSecret")));
        }
    }
}
