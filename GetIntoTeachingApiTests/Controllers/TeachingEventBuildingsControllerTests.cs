using FluentAssertions;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventBuildingsControllerTests
    {
        private readonly Mock<IStore> _mockStore;

        private readonly TeachingEventBuildingsController _controller;
        public TeachingEventBuildingsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _controller = new TeachingEventBuildingsController(_mockStore.Object);
        }

        [Fact]
        public void GetTeachingEventBuildings_ReturnsAllTeachingEventBuildings()
        {
            var mockBuildings = new List<TeachingEventBuilding>()
            {
                new TeachingEventBuilding() { AddressCity = "test" }
            }
            .AsQueryable();

            _mockStore.Setup(mock => mock.GetTeachingEventBuildings()).Returns(mockBuildings);

            var response = _controller.GetTeachingEventBuildings();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockBuildings);
        }
    }
}
