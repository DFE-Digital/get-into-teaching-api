using System.Text.Json;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class RedactorTests
    {
        [Fact]
        public void RedactJson_WithValidJson_RedactsSensitiveData()
        {
            string json = JsonSerializer.Serialize(
                new
                {
                    password = "abc123",
                    firstName = "Ross",
                    nested = new
                    {
                        addressLine1 = "6 main street",
                        password = "abc123",
                    },
                    email = new string[] { "first", "second" },
                    addressPostcode = "TE7 1NG",
                }
            );

            string redactedJson = JsonSerializer.Serialize(
                new
                {
                    password = "******",
                    firstName = "******",
                    nested = new
                    {
                        addressLine1 = "******",
                        password = "******",
                    },
                    email = "******",
                    addressPostcode = "TE7 1NG",
                }
            );

            var result = Redactor.RedactJson(json);

            result.Should().Be(redactedJson);
        }

        [Fact]
        public void RedactJson_WithInvalidJson_ReturnsEmpty()
        {
            var invalidJson = "my password is 123456";

            var result = Redactor.RedactJson(invalidJson);

            result.Should().Be(string.Empty);
        }
    }
}
