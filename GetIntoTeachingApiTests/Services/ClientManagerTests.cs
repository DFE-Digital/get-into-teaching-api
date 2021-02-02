using System.Collections.Generic;
using System.Linq;
using CSScriptLib;
using FluentAssertions;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class ClientManagerTests
    {
        private readonly Mock<IEnv> _mockEnv;

        public ClientManagerTests()
        {
            _mockEnv = new Mock<IEnv>();
            _mockEnv.Setup(m => m.EnvironmentName).Returns("Test");
        }

        [Fact]
        public void Clients_AreAllValid()
        {
            _mockEnv.Setup(m => m.Get(It.IsAny<string>())).Returns(null as string);
            var manager = new ClientManager(_mockEnv.Object);

            var clients = manager.Clients;

            var validator = new ClientValidator();
            manager.Clients.ForEach(c => validator.Validate(c).IsValid.Should().BeTrue());
        }

        [Fact]
        public void Clients_ContainNoDuplicateApiKeyPrefixes()
        {
            _mockEnv.Setup(m => m.Get(It.IsAny<string>())).Returns(null as string);
            var manager = new ClientManager(_mockEnv.Object);
            var keys = manager.Clients.Select(c => c.ApiKeyPrefix);

            keys.Distinct().Count().Should().Be(keys.Count());
        }

        [Fact]
        public void GetClient_WithSecret_ReturnsCorrespondingClient()
        {
            _mockEnv.Setup(m => m.Get("ADMIN_API_KEY")).Returns("admin_secret");
            var manager = new ClientManager(_mockEnv.Object);

            var client = manager.GetClient("admin_secret");

            client.Name.Should().Be("Administrator");
            client.ApiKeyPrefix.Should().Be("ADMIN");
            client.Role.Should().Be("Admin");
            client.Description.Should().Be("A super-user client for Development convenience and (securely) debugging in production");
        }

        [Fact]
        public void GetClient_WithIncorrectSecret_ReturnsNull()
        {
            _mockEnv.Setup(m => m.Get("an_invalid_secret")).Returns(null as string);
            var manager = new ClientManager(_mockEnv.Object);

            var client = manager.GetClient("an_invalid_secret");

            client.Should().BeNull();
        }
    }
}