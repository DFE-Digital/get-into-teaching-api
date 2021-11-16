using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class TeachingEventTests
    {
        private readonly OrganizationServiceContext _context;
        private readonly Mock<TeachingEventBuilding> _mockBuilding;
        private readonly Mock<ICrmService> _mockCrm;

        public TeachingEventTests()
        {
            _context = new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
            _mockBuilding = new Mock<TeachingEventBuilding>();
            _mockCrm = new Mock<ICrmService>();
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
            type.GetProperty("InternalName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_name");
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
            type.GetProperty("ProvidersList").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_providerslist");
            type.GetProperty("BuildingId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_building" && a.Type == typeof(EntityReference) && a.Reference == "msevtmgt_building");

            type.GetProperty("Building").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "msevtmgt_event_building" && a.Type == typeof(TeachingEventBuilding));
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

        [Fact]
        public void InternalName_Set_ReturnsName()
        {
            var teachingEvent = new TeachingEvent() { Name = "name" };
            teachingEvent.InternalName.Should().Be(teachingEvent.Name);
        }

        [Fact]
        public void InternalTimeZone_Default_ReturnsGmtCode()
        {
            var teachingEvent = new TeachingEvent();
            teachingEvent.InternalTimeZone.Should().Be(85);
        }

        [Fact]
        public void ToEntity_WhenBuildingIsRemoved_DeletesLink()
        {
            _mockBuilding.Setup(mock => mock.ToEntity(It.IsAny<ICrmService>(), _context)).Returns(new Entity());
            _mockCrm.Setup(m => m.NewEntity("msevtmgt_event", _context)).Returns(new Entity());
            _mockCrm.Setup(mock => mock.GetTeachingEvent("readableId"))
                .Returns(new TeachingEvent
                {
                    ReadableId = "readableId",
                    Building = _mockBuilding.Object
                });

            var updatedEvent = new TeachingEvent
            {
                ReadableId = "readableId",
                Building = null
            };

            updatedEvent.ToEntity(_mockCrm.Object, _context);

            _mockCrm.Verify(m => m.DeleteLink(It.IsAny<Entity>(), It.IsAny<Relationship>(),
                It.IsAny<Entity>(), It.IsAny<OrganizationServiceContext>()), Times.Once);
        }

        [Fact]
        public void ToEntity_WhenEventIsNew_ReturnsEntity()
        {
            _mockBuilding.Setup(mock => mock.ToEntity(It.IsAny<ICrmService>(), _context)).Returns(new Entity());
            _mockCrm.Setup(m => m.NewEntity("msevtmgt_event", _context)).Returns(new Entity());
            _mockCrm.Setup(mock => mock.GetTeachingEvent("readableId"))
                .Returns<TeachingEvent>(null);

            var newEvent = new TeachingEvent
            {
                ReadableId = "readableId",
                Building = new TeachingEventBuilding()
            };

            var entity = newEvent.ToEntity(_mockCrm.Object, _context);

            entity.Should().BeOfType<Entity>();
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ToEntity_WhenBuildingIsNotRemoved_DoesNotDeleteLink()
        {
            _mockBuilding.Setup(mock => mock.ToEntity(It.IsAny<ICrmService>(), _context)).Returns(new Entity());
            _mockCrm.Setup(m => m.NewEntity("msevtmgt_event",  _context)).Returns(new Entity());
            _mockCrm.Setup(mock => mock.GetTeachingEvent("readableId"))
                .Returns(new TeachingEvent
                {
                    ReadableId = "readableId",
                    Building = new TeachingEventBuilding()
                });

            var updatedEvent = new TeachingEvent
            {
                ReadableId = "readableId",
                Building = new TeachingEventBuilding()
            };

            updatedEvent.ToEntity(_mockCrm.Object, _context);

            _mockCrm.Verify(m => m.DeleteLink(It.IsAny<Entity>(), It.IsAny<Relationship>(),
                It.IsAny<Entity>(), It.IsAny<OrganizationServiceContext>()), Times.Never);
        }

        [Fact]
        public void ToEntity_WhenThereIsNoPreexistingRelationship_DoesNotDeleteLink()
        {
            _mockBuilding.Setup(mock => mock.ToEntity(It.IsAny<ICrmService>(), _context)).Returns(new Entity());
            _mockCrm.Setup(m => m.NewEntity("msevtmgt_event", _context)).Returns(new Entity());
            _mockCrm.Setup(mock => mock.GetTeachingEvent("readableId"))
                .Returns(new TeachingEvent
                {
                    ReadableId = "readableId",
                });

            var updatedEvent = new TeachingEvent
            {
                ReadableId = "readableId",
                Building = null
            };

            updatedEvent.ToEntity(_mockCrm.Object, _context);

            _mockCrm.Verify(m => m.DeleteLink(It.IsAny<Entity>(), It.IsAny<Relationship>(),
                It.IsAny<Entity>(), It.IsAny<OrganizationServiceContext>()), Times.Never);
        }
    }
}
