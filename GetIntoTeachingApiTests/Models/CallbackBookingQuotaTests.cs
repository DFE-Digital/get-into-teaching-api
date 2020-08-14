using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CallbackBookingQuotaTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CallbackBookingQuota);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_callbackbookingquota");

            type.GetProperty("TimeSlot").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_name");
            type.GetProperty("Day").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_workingdayname");
            type.GetProperty("StartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_starttime");
            type.GetProperty("EndAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_endtime");
            type.GetProperty("Quota").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitequota");
            type.GetProperty("NumberOfBookings").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitenumberofbookings");
        }

        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(0, 0, false)]
        [InlineData(2, 2, false)]
        [InlineData(3, 2, false)]
        public void IsAvailable_ReturnsCorrectly(int numberOfBookings, int quota, bool expected)
        {
            var callbackBookingQuota = new CallbackBookingQuota() { NumberOfBookings = numberOfBookings, Quota = quota };

            callbackBookingQuota.IsAvailable.Should().Be(expected);
        }
    }
}