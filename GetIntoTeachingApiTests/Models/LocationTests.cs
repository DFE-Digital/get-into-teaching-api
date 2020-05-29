using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class LocationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(Location);

            type.GetProperty("Postcode").Should().BeDecoratedWith<KeyAttribute>();
        }
    }
}
