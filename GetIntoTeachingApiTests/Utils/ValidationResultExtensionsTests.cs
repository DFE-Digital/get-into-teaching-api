using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class ValidationResultExtensionsTests
    {
        [Fact]
        public void MapRelatedModelErrorsToRequestModel_WhenThereAreNoMatchingPropertiesOnTheRequestModel_ReturnsRelatedModelErrors()
        {
            const string relatedModelName = "Candidate";
            var requestModel = new GetIntoTeachingCallback();
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Candidate.MobileTelephone", "Some error message")
            };
            var validationResult = new ValidationResult(failures: validationErrors);
            var validationContext = new ValidationContext<GetIntoTeachingCallback>(instanceToValidate: requestModel);

            validationResult.MapRelatedModelErrorsToRequestModel(validationContext, relatedModelName);

            validationResult.Errors.Find(error => error.PropertyName == "Candidate.MobileTelephone").ErrorMessage
                .Should().Be("Some error message");
            validationResult.Errors.Find(error => error.PropertyName == "MobileTelephone").Should().BeNull();
        }

        [Fact]
        public void MapRelatedModelErrorsToRequestModel_WhenThereAreMatchingPropertiesOnTheRequestModel_MapsRelatedModelErrorsToRequestModel()
        {
            const string relatedModelName = "Candidate";
            var requestModel = new GetIntoTeachingCallback();
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Candidate.AddressTelephone", "Some error message"),
                new ValidationFailure("Candidate.PrivacyPolicy.AcceptedPolicyId", "Some other error message")
            };
            var validationResult = new ValidationResult(failures: validationErrors);
            var validationContext = new ValidationContext<GetIntoTeachingCallback>(instanceToValidate: requestModel);

            validationResult.MapRelatedModelErrorsToRequestModel(validationContext, relatedModelName);

            validationResult.Errors.Find(error => error.PropertyName == "AddressTelephone").ErrorMessage
                .Should().Be("Some error message");
            validationResult.Errors.Find(error => error.PropertyName == "Candidate.AddressTelephone").Should().BeNull();
            validationResult.Errors.Find(error => error.PropertyName == "AcceptedPolicyId").ErrorMessage
                .Should().Be("Some other error message");
        }
    }
}
