using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Reflection;
using GetIntoTeachingApi.Filters;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TypesControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly TypesController _controller;

        public TypesControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _controller = new TypesController(_mockStore.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(TypesController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            var methods = typeof(TypesController).GetMethods(BindingFlags.DeclaredOnly);

            methods.ForEach(m => m.Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public async void GetCountries_ReturnsAllCountries()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetLookupItems("dfe_country")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetTeachingSubjects_ReturnsAllSubjects()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01"))
                .Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_channelcreation")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateGcseStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_hasgcseenglish")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateRetakeGcseStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateRetakeGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateDescribeYourselfOptions_ReturnsAllOptions()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitedescribeyourself")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateDescribeYourselfOptions();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateConsiderationJourneyStages_ReturnsAllStages()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateConsiderationJourneyStages();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetQualificationUkDegreeGrades_ReturnsAllGrades()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetQualificationUkDegreeGrades();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetPhoneCallChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetPickListItems("phonecall", "dfe_channelcreation")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetPhoneCallChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        private static TypeEntity[] MockTypeEntities()
        {
            return new[]
            {
                new TypeEntity {Id = Guid.NewGuid().ToString(), Value = "Type 1"},
                new TypeEntity {Id = Guid.NewGuid().ToString(), Value = "Type 2"},
            };
        }
    }
}
