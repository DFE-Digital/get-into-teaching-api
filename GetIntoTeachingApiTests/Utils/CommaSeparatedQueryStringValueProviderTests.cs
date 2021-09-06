using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class CommaSeparatedQueryStringValueProviderTests
    {
        private readonly string[] _keys;
        private readonly IQueryCollection _values;
        private readonly CommaSeparatedQueryStringValueProvider _provider;

        public CommaSeparatedQueryStringValueProviderTests()
        {
            _keys = new string[] { "key1", "key2" };
            _values = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "key1", "value1,value2,value3" },
                { "key2", "other_value1,other_value2" },
                { "key3", "other_value1|other_value2" },
            });

            _provider = new CommaSeparatedQueryStringValueProvider(_keys, _values, ",");
        }

        [Fact]
        public void GetValue_WithNonMatchingKey_ReturnsASingleValue()
        {
            var result = _provider.GetValue("key3");

            result.Values.Should().BeEquivalentTo(new string[] { "other_value1|other_value2" });
        }

        [Fact]
        public void GetValue_WithMatchingKeyAndSeparator_ReturnsMultipleValues()
        {
            var result = _provider.GetValue("key1");

            result.Values.Should().BeEquivalentTo(new string[] { "value1", "value2", "value3" });
        }

        [Fact]
        public void GetValue_WithMatchingKeyAndNoSeparator_ReturnsSingleValue()
        {
            var provider = new CommaSeparatedQueryStringValueProvider(new string[] { "key3" }, _values, ",");

            var result = provider.GetValue("key3");

            result.Values.Should().BeEquivalentTo(new string[] { "other_value1|other_value2" });
        }
    }
}
