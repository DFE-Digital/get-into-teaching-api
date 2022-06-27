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
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class BaseModelTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IValidator<MockModel>> _mockValidator;
        private readonly Mock<IOrganizationServiceAdapter> _mockService;
        private readonly Mock<ILogger<ICrmService>> _mockLogger;
        private readonly CrmService _crm;
        private readonly OrganizationServiceContext _context;

        public BaseModelTests()
        {
            _mockValidator = new Mock<IValidator<MockModel>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(m => m.GetService(It.IsAny<Type>())).Returns<IValidator>(null);

            var mockAppSettings = new Mock<IAppSettings>();
            _mockService = new Mock<IOrganizationServiceAdapter>();
            _mockLogger = new Mock<ILogger<ICrmService>>();
            _context = _mockService.Object.Context();
            _crm = new CrmService(_mockService.Object, mockAppSettings.Object, new DateTimeProvider(), _mockLogger.Object, _mockServiceProvider.Object);
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithAttribute_ReturnsAttribute()
        {
            var property = typeof(MockModel).GetProperty("Field1");
            BaseModel.EntityFieldAttribute(property).Should().BeOfType<EntityFieldAttribute>();
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithFeatureFlag_ReturnsNullIfFeatureFlagIsOff()
        {
            var previous = Environment.GetEnvironmentVariable("TEST");
            Environment.SetEnvironmentVariable("TEST_FEATURE", "off");

            var property = typeof(MockModel).GetProperty("Field4");
            BaseModel.EntityFieldAttribute(property).Should().BeNull();

            Environment.SetEnvironmentVariable("TEST_FEATURE", previous);
        }

        [Fact]
        public void EntityFieldAttribute_OnPropertyWithFeatureFlag_ReturnsAttributeIfFeatureFlagIsOn()
        {
            var previous = Environment.GetEnvironmentVariable("TEST");
            Environment.SetEnvironmentVariable("TEST_FEATURE", "on");

            var property = typeof(MockModel).GetProperty("Field4");
            BaseModel.EntityFieldAttribute(property).Should().BeOfType<EntityFieldAttribute>();

            Environment.SetEnvironmentVariable("TEST_FEATURE", previous);
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
                    new Type[] { typeof(Entity), typeof(ICrmService), typeof(IValidator) }, null);
                requiredConstructor.Should().NotBeNull();
                // Instantiate to include in coverage reports
                var entity = new Entity();
                var mockValidator = new Mock<IValidator>();
                Activator.CreateInstance(subType, entity, new Mock<ICrmService>().Object, mockValidator.Object);
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
        public void DisableEnableChangeTracking_WorksCorrectly()
        {
            var model = new MockModel() { Id = Guid.NewGuid() };

            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "FieldDefinedWithValue" });

            model.DisableChangeTracking();
            model.Field4 = "test";
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "FieldDefinedWithValue" });

            model.EnableChangeTracking();
            model.Field2 = 123;
            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "FieldDefinedWithValue", "Field2" });
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

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

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

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

            mock.Field3.Should().Be("a field3\n\rgoes here");
            mock.Field4.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEntityThatHasNullStrings_MapsCorrectly()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field3"] = null;

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

            mock.Field3.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEntityThatHasAnInvalidFieldAttributeValue_NullifiesValue()
        {
            var entity = new Entity("mock") { Id = Guid.NewGuid() };
            entity["dfe_field2"] = new OptionSetValue { Value = 123 };
            entity["dfe_field3"] = "an-invalid-value";

            var validationResult = new ValidationResult();
            _mockValidator.Setup(m => m.Validate(It.IsAny<ValidationContext<BaseModel>>())).Returns(validationResult);
            validationResult.Errors.Add(new ValidationFailure("Field3", "this value is not valid!"));

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

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

            var validationResult = new ValidationResult();
            _mockValidator.Setup(m => m.Validate(It.IsAny<ValidationContext<BaseModel>>())).Returns(validationResult);
            validationResult.Errors.Add(new ValidationFailure("RelatedMock", "this value is not valid!"));

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

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

            var mock = new MockModel(entity, _crm, _mockValidator.Object);

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
            };

            var mockEntity = new Entity("mock");

            _mockService.Setup(m => m.BlankExistingEntity("mock", (Guid)mock.Id, _context))
                .Returns(mockEntity);

            mock.ToEntity(_crm, _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Id.Should().Be((Guid)mock.Field1);
            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").LogicalName.Should().Be("dfe_list");
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Value.Should().Be(mock.Field2);
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().Be(mock.Field3);
        }

        [Fact]
        public void ToEntity_WithNew()
        {
            var mock = new MockModel()
            {
                Field1 = Guid.NewGuid(),
                Field2 = 1,
                Field3 = "field3",
            };

            var mockEntity = new Entity("mock");

            _mockService.Setup(m => m.NewEntity("mock", null, _context)).Returns(mockEntity);

            mock.ToEntity(_crm, _context);

            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").Id.Should().Be((Guid)mock.Field1);
            mockEntity.GetAttributeValue<EntityReference>("dfe_field1").LogicalName.Should().Be("dfe_list");
            mockEntity.GetAttributeValue<OptionSetValue>("dfe_field2").Value.Should().Be(mock.Field2);
            mockEntity.GetAttributeValue<string>("dfe_field3").Should().Be(mock.Field3);
        }

        [Fact]
        public void ToEntity_WhenIdIsGeneratedUpfront_CallsNewEntityWithId()
        {
            var mock = new MockModel();

            mock.GenerateUpfrontId();
            mock.ToEntity(_crm, _context);

            _mockService.Verify(m => m.NewEntity("mock", mock.Id, _context));
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

            var numberOfChangedAttributes = mockEntity.Attributes.Count;
            numberOfChangedAttributes.Should().Be(3);
        }

        [Fact]
        public void ToEntity_DoesNotWriteUnchangedProperties()
        {
            var mock = new MockModel() { Id = Guid.NewGuid(), Field1 = null, Field3 = "new value" };

            var mockEntity = new Entity("mock", (Guid)mock.Id);

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);

            mock.ToEntity(_crm, _context);

            var numberOfChangedAttributes = mockEntity.Attributes.Count;
            numberOfChangedAttributes.Should().Be(2);
        }

        [Fact]
        public void ToEntity_DoesNotWriteNullRelationships()
        {
            var mock = new MockModel() { Id = Guid.NewGuid() };

            var mockEntity = new Entity("mock", (Guid)mock.Id);

            _mockService.Setup(m => m.BlankExistingEntity("mock", mockEntity.Id, _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", null, _context)).Returns<Entity>(null);

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

            _mockService.Setup(m => m.NewEntity("mock", null, _context)).Returns(mockEntity);
            _mockService.Setup(m => m.NewEntity("relatedMock", null, _context)).Returns(relatedMockEntity);
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

        [Fact]
        public void HasUpfrontId_IsDecoratedWithJsonPropertyAttribute()
        {
            // Decorating properties that have private setters with the JsonProperty
            // attribute allows them to be deserialized by Newtonsoft.Json 
            typeof(BaseModel).GetProperty("HasUpfrontId").Should().BeDecoratedWith<JsonPropertyAttribute>();
        }

        [Fact]
        public void GenerateUpfrontId_WhenAlreadyHasAnId_ThrowsException()
        {
            var mock = new MockModel { Id = Guid.NewGuid() };

            Action action = () => mock.GenerateUpfrontId();

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Can only generate IDs on models that do not have a preexisting ID.");
        }

        [Fact]
        public void GenerateUpfrontId_WhenNoPreexistingId_GeneratesIdAndSetsHasUpfrontIdFlagToTrue()
        {
            var mock = new MockModel();
            mock.Id.Should().BeNull();
            mock.HasUpfrontId.Should().Be(false);

            mock.GenerateUpfrontId();

            mock.Id.Should().NotBeNull();
            mock.HasUpfrontId.Should().Be(true);
        }
    }
}
