using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingSubjectTests
    {
        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(TeachingSubject);

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_teachingsubjectlistid");
        }
    }
}