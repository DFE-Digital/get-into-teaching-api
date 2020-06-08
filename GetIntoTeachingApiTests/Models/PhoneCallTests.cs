using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PhoneCallTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(PhoneCall);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "phonecall");

            type.GetProperty("ChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_channelcreation" && a.Type == typeof(OptionSetValue));
            
            type.GetProperty("ScheduledAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "scheduledstart");
            type.GetProperty("Telephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "phonenumber");
        }
    }
}
