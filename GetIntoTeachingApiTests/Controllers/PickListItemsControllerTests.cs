using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using Xunit;
using GetIntoTeachingApi.Attributes;

namespace GetIntoTeachingApiTests.Controllers
{
    public class PickListItemsControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly PickListItemsController _controller;

        public PickListItemsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _controller = new PickListItemsController(_mockStore.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(PickListItemsController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
        {
            typeof(PickListItemsController).Should().BeDecoratedWith<PrivateShortTermResponseCacheAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            var methods = typeof(PickListItemsController).GetMethods(BindingFlags.DeclaredOnly);

            methods.ForEach(m => m.Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public async void GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateMailingListSubscriptionChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_gitismlservicesubscriptionchannel")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateMailingListSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateEventSubscriptionChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_gitiseventsservicesubscriptionchannel")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateEventSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateGcseStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitehasgcseenglish")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateRetakeGcseStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateRetakeGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateConsiderationJourneyStages_ReturnsAllStages()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateConsiderationJourneyStages();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_typeofcandidate")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateAssignmentStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_candidatestatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAssignmentStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateAdviserEligibilities_ReturnsAllEligibilities()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_iscandidateeligibleforadviser")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserEligibilities();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetCandidateAdviserRequirements_ReturnsAllEligibilities()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_isadvisorrequiredos")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserRequirements();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetQualificationTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_type")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetQualificationUkDegreeGrades_ReturnsAllGrades()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationUkDegreeGrades();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetTeachingEventStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_eventstatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetTeachingEventRegistrationChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_eventregistration", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventRegistrationChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetPhoneCallChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("phonecall", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetPhoneCallChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async void GetSubscriptionTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_servicesubscription", "dfe_servicesubscriptiontype"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetSubscriptionTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        private static PickListItem[] MockPickListItems()
        {
            return new[]
            {
                new PickListItem {Id = 1, Value = "Type 2"},
                new PickListItem {Id = 2, Value = "Type 3"},
                new PickListItem {Id = 3, Value = "Type 1"},
            };
        }
    }
}
