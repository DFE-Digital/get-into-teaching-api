﻿using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Controllers.GetIntoTeaching;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers.GetIntoTeaching
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
        public void Authorize_IsPresent()
        {
            typeof(TeachingEventBuildingsController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
        {
            typeof(TeachingEventBuildingsController).Should()
                .BeDecoratedWith<PrivateShortTermResponseCacheAttribute>();
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
