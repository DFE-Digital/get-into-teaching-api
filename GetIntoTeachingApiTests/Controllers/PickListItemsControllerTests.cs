using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using GetIntoTeachingApi.Attributes;
using System.Threading.Tasks;

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
        public async Task GetCandidateInitialTeacherTrainingYears_ReturnsAllYears()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateInitialTeacherTrainingYears();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidatePreferredEducationPhases_ReturnsAllPhases()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidatePreferredEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateTeacherTrainingAdviserSubscriptionChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_gitisttaservicesubscriptionchannel")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateTeacherTrainingAdviserSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateMailingListSubscriptionChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_gitismlservicesubscriptionchannel")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateMailingListSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateEventSubscriptionChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_gitiseventsservicesubscriptionchannel")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateEventSubscriptionChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateGcseStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitehasgcseenglish")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateRetakeGcseStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateRetakeGcseStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateConsiderationJourneyStages_ReturnsAllStages()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateConsiderationJourneyStages();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_typeofcandidate")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateAssignmentStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_candidatestatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAssignmentStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateAdviserEligibilities_ReturnsAllEligibilities()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_iscandidateeligibleforadviser")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserEligibilities();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateAdviserRequirements_ReturnsAllEligibilities()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_isadvisorrequiredos")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateAdviserRequirements();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetQualificationDegreeStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationDegreeStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetQualificationTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_type")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetQualificationUkDegreeGrades_ReturnsAllGrades()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetQualificationUkDegreeGrades();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetPastTeachingPositionEducationPhases_ReturnsAllPhases()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetPastTeachingPositionEducationPhases();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetTeachingEventTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_event_type")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetTeachingEventRegions_ReturnsAllRegions()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_eventregion")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventRegions();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetTeachingEventStatus_ReturnsAllStatus()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_event", "dfe_eventstatus")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetTeachingEventRegistrationChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("msevtmgt_eventregistration", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingEventRegistrationChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetPhoneCallChannels_ReturnsAllChannels()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("phonecall", "dfe_channelcreation")).Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetPhoneCallChannels();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetSubscriptionTypes_ReturnsAllTypes()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("dfe_servicesubscription", "dfe_servicesubscriptiontype"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetSubscriptionTypes();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateSituations_ReturnsAllSituations()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_situation"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateSituations();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);
        }

        [Fact]
        public async Task GetCandidateCitizenship_ReturnsAllCitizenshipItems()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_citizenship"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateCitizenship();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);

            _mockStore.Verify(mock =>
                mock.GetPickListItems("contact", "dfe_citizenship"), Times.Once);
        }

        [Fact]
        public async Task GetCandidateVisaStatus_ReturnsAllVisaStatusItems()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock => mock.GetPickListItems("contact", "dfe_visastatus"))
                .Returns(mockItems.AsAsyncQueryable());

            var response = await _controller.GetCandidateVisaStatus();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);

            _mockStore.Verify(mock =>
                mock.GetPickListItems("contact", "dfe_visastatus"), Times.Once);
        }

        [Fact]
        public async Task GetCandidateLocations_ReturnsAllLocationItems()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock =>
                mock.GetPickListItems("contact", "dfe_location"))
                    .Returns(mockItems.AsAsyncQueryable()).Verifiable();

            IActionResult response = await _controller.GetCandidateLocation();

            OkObjectResult ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);

            _mockStore.Verify(mock =>
                mock.GetPickListItems("contact", "dfe_location"), Times.Once);
        }

        public async Task GetTeachingEventAccessibility_ReturnsAllAccessibilityItems()
        {
            var mockItems = MockPickListItems();
            _mockStore.Setup(mock =>
                mock.GetPickListItems("msevtmgt_event", "dfe_accessibility"))
                    .Returns(mockItems.AsAsyncQueryable()).Verifiable();

            IActionResult response = await _controller.GetTeachingEventAccessibilityItems();

            OkObjectResult ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockItems);

            _mockStore.Verify(mock =>
                mock.GetPickListItems("msevtmgt_event", "dfe_accessibility"), Times.Once);
        }

        private static PickListItem[] MockPickListItems()
        {
            return
            [
                new PickListItem {Id = 1, Value = "Type 2"},
                new PickListItem {Id = 2, Value = "Type 3"},
                new PickListItem {Id = 3, Value = "Type 1"},
            ];
        }
    }
}
