using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PickListItemTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(PickListItem);

            type.GetProperty("Id").Should().BeDecoratedWith<DatabaseGeneratedAttribute>(
                a => a.DatabaseGeneratedOption == DatabaseGeneratedOption.None);
        }

        [Fact]
        public void Constructor_WithPickListItem()
        {
            var pickListItem = new Microsoft.PowerPlatform.Dataverse.Client.Extensions.PickListItem { PickListItemId = 123, DisplayLabel = "name" };

            var typeEntity = new PickListItem(pickListItem, "entityName", "attributeName");

            typeEntity.Id.Should().Be(pickListItem.PickListItemId);
            typeEntity.Value.Should().Be(pickListItem.DisplayLabel);
            typeEntity.EntityName.Should().Be("entityName");
            typeEntity.AttributeName.Should().Be("attributeName");
        }
    }
}
