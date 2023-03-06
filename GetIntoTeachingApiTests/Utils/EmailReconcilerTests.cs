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
            var previous = Environment.GetEnvironmentVariable("RECONCILE_EMAILS_FEATURE");
            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", "true");

            var equivalentEmails = EmailReconciler.EquivalentEmails(email);

            equivalentEmails.Should().BeEquivalentTo(expectedEquivalentEmails);

            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", previous);
        }

        [Fact]
        public void EquivalentEmails_WithInvalidEmail_Throws()
        {
            var previous = Environment.GetEnvironmentVariable("RECONCILE_EMAILS_FEATURE");
            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", "true");

            var action = () => EmailReconciler.EquivalentEmails("invalid@email@domain.com");

            action.Should().Throw<FormatException>()
                .WithMessage("An invalid character was found in the mail header: '@'.");

            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", previous);
        }

        [Fact]
        public void EquivalentEmails_WhenFeatureIsOff_ReturnsSingleEmail()
        {
            var previous = Environment.GetEnvironmentVariable("RECONCILE_EMAILS_FEATURE");
            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", "false");

            var equivalentEmails = EmailReconciler.EquivalentEmails("john@gmail.com");

            equivalentEmails.Should().BeEquivalentTo(new string[] { "john@gmail.com" });

            Environment.SetEnvironmentVariable("RECONCILE_EMAILS_FEATURE", previous);
        }
    }
}

