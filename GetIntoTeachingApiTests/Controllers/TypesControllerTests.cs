using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
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
        private readonly TypesController _controller;

        public TypesControllerTests()
        {
            _mockCrm = new Mock<ICrmService>();
            var mockLogger = new Mock<ILogger<TypesController>>();
            _controller = new TypesController(mockLogger.Object, _mockCrm.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TypesController));
        }

        [Fact]
        public void GetCountries_ReturnsAllCountries()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetLookupItems("dfe_country")).Returns(mockEntities);

            var response = _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetTeachingSubjects_ReturnsAllSubjects()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist")).Returns(mockEntities);

            var response = _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear")).Returns(mockEntities);

            var response = _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01")).Returns(mockEntities);

            var response = _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetCandidateLocations_ReturnsAllLocations()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("contact", "dfe_isinuk")).Returns(mockEntities);

            var response = _controller.GetCandidateLocations();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_degreestatus")).Returns(mockEntities);

            var response = _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetQualificationCategories_ReturnsAllCategories()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_category")).Returns(mockEntities);

            var response = _controller.GetQualificationCategories();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetQualificatioTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("dfe_qualification", "dfe_type")).Returns(mockEntities);

            var response = _controller.GetQualificationTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase")).Returns(mockEntities);

            var response = _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEntities);
        }

        [Fact]
        public void GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockCrm.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type")).Returns(mockEntities);

            var response = _controller.GetTeachingEventTypes();

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
