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
        public void PopulateEntity_ReverseMapsCorrectly()
        {
            var phoneCall = new PhoneCall() { ScheduledAt = new DateTime(2021, 2, 13, 10, 34, 12) };
            const string telephone = "07594 835 274";
            var entity = new Entity("phonecall");
            phoneCall.PopulateEntity(entity, telephone);

            entity.GetAttributeValue<string>("phonenumber").Should().Be(telephone);
            entity.GetAttributeValue<DateTime?>("scheduledstart").Should().Be(phoneCall.ScheduledAt);
        }
    }
}
