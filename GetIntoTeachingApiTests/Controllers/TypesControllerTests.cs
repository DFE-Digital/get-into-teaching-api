using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Services.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TypesControllerTests
    {
        private readonly Mock<IWebApiClient> _mockClient;
        private readonly TypesController _controller;

        public TypesControllerTests()
        {
            _mockClient = new Mock<IWebApiClient>();
            var mockLogger = new Mock<ILogger<TypesController>>();
            _controller = new TypesController(mockLogger.Object, _mockClient.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TypesController));
        }

        [Fact]
        public async void GetCountries_ReturnsAllCountries()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetLookupItems(Lookup.Country)).ReturnsAsync(mockEntities);

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetTeachingSubjects_ReturnsAllSubjects()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetLookupItems(Lookup.TeachingSubject))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.CandidateInitialTeacherTrainingYears))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.CandidatePreferredEducationPhases))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetCandidateLocations_ReturnsAllLocations()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.CandidateLocations))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetCandidateLocations();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.QualificationDegreeStatus))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetQualificationCategories_ReturnsAllCategories()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.QualificationCategories))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetQualificationCategories();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetQualificationTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.QualificationTypes))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetQualificationTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.PastTeachingPositionEducationPhases))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public async void GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockClient.Setup(mock => mock.GetOptionSetItems(OptionSet.TeachingEventTypes))
                .ReturnsAsync(mockEntities);

            var response = await _controller.GetTeachingEventTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        private static IEnumerable<TypeEntity> MockTypeEntities()
        {
            return new []
            {
                new TypeEntity { Id = Guid.NewGuid(), Value = "Type 1" },
                new TypeEntity { Id = Guid.NewGuid(), Value = "Type 2" },
            };
        }
    }
}
