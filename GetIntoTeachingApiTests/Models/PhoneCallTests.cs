using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PhoneCallTests
    {
        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var phoneCall = new PhoneCall() { ScheduledAt = new DateTime(2021, 2, 13, 10, 34, 12), Telephone = "07594 835 274" };
            var entity = new Entity("phonecall");
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var mockContext = mockService.Object.Context("mock-connection-string");
            mockService.Setup(mock => mock.NewEntity("phonecall",
                It.IsAny<OrganizationServiceContext>())).Returns(entity);
            phoneCall.ToEntity(mockService.Object, mockContext);

            entity.GetAttributeValue<string>("phonenumber").Should().Be(phoneCall.Telephone);
            entity.GetAttributeValue<DateTime?>("scheduledstart").Should().Be(phoneCall.ScheduledAt);
        }
    }
}
