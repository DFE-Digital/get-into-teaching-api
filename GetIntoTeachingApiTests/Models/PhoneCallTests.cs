using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
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
            phoneCall.ToEntity(entity);

            entity.GetAttributeValue<string>("phonenumber").Should().Be(phoneCall.Telephone);
            entity.GetAttributeValue<DateTime?>("scheduledstart").Should().Be(phoneCall.ScheduledAt);
        }
    }
}
