using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventSearchRequestTests
    {
        private readonly Mock<ILocationService> _mockLocationService;

        public TeachingEventSearchRequestTests()
        {
            _mockLocationService = new Mock<ILocationService>();
        }

        [Fact]
        public void Match_WithMatchingValues_ReturnsTrue()
        {
            var teachingEvent = new TeachingEvent()
            {
                StartAt = DateTime.Now.AddDays(1), 
                TypeId = 123, 
                Building = new TeachingEventBuilding() { AddressPostcode = "KY10 9DS" }
            };
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "CA4 8HF", 
                Radius = 20, 
                StartAfter = DateTime.Now, 
                StartBefore = DateTime.Now.AddDays(2), 
                TypeId = 123
            };
            _mockLocationService.Setup(mock => mock.DistanceBetween("CA4 8HF", "KY10 9DS")).Returns(19);

            request.Match(teachingEvent, _mockLocationService.Object).Should().BeTrue();
        }

        [Fact]
        public void Match_WithNullValues_ReturnsTrue()
        {
            new TeachingEventSearchRequest()
                .Match(new TeachingEvent(), _mockLocationService.Object).Should().BeTrue();
        }

        [Fact]
        public void Match_DifferentType_ReturnsFalse()
        {
            var teachingEvent = new TeachingEvent() {TypeId = 456};
            var request = new TeachingEventSearchRequest() {TypeId = 123};

            request.Match(teachingEvent, _mockLocationService.Object).Should().BeFalse();
        }

        [Fact]
        public void Match_StartAfterLaterThanStartAt_ReturnsFalse()
        {
            var teachingEvent = new TeachingEvent() { StartAt = DateTime.Now };
            var request = new TeachingEventSearchRequest() { StartAfter = DateTime.Now.AddDays(1) };

            request.Match(teachingEvent, _mockLocationService.Object).Should().BeFalse();
        }

        [Fact]
        public void Match_StartBeforeEarlierThanStartAt_ReturnsFalse()
        {
            var teachingEvent = new TeachingEvent() { StartAt = DateTime.Now };
            var request = new TeachingEventSearchRequest() { StartBefore = DateTime.Now.AddDays(-1) };

            request.Match(teachingEvent, _mockLocationService.Object).Should().BeFalse();
        }

        [Fact]
        public void Match_RadiusLessThanDistanceBetweenPostcodes_ReturnsFalse()
        {
            var teachingEvent = new TeachingEvent() { Building = new TeachingEventBuilding() { AddressPostcode = "KY10 9DS" } };
            var request = new TeachingEventSearchRequest() { Postcode = "CA4 8HF", Radius = 20};
            _mockLocationService.Setup(mock => mock.DistanceBetween("CA4 8HF", "KY10 9DS")).Returns(21);
            request.Match(teachingEvent, _mockLocationService.Object).Should().BeFalse();
        }
    }
}
