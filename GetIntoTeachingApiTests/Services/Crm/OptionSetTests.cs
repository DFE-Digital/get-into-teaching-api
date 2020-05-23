using FluentAssertions;
using GetIntoTeachingApi.Services.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Services.Crm
{
    public class OptionSetTests
    {
        [Fact]
        public void CacheKey_CompoundsEntityNameAndIdAttribute()
        {
            var lookup = new OptionSet("entity_metadata_id", "attribute_metadata_id");
            lookup.CacheKey.Should().Be("entity_metadata_id-attribute_metadata_id");
        }
    }
}
