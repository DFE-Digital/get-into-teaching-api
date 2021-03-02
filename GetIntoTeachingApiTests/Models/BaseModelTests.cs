using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Mocks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class BaseModelTests
    {
        private readonly Mock<IOrganizationServiceAdapter> _mockService;
        private readonly Mock<IValidatorFactory> _mockValidatorFactory;
        private readonly CrmService _crm;
        private readonly OrganizationServiceContext _context;

        public BaseModelTests()
        {
            _mockValidatorFactory = new Mock<IValidatorFactory>();
            _mockValidatorFactory.Setup(m => m.GetValidator(It.IsAny<Type>())).Returns<IValidator>(null);
            _mockService = new Mock<IOrganizationServiceAdapter>();
            _context = _mockService.Object.Context();
            _crm = new CrmService(_mockService.Object, _mockValidatorFactory.Object);
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithAttribute_ReturnsAttribute()
        {
            var property = typeof(MockModel).GetProperty("Field1");
            BaseModel.EntityFieldAttribute(property).Should().BeOfType<EntityFieldAttribute>();
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithIgnoreInEnvironmentAttribute_ReturnsNull()
        {
            var property = typeof(MockModel).GetProperty("Field4");
            BaseModel.EntityFieldAttribute(property).Should().BeNull();
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithoutAttribute_ReturnsNull()
        {
            var property = typeof(MockModel).GetProperty("RelatedMock");
            BaseModel.EntityFieldAttribute(property).Should().BeNull();
        }

        [Fact]
        public void EntityRelationshipAttribute_OnPropertyWithAttribute_ReturnsAttribute()
        {
            var property = typeof(MockModel).GetProperty("RelatedMock");
            BaseModel.EntityRelationshipAttribute(property).Should().BeOfType<EntityRelationshipAttribute>();
        }

        [Fact]
        public void EntityRelationshipAttribute_OnPropertyWithoutAttribute_ReturnsNull()
        {
            var property = typeof(MockModel).GetProperty("Field1");
            BaseModel.EntityRelationshipAttribute(property).Should().BeNull();
        }

        [Fact]
        public void LogicalName_ReturnsCorrectly()
        {
            BaseModel.LogicalName(typeof(MockModel)).Should().Be("mock");
        }

        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(BaseModel);

            type.GetProperty("Id").Should().BeDecoratedWith<DatabaseGeneratedAttribute>(
                a => a.DatabaseGeneratedOption == DatabaseGeneratedOption.None);
        }

        [Fact]
        public void AllSubTypes_HaveRequiredConstructor()
        {
            var assembly = typeof(BaseModel).Assembly;
            var subTypes = assembly.GetTypes().Where(t => t.BaseType == typeof(BaseModel));

            foreach (var subType in subTypes)
            {
                // Constructor is required for creating related models in BaseModel.MapRelationshipAttributesFromEntity
                var requiredConstructor = subType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                    new Type[] { typeof(Entity), typeof(ICrmService), typeof(IValidatorFactory) }, null);
                requiredConstructor.Should().NotBeNull();
                // Instantiate to include in coverage reports
                var entity = new Entity();
                Activator.CreateInstance(subType, entity, new Mock<ICrmService>().Object, _mockValidatorFactory.Object);
            }
        }

        [Fact]
        public void ChangedPropertyNames_UpdatesAsExpected()
        {
            var model = new MockModel();

            // Contains fields defined with a value on init.
            model.ChangedPropertyNames.Should().ContainSingle("FieldDefinedWithValue");

            // Null to value.
            model.Field3 = "test";
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3" });

            // Value to null.
            model.Field3 = null;
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3" });

            // Second property change.
            model.Field2 = 0;
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3", "Field2" });

            // Computed attributes.
            model.Field4 = "test";
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3", "Field2", "Field4", "CompoundField" });

            // Not changed to null.
            model.Field1 = null;
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3", "Field2", "Field4", "CompoundField", "Field1" });

            // Init with changes.
            model = new MockModel() { Field3 = "test", Field2 = 0 };
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "FieldDefinedWithValue", "Field3", "Field2" });
        }

        [Fact]
        public void ChangedPropertyNames_DuringDeserialization_IsNotAltered()
        {
            // Ensures the JSON serializer correctly deserializes
            // ChangedPropertyNames (and doesn't inadvertently change it
            // during the deserialization process when writing to attributes).
            var model = new MockModel() { Id = Guid.NewGuid(), Field3 = "test" };

            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field3", "FieldDefinedWithValue" });

            // Test using System.Text.Json as this is the app default (works correctly out of the box with tracking enabled).
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var deserializedModel = System.Text.Json.JsonSerializer.Deserialize<MockModel>(json);

            deserializedModel.Id.Should().Be(model.Id);
            deserializedModel.Field3.Should().Be(model.Field3);
            deserializedModel.Field4.Should().Be(model.Field4);
            deserializedModel.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field3", "FieldDefinedWithValue" });
        }

        [Fact]
        public void Constructor_WithEntity()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field1"] = new EntityReference { Id = Guid.NewGuid() };
            entity["dfe_field2"] = new OptionSetValue { Value = 1 };
            entity["dfe_field3"] = "field3";

            var relatedEntity1 = new Entity("relatedMock") { Id = Guid.NewGuid() };
            var relatedEntity2 = new Entity("relatedMock") { Id = Guid.NewGuid() };

            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mock"))
                .Returns(new List<Entity> { relatedEntity1 });
            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mocks"))
                .Returns(new List<Entity> { relatedEntity2 });

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

            mock.Id.Should().Be(entity.Id);
            mock.Field1.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_field1").Id);
            mock.Field2.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_field2").Value);
            mock.Field3.Should().Be(entity.GetAttributeValue<string>("dfe_field3"));

            mock.RelatedMock.Id.Should().Be(relatedEntity1.Id);
            mock.RelatedMocks.First().Id.Should().Be(relatedEntity2.Id);
        }

        [Fact]
        public void Constructor_WithEntityThatHasUntrimmedAndEmptyStrings_TrimsAndNullifiesStrings()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field3"] = "   a field3\n\rgoes here\n\r  ";
            entity["dfe_field4"] = "  ";

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

            mock.Field3.Should().Be("a field3\n\rgoes here");
            mock.Field4.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEntityThatHasNullStrings_MapsCorrectly()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field3"] = null;

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

            mock.Field3.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEntityThatHasAnInvalidFieldAttributeValue_NullifiesValue()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field2"] = new OptionSetValue { Value = 123 };
            entity["dfe_field3"] = "an-invalid-value";

            var mockValidator = new Mock<IValidator>();
            var validationResult = new ValidationResult();
            _mockValidatorFactory.Setup(m => m.GetValidator(It.IsAny<Type>())).Returns(mockValidator.Object);
            mockValidator.Setup(m => m.Validate(It.IsAny<BaseModel>())).Returns(validationResult);
            validationResult.Errors.Add(new ValidationFailure("Field3", "this value is not valid!"));

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

            mock.Field2.Should().Be(123);
            mock.Field3.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEntityThatHasAnInvalidRelationshipAttributeValue_DoesNotNullifyValue()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            var relatedEntity = new Entity("relatedMock") { Id = Guid.NewGuid() };

            _mockService.Setup(m => m.RelatedEntities(entity, "dfe_mock_dfe_relatedmock_mock"))
                .Returns(new List<Entity> { relatedEntity });

            var mockValidator = new Mock<IValidator>();
            var validationResult = new ValidationResult();
            _mockValidatorFactory.Setup(m => m.GetValidator(It.IsAny<Type>())).Returns(mockValidator.Object);
            mockValidator.Setup(m => m.Validate(It.IsAny<BaseModel>())).Returns(validationResult);
            validationResult.Errors.Add(new ValidationFailure("RelatedMock", "this value is not valid!"));

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

            mock.Id.Should().Be(entity.Id);
            mock.RelatedMock.Id.Should().Be(relatedEntity.Id);
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

            var mock = new MockModel(entity, _crm, _mockValidatorFactory.Object);

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

            mock.ToEntity(_crm, _context);

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

            mock.ToEntity(_crm, _context);

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
        public void ToEntity_WithNewThatHasExistingRelationships()
        {
            var mock = new MockModel()
            {
                RelatedMock = new MockRelatedModel(),
            };

            var mockEntity = new Entity("mock");
            var relatedMockEntity = new Entity("mock");

            _mockService.Setup(m => m.NewEntity("mock", _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", _context)).Returns(relatedMockEntity);

            mock.ToEntity(_crm, _context);

            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mock"),
                relatedMockEntity, _context), Times.Never);
        }

        [Fact]
        public void ToEntity_WritesNullProperties()
        {
            var mock = new MockModel()
            {
                Id = Guid.NewGuid(),
                Field1 = null,
                Field2 = null,
                Field3 = null,
            };

            var mockEntity = new Entity("mock", (Guid)mock.Id);

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);

            mock.ToEntity(_crm, _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Should().BeNull();
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Should().BeNull();
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().BeNull();

            var numberOfChangedAttributes = mockEntity.Attributes.Count();
            numberOfChangedAttributes.Should().Be(3);
        }

        [Fact]
        public void ToEntity_DoesNotWriteUnchangedProperties()
        {
            var mock = new MockModel() { Id = Guid.NewGuid(), Field1 = null, Field3 = "new value" };

            var mockEntity = new Entity("mock", (Guid)mock.Id);

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);

            mock.ToEntity(_crm, _context);

            var numberOfChangedAttributes = mockEntity.Attributes.Count();
            numberOfChangedAttributes.Should().Be(2);
        }

        [Fact]
        public void ToEntity_DoesNotWriteNullRelationships()
        {
            var mock = new MockModel() { Id = Guid.NewGuid() };

            var mockEntity = new Entity("mock", (Guid)mock.Id);

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", _context)).Returns<Entity>(null);

            mock.ToEntity(_crm, _context);

            _mockService.Verify(m => m.AddLink(mockEntity, It.IsAny<Relationship>(), It.IsAny<Entity>(), _context), Times.Never());
        }

        [Fact]
        public void ToEntity_WhenRelatedModelToEntityReturnsNull_DoesNotWriteNullRelationship()
        {
            var relatedMock = new Mock<MockRelatedModel>();
            var mock = new MockModel()
            {
                RelatedMock = relatedMock.Object,
            };

            var mockEntity = new Entity("mock");
            var relatedMockEntity = new Entity("mock");

            _mockService.Setup(m => m.NewEntity("mock", _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", _context)).Returns(relatedMockEntity);
            relatedMock.Setup(m => m.ToEntity(_crm, _context)).Returns<Entity>(null);

            mock.ToEntity(_crm, _context);

            _mockService.Verify(m => m.AddLink(mockEntity, new Relationship("dfe_mock_dfe_relatedmock_mock"),
                relatedMockEntity, _context), Times.Never);
        }

        [Fact]
        public void EntityFieldAttributeNames_ReturnsAllNames()
        {
            var names = BaseModel.EntityFieldAttributeNames(typeof(MockModel));
            names.Should().BeEquivalentTo(new[] { "dfe_field1", "dfe_field2", "dfe_field3", "mockid" });
        }
    }
}
