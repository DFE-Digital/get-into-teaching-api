using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    [Entity(LogicalName = "mock")]
    internal class MockModel : BaseModel
    {
        [EntityField(Name = "dfe_field1", Type = typeof(EntityReference), Reference = "dfe_list")]
        public Guid? Field1 { get; set; }
        [EntityField(Name = "dfe_field2", Type = typeof(OptionSetValue))]
        public int? Field2 { get; set; }
        [EntityField(Name = "dfe_field3")]
        public string Field3 { get; set; }
        [EntityRelationship(Name = "dfe_mock_dfe_relatedmock_mock", Type = typeof(MockRelatedModel))]
        public MockRelatedModel RelatedMock { get; set; }
        [EntityRelationship(Name = "dfe_mock_dfe_relatedmock_mocks", Type = typeof(MockRelatedModel))]
        public IEnumerable<MockRelatedModel> RelatedMocks { get; set; }

        public MockModel() : base() { }

        public MockModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }

    [Entity(LogicalName = "relatedMock")]
    internal class MockRelatedModel : BaseModel
    {
        public MockRelatedModel() : base() { }

        public MockRelatedModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }

    public class BaseModelTests
    {
        private readonly Mock<IOrganizationServiceAdapter> _mockService;
        private readonly OrganizationServiceContext _context;

        public BaseModelTests()
        {
            _mockService = new Mock<IOrganizationServiceAdapter>();
            _context = _mockService.Object.Context("mock-connection-string");
        }

        [Fact]
        public void Constructor_WithEntity()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field1"] = new EntityReference { Id = Guid.NewGuid() };
            entity["dfe_field2"] = new OptionSetValue { Value = 1 };
            entity["dfe_field3"] = "field3";

            var relatedEntity1 = new Entity("relatedMock") { Id = Guid.NewGuid() };
            entity["dfe_mock_dfe_relatedmock_mock"] = relatedEntity1;

            var relatedEntity2 = new Entity("relatedMock") { Id = Guid.NewGuid() };
            entity["dfe_mock_dfe_relatedmock_mocks"] = new List<Entity>() { relatedEntity2 };

            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mock"))
                .Returns(new List<Entity> { relatedEntity1 });
            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mocks"))
                .Returns(new List<Entity> { relatedEntity2 });

            var mock = new MockModel(entity, new CrmService(_mockService.Object));

            mock.Id.Should().Be(entity.Id);
            mock.Field1.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_field1").Id);
            mock.Field2.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_field2").Value);
            mock.Field3.Should().Be(entity.GetAttributeValue<string>("dfe_field3"));

            mock.RelatedMock.Id.Should().Be(relatedEntity1.Id);
            mock.RelatedMocks.First().Id.Should().Be(relatedEntity2.Id);
        }

        [Fact]
        public void Constructor_WithEntityThatHasNoLoadedRelationships()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field1"] = new EntityReference { Id = Guid.NewGuid() };
            entity["dfe_field2"] = new OptionSetValue { Value = 1 };
            entity["dfe_field3"] = "field3";

            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mock")).Returns(new List<Entity>());
            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mocks")).Returns(new List<Entity>());

            var mock = new MockModel(entity, new CrmService(_mockService.Object));

            mock.Id.Should().Be(entity.Id);
            mock.Field1.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_field1").Id);
            mock.Field2.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_field2").Value);
            mock.Field3.Should().Be(entity.GetAttributeValue<string>("dfe_field3"));

            mock.RelatedMock.Should().BeNull();
            mock.RelatedMocks.Should().BeNull();
        }

        [Fact]
        public void ToEntity_WithExisting()
        {
            var mock = new MockModel()
            {
                Id = Guid.NewGuid(),
                Field1 = Guid.NewGuid(),
                Field2 = 1,
                Field3 = "field3",
                RelatedMock = new MockRelatedModel() { Id = Guid.NewGuid() },
                RelatedMocks = new List<MockRelatedModel>() { new MockRelatedModel() { Id = Guid.NewGuid() } },
            };

            var mockEntity = new Entity("mock");
            var relatedMockEntity = new Entity("mock") { EntityState = EntityState.Created };

            _mockService.Setup(m => m.BlankExistingEntity("mock", (Guid)mock.Id, _context))
                .Returns(mockEntity);
            _mockService.Setup(m => m.BlankExistingEntity("relatedMock", 
                (Guid)mock.RelatedMock.Id, _context)).Returns(relatedMockEntity);
            _mockService.Setup(m => m.BlankExistingEntity("relatedMock", 
                (Guid)mock.RelatedMocks.First().Id, _context)).Returns(relatedMockEntity);

            mock.ToEntity(new CrmService(_mockService.Object), _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Id.Should().Be((Guid)mock.Field1);
            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").LogicalName.Should().Be("dfe_list");
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Value.Should().Be(mock.Field2); 
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().Be(mock.Field3);

            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mock"), 
                relatedMockEntity, _context));
            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mocks"), 
                relatedMockEntity, _context));
        }

        [Fact]
        public void ToEntity_WithNew()
        {
            var mock = new MockModel()
            {
                Field1 = Guid.NewGuid(),
                Field2 = 1,
                Field3 = "field3",
                RelatedMock = new MockRelatedModel(),
                RelatedMocks = new List<MockRelatedModel>() { new MockRelatedModel() },
            };

            var mockEntity = new Entity("mock");
            var relatedMockEntity = new Entity("mock") { EntityState = EntityState.Created };

            _mockService.Setup(m => m.NewEntity("mock", _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", _context)).Returns(relatedMockEntity);

            mock.ToEntity(new CrmService(_mockService.Object), _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Id.Should().Be((Guid)mock.Field1);
            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").LogicalName.Should().Be("dfe_list");
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Value.Should().Be(mock.Field2);
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().Be(mock.Field3);

            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mock"),
                relatedMockEntity, _context));
            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mocks"), 
                relatedMockEntity, _context));
        }

        [Fact]
        public void ToEntity_WithNullProperties()
        {
            var mock = new MockModel()
            {
                Id = Guid.NewGuid(),
                Field1 = Guid.NewGuid(),
                Field2 = 1,
                Field3 = null,
                RelatedMock = null,
                RelatedMocks = null,
            };

            var mockEntity = new Entity("mock", (Guid)mock.Id);
            var relatedMockEntity = new Entity("mock") { EntityState = EntityState.Created };

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", _context)).Returns(relatedMockEntity);

            mock.ToEntity(new CrmService(_mockService.Object), _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Id.Should().Be((Guid)mock.Field1);
            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").LogicalName.Should().Be("dfe_list");
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Value.Should().Be(mock.Field2);
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().Be(mock.Field3);

            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mock"), 
                relatedMockEntity, _context), Times.Never());
            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mocks"), 
                relatedMockEntity, _context), Times.Never());
        }
    }
}
