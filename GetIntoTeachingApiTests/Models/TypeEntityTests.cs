using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TypeEntityTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity["dfe_name"] = "name";

            var typeEntity = new TypeEntity(entity);

            ((Guid)typeEntity.Id).Should().Be(entity.Id);
            (typeEntity.Value as string).Should().Be(entity.GetAttributeValue<string>("dfe_name"));
        }

        [Fact]
        public void Constructor_WithPickListItem_MapsCorrectly()
        {
            var pickListItem = new CdsServiceClient.PickListItem {PickListItemId = 123, DisplayLabel = "name"};

            var typeEntity = new TypeEntity(pickListItem);

            ((int)typeEntity.Id).Should().Be(pickListItem.PickListItemId);
            (typeEntity.Value as string).Should().Be(pickListItem.DisplayLabel);
        }
    }
}
