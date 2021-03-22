using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingEvent);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "msevtmgt_event");
            
            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_event_type" && a.Type == typeof(OptionSetValue));
            type.GetProperty("StatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_eventstatus" && a.Type == typeof(OptionSetValue));

            type.GetProperty("ReadableId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websiteeventpartialurl");
            type.GetProperty("WebFeedId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_eventwebfeedid");
            type.GetProperty("IsOnline").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_isonlineevent");
            type.GetProperty("Name").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_externaleventtitle");
            type.GetProperty("Description").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_description");
            type.GetProperty("Summary").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_eventsummary_ml");
            type.GetProperty("VideoUrl").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_videolink");
            type.GetProperty("ScribbleId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_scribbleurl");
            type.GetProperty("ProviderWebsiteUrl").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providerwebsite");
            type.GetProperty("ProviderTargetAudience").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providertargetaudience_ml");
            type.GetProperty("ProviderOrganiser").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providerorganiser");
            type.GetProperty("ProviderContactEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providercontactemailaddress");
            type.GetProperty("Message").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_miscellaneousmessage_ml");
            type.GetProperty("StartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventstartdate");
            type.GetProperty("EndAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventenddate");

            type.GetProperty("Building").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "msevtmgt_event_building" && a.Type == typeof(TeachingEventBuilding));
            type.GetProperty("BuildingId").Should().BeDecoratedWith<EntityFieldAttribute>(
               a => a.Name == "msevtmgt_building" && a.Type == typeof(EntityReference));
        }

        [Theory]
        [InlineData(true, "", false)]
        [InlineData(true, "  ", false)]
        [InlineData(true, "KY11 9YU", true)]
        [InlineData(true, null, false)]
        [InlineData(false, null, false)]
        [InlineData(false, "", false)]
        [InlineData(false, "  ", false)]
        [InlineData(false, "KY11 9YU", false)]
        public void IsVirtual_ReturnsCorrectly(bool isOnline, string addressPostcode, bool expected)
        {
            var teachingEvent = new TeachingEvent()
            {
                IsOnline = isOnline,
                Building = new TeachingEventBuilding() { AddressPostcode = addressPostcode },
            };

            teachingEvent.IsVirtual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        public void IsInPerson_ReturnsCorrectly(bool isOnline, bool isVirtual, bool expected)
        {
            var teachingEvent = new TeachingEvent()
            {
                IsOnline = isOnline,
            };

            if (isVirtual || !isOnline)
            {
                teachingEvent.Building = new TeachingEventBuilding() { AddressPostcode = "KY11 9YU" };
            }

            teachingEvent.IsInPerson.Should().Be(expected);
        }
    }
}
