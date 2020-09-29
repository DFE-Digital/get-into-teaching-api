using System;
using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TypeEntityTests
    {
        [Fact]
        public void Loggable_IsPresent()
        {
            typeof(TypeEntity).Should().BeDecoratedWith<LoggableAttribute>();
        }

        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(BaseModel);

            type.GetProperty("Id").Should().BeDecoratedWith<DatabaseGeneratedAttribute>(
                a => a.DatabaseGeneratedOption == DatabaseGeneratedOption.None);
        }

        [Fact]
        public void Constructor_WithEntity()
        {
            var entity = new Entity
            {
                Id = Guid.NewGuid()
            };
            entity["dfe_name"] = "name";

            var typeEntity = new TypeEntity(entity, "entityName");

            typeEntity.Id.Should().Be(entity.Id.ToString());
            typeEntity.Value.Should().Be(entity.GetAttributeValue<string>("dfe_name"));
            typeEntity.EntityName.Should().Be("entityName");
        }

        [Fact]
        public void Constructor_WithPickListItem()
        {
            var pickListItem = new CdsServiceClient.PickListItem { PickListItemId = 123, DisplayLabel = "name" };

            var typeEntity = new TypeEntity(pickListItem, "entityName", "attributeName");

            typeEntity.Id.Should().Be(pickListItem.PickListItemId.ToString());
            typeEntity.Value.Should().Be(pickListItem.DisplayLabel);
            typeEntity.EntityName.Should().Be("entityName");
            typeEntity.AttributeName.Should().Be("attributeName");
        }
    }
}
