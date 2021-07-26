using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Utils;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class ValidationResultExtensionsTests
    {
        [Fact]
        public void SurfaceErrorsOnMatchingProperties_WhenThereAreNoMatchingPropertiesOnTheRelatedModel_DoesNotSurfaceErrors()
        {
            var model = new GetIntoTeachingCallback();
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Candidate.MobileTelephone", "Some error message")
            };
            var validationResult = new ValidationResult(failures: validationErrors);
            var validationContext = new ValidationContext<GetIntoTeachingCallback>(instanceToValidate: model);

            validationResult.SurfaceErrorsOnMatchingProperties(validationContext);

            validationResult.Errors.Find(error => error.PropertyName == "Candidate.MobileTelephone").ErrorMessage
                .Should().Be("Some error message");
            validationResult.Errors.Find(error => error.PropertyName == "MobileTelephone").Should().BeNull();
        }

        [Fact]
        public void SurfaceErrorsOnMatchingProperties_WhenThereAreMatchingPropertiesOnTheRelatedModel_SurfacesErrors()
        {
            var model = new GetIntoTeachingCallback();
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Candidate.AddressTelephone", "Some error message"),
                new ValidationFailure("Candidate.PrivacyPolicy.AcceptedPolicyId", "Some other error message")
            };
            var validationResult = new ValidationResult(failures: validationErrors);
            var validationContext = new ValidationContext<GetIntoTeachingCallback>(instanceToValidate: model);

            validationResult.SurfaceErrorsOnMatchingProperties(validationContext);

            validationResult.Errors.Find(error => error.PropertyName == "AddressTelephone").ErrorMessage
                .Should().Be("Some error message");
            validationResult.Errors.Find(error => error.PropertyName == "AcceptedPolicyId").ErrorMessage
                .Should().Be("Some other error message");
            validationResult.Errors.Find(error => error.PropertyName == "Candidate.AddressTelephone").Should().BeNull();
        }
    }
}
