using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class MailingListAddMemberRequestValidatorTests
    {
        private readonly MailingListAddMemberRequestValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public MailingListAddMemberRequestValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new MailingListAddMemberRequestValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };

            _mockStore
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockEntityReference }.AsQueryable());

            var request = new MailingListAddMemberRequest()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email@address.com",
                Telephone = "07584 734 576",
                AddressPostcode = "postcode",
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id }
            };

            var result = _validator.TestValidate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_PrivacyPolicyIsInvalid_HasError()
        {
            var request = new MailingListAddMemberRequest
            {
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };
            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(r => r.PrivacyPolicy.AcceptedPolicyId);
        }

        [Fact]
        public void Validate_EmailAddressIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, "");
        }

        [Fact]
        public void Validate_EmailAddressIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, "invalid-email@");
        }

        [Fact]
        public void Validate_EmailAddressTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, $"{new string('a', 50)}@{new string('a', 50)}.com");
        }

        [Fact]
        public void Validate_EmailAddressPresent_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.Email, "valid@email.com");
        }

        [Fact]
        public void Validate_FirstNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, "");
        }

        [Fact]
        public void Validate_FirstNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, new string('a', 257));
        }

        [Fact]
        public void Validate_LastNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, "");
        }

        [Fact]
        public void Validate_LastNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, new string('a', 257));
        }

        [Fact]
        public void Validate_TelephoneIsEmpty_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(request => request.Telephone, "");
        }

        [Fact]
        public void Validate_TelephoneTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Telephone, new string('a', 51));
        }

        [Fact]
        public void Validate_AddressPostcodeIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressPostcode, "");
        }

        [Fact]
        public void Validate_AddressPostcodeIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AddressPostcode, new string('a', 41));
        }

        [Fact]
        public void Validate_PrivacyPolicyIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.PrivacyPolicy, null as CandidatePrivacyPolicy);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId, Guid.NewGuid());
        }
    }
}
