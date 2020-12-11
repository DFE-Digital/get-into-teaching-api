using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Reflection;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using Xunit;
using GetIntoTeachingApi.Attributes;
using System.Collections.Generic;
using System.Linq;

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
        public void LogRequests_IsPresent()
        {
            typeof(TypesController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
        {
            typeof(TypesController).Should().BeDecoratedWith<PrivateShortTermResponseCacheAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            var methods = typeof(TypesController).GetMethods(BindingFlags.DeclaredOnly);

            methods.ForEach(m => m.Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public async void GetCountries_ReturnsAllCountriesSortedByCountryName()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_country", null)).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var countries = (IEnumerable<TypeEntity>)ok.Value;
            countries.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Type 1", "Type 2", "Type 3" });
        }

        [Fact]
        public async void GetTeachingSubjects_ReturnsAllSubjectsSortedBySubjectName()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_teachingsubjectlist", null)).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var subjects = (IEnumerable<TypeEntity>)ok.Value;
            subjects.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Type 1", "Type 2", "Type 3" });
        }

        [Fact]
        public async void GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_ittyear")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_preferrededucationphase01"))
                .Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_channelcreation")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateMailingListSubscriptionChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_gitismlservicesubscriptionchannel")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateMailingListSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateEventSubscriptionChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_gitiseventsservicesubscriptionchannel")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateEventSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateGcseStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_websitehasgcseenglish")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateRetakeGcseStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_websiteplanningretakeenglishgcse")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateRetakeGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateConsiderationJourneyStages_ReturnsAllStages()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_websitewhereinconsiderationjourney")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateConsiderationJourneyStages();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_typeofcandidate")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateAssignmentStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_candidatestatus")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateAssignmentStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateAdviserEligibilities_ReturnsAllEligibilities()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_iscandidateeligibleforadviser")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserEligibilities();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetCandidateAdviserRequirements_ReturnsAllEligibilities()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("contact", "dfe_isadvisorrequiredos")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserRequirements();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_candidatequalification", "dfe_degreestatus")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetQualificationTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_candidatequalification", "dfe_type")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetQualificationTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetQualificationUkDegreeGrades_ReturnsAllGrades()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_candidatequalification", "dfe_ukdegreegrade")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetQualificationUkDegreeGrades();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("msevtmgt_event", "dfe_event_type")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetTeachingEventStatus_ReturnsAllStatus()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("msevtmgt_event", "dfe_eventstatus")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetTeachingEventRegistrationChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("msevtmgt_eventregistration", "dfe_channelcreation")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventRegistrationChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetPhoneCallChannels_ReturnsAllChannels()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("phonecall", "dfe_channelcreation")).Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetPhoneCallChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        [Fact]
        public async void GetSubscriptionTypes_ReturnsAllTypes()
        {
            var mockEntities = MockTypeEntities();
            _mockStore.Setup(mock => mock.GetTypeEntitites("dfe_servicesubscription", "dfe_servicesubscriptiontype"))
                .Returns(mockEntities.AsAsyncQueryable());

            var response = await _controller.GetSubscriptionTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEntities);
        }

        private static TypeEntity[] MockTypeEntities()
        {
            return new[]
            {
                new TypeEntity {Id = Guid.NewGuid().ToString(), Value = "Type 2"},
                new TypeEntity {Id = Guid.NewGuid().ToString(), Value = "Type 3"},
                new TypeEntity {Id = Guid.NewGuid().ToString(), Value = "Type 1"},
            };
        }
    }
}
