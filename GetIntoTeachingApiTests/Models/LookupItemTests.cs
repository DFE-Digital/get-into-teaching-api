using System;
using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class LookupItemTests
    {
        [Fact]
        public void Loggable_IsPresent()
        {
            typeof(LookupItem).Should().BeDecoratedWith<LoggableAttribute>();
        }

        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(LookupItem);

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

            var lookupItem = new LookupItem(entity, "entityName");

            lookupItem.Id.Should().Be(entity.Id);
            lookupItem.Value.Should().Be(entity.GetAttributeValue<string>("dfe_name"));
            lookupItem.EntityName.Should().Be("entityName");
        }
    }
}
