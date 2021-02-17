using System;
using System.Text.Json;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class EmptyStringToNullJsonConverterTests
    {
        private readonly EmptyStringToNullJsonConverter _converter;

        public EmptyStringToNullJsonConverterTests()
        {
            _converter = new EmptyStringToNullJsonConverter();
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(object), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(bool), false)]
        public void CanConvert_ReturnsCorrectly(Type type, bool expected)
        {
            _converter.CanConvert(type).Should().Be(expected);
        }

        [Theory]
        [InlineData("{\"Name\":\"\"}", null)]
        [InlineData("{\"Name\":\" \"}", null)]
        [InlineData("{\"Name\":null}", null)]
        [InlineData("{\"Name\":\" a\"}", " a")]
        [InlineData("{\"Name\":\"a test string\"}", "a test string")]
        public void Read_DeserializesString_ToNullIfEmpty(string json, string expected)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(_converter);

            var result = JsonSerializer.Deserialize<StubPerson>(json, options);

            result.Name.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "{\"Name\":null}")]
        [InlineData(" ", "{\"Name\":null}")]
        [InlineData(null, "{\"Name\":null}")]
        [InlineData(" a", "{\"Name\":\" a\"}")]
        [InlineData("a test string", "{\"Name\":\"a test string\"}")]
        public void Write_SerializesString_ToNullIfEmpty(string input, string expected)
        {
            var stub = new StubPerson() { Name = input };
            var options = new JsonSerializerOptions();
            options.Converters.Add(_converter);
           
            var result = JsonSerializer.Serialize(stub, options);

            result.Should().Be(expected);
        }

        private class StubPerson
        {
            public string Name { get; set; }
        }
    }
}
