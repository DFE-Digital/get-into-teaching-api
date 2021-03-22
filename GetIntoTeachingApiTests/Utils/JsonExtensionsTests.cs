using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Mocks;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void SerializeChangeTracked_IgnoresNullValues()
        {
            var model = new MockModel() { Id = Guid.NewGuid(), Field3 = null };

            var json = model.SerializeChangeTracked();

            json.Should().NotContain("field3");
            json.Should().Contain("id");
        }

        [Fact]
        public void SerializeChangeTracked_UsesCamelCaseAttributeNames()
        {
            var model = new MockModel() { Field3 = "test" };

            var json = model.SerializeChangeTracked();

            json.Should().Contain("\"field3\": \"test\"");
        }

        [Fact]
        public void DeserializeChangeTracked_WithCamelCaseAttributeNames_DeserializesCorrectly()
        {
            var model = "{\"field3\":\"test\"}".DeserializeChangeTracked<MockModel>();

            model.Field3.Should().Be("test");
        }

        [Fact]
        public void DeserializeChangeTracked_WithPascalCaseAttributeNames_DeserializesCorrectly()
        {
            // Ensures any jobs in the queue will still deserialize correctly
            // while we roll out the change to serializing with camelCase attribute names.
            var model = "{\"Field3\":\"test\"}".DeserializeChangeTracked<MockModel>();

            model.Field3.Should().Be("test");
        }

        [Fact]
        public void SerializeAndDeserializeChangeTracked_BehavesCorrectly()
        {
            // Ensures the JSON serializer correctly deserializes
            // ChangedPropertyNames (and doesn't inadvertently change it
            // during the deserialization process when writing to attributes).
            var model = new MockModel() { Id = Guid.NewGuid(), Field3 = "test" };

            model.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field3", "FieldDefinedWithValue" });

            // Test serializing/deseriaizing model.
            var json = model.SerializeChangeTracked();
            var deserializedModel = json.DeserializeChangeTracked<MockModel>();

            deserializedModel.Id.Should().Be(model.Id);
            deserializedModel.Field3.Should().Be(model.Field3);
            deserializedModel.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field3", "FieldDefinedWithValue" });

            // Test deserializing model with ChangedPropertyNames in different order/combinations.
            json = "{\"ChangedPropertyNames\":[\"Id\",\"Field1\"],\"Field3\":\"test\",\"Field2\":123}";
            deserializedModel = json.DeserializeChangeTracked<MockModel>();

            deserializedModel.Field2.Should().Be(123);
            deserializedModel.Field3.Should().Be("test");
            deserializedModel.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field1", "FieldDefinedWithValue" });

            json = "{\"Field3\":\"test\",\"Field2\":123,\"ChangedPropertyNames\":[\"Id\",\"Field1\"]}";
            deserializedModel = json.DeserializeChangeTracked<MockModel>();

            deserializedModel.Field2.Should().Be(123);
            deserializedModel.Field3.Should().Be("test");
            deserializedModel.ChangedPropertyNames.Should().BeEquivalentTo(new HashSet<string>() { "Id", "Field1", "FieldDefinedWithValue" });
        }
    }
}
