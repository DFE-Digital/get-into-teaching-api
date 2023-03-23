using System;
using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CountryTests
    {
        private readonly Entity _entity;

        public CountryTests()
        {
            _entity = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = "entityName"
            };

            _entity["dfe_name"] = "name";
            _entity["dfe_countrykey"] = "code";
        }

        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(Country);

            type.GetProperty("Id").Should().BeDecoratedWith<DatabaseGeneratedAttribute>(
                a => a.DatabaseGeneratedOption == DatabaseGeneratedOption.None);
        }

        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var country = new Country(_entity);

            country.Id.Should().Be(_entity.Id);
            country.Value.Should().Be(_entity.GetAttributeValue<string>("dfe_name"));
            country.IsoCode.Should().Be(_entity.GetAttributeValue<string>("dfe_countrykey"));
        }
    }
}
