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
        public void Loggable_IsPresent()
        {
            typeof(TeachingEvent).Should().BeDecoratedWith<LoggableAttribute>();
        }

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
            type.GetProperty("ProviderWebsiteUrl").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providerwebsite");
            type.GetProperty("ProviderTargetAudience").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providertargetaudience_ml");
            type.GetProperty("ProviderOrganiser").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providerorganiser");
            type.GetProperty("ProviderContactEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providercontactemailaddress");
            type.GetProperty("Message").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_miscellaneousmessage_ml");
            type.GetProperty("StartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventstartdate");
            type.GetProperty("EndAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventenddate");

            type.GetProperty("Building").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "msevtmgt_event_building" && a.Type == typeof(TeachingEventBuilding));
        }
    }
}
