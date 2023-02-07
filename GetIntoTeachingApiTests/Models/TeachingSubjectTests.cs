using System;
using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingSubjectTests
    {
        private readonly Entity _entity;

        public TeachingSubjectTests()
        {
            _entity = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = "entityName"
            };

            _entity["dfe_name"] = "name";
        }

        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingSubject);

            type.GetProperty("Id").Should().BeDecoratedWith<DatabaseGeneratedAttribute>(
                a => a.DatabaseGeneratedOption == DatabaseGeneratedOption.None);
        }

        [Fact]
        public void Constructor_WithEntity()
        {
            var subject = new TeachingSubject(_entity);

            subject.Id.Should().Be(_entity.Id);
            subject.Value.Should().Be(_entity.GetAttributeValue<string>("dfe_name"));
        }
    }
}
