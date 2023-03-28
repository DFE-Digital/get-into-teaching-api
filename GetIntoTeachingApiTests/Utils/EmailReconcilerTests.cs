using FluentAssertions;
using System;
using Xunit;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApiTests.Utils
{
	public class EmailReconcilerTests
	{
        [Theory]
        [InlineData("john@gmail.com", new string[] { "john@gmail.com", "john@googlemail.com" })]
        [InlineData("john.smith@googlemail.com", new string[] { "john.smith@gmail.com", "john.smith@googlemail.com" })]
        [InlineData("john@domain.com", new string[] { "john@domain.com" })]
        public void EquivalentEmails_WithEmail_ReturnsEquivalent(string email, string[] expectedEquivalentEmails)
        {
            var equivalentEmails = EmailReconciler.EquivalentEmails(email);

            equivalentEmails.Should().BeEquivalentTo(expectedEquivalentEmails);
        }

        [Fact]
        public void EquivalentEmails_WithInvalidEmail_Throws()
        {
            var action = () => EmailReconciler.EquivalentEmails("invalid@email@domain.com");

            action.Should().Throw<FormatException>()
                .WithMessage("An invalid character was found in the mail header: '@'.");
        }
    }
}

