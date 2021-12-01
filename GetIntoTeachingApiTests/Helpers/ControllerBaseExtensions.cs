using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApiTests.Helpers
{
    public static class ControllerBaseExtensions
    {
        public static void MockUser(this ControllerBase controller, string name = "Client")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name),

            }, "mock"));

            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }
    }
}
