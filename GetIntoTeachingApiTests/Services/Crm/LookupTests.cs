using FluentAssertions;
using GetIntoTeachingApi.Services.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Services.Crm
{
    public class LookupTests
    {
        [Fact]
        public void CacheKey_CompoundsEntityNameAndIdAttribute()
        {
            var lookup = new Lookup("entity_name", "id_attribute");
            lookup.CacheKey.Should().Be("entity_name-id_attribute");
        }
    }
}
