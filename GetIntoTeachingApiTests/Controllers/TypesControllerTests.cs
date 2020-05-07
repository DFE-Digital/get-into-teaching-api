using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TypesControllerTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<ILogger<TypesController>> _mockLogger;
        private readonly TypesController _controller;

        public TypesControllerTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockLogger = new Mock<ILogger<TypesController>>();
            _controller = new TypesController(_mockLogger.Object, _mockCrm.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TypesController));
        }

        [Fact]
        public void GetCountries_ReturnsAllCountries()
        {
            IEnumerable<TypeEntity> mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetCountries()).Returns(mockEntities);

            var response = _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public void GetTeachingSubjects_ReturnsAllSubjects()
        {
            IEnumerable<TypeEntity> mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetTeachingSubjects()).Returns(mockEntities);

            var response = _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        private IEnumerable<TypeEntity> MockTypeEntities()
        {
            return new []
            {
                new TypeEntity { Id = Guid.NewGuid(), Value = "Type 1" },
                new TypeEntity { Id = Guid.NewGuid(), Value = "Type 2" },
            };
        }
    }
}
