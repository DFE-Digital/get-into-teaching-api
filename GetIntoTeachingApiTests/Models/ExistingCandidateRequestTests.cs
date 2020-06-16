using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class ExistingCandidateRequestTests
    {
        private readonly ExistingCandidateRequest _request;

        public ExistingCandidateRequestTests()
        {
            _request = new ExistingCandidateRequest
            {
                Email = "email@address.com",
                FirstName = "first",
                LastName = "last",
                DateOfBirth = new DateTime(2000, 1, 1)
            };
        }

        [Fact]
        public void Match_WithNullCandidate_ReturnsFalse()
        {
            _request.Match(null).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndNoAdditionalAttributes_ReturnsFalse()
        {
            var entity = new Entity();
            entity["emailaddress1"] = "email@address.com";

            _request.Match(entity).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndOneAdditionalAttribute_ReturnsFalse()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email;
            entity["firstname"] = _request.FirstName;

            _request.Match(entity).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndTwoAdditionalAttributes_ReturnsTrue()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email;
            entity["firstname"] = _request.FirstName;
            entity["lastname"] = _request.LastName;

            _request.Match(entity).Should().BeTrue();
        }

        [Fact]
        public void Match_WithoutEmailAndWithTwoAdditionalAttributes_ReturnsFalse()
        {
            var entity = new Entity();
            entity["emailaddress1"] = "wrong@email.com";
            entity["firstname"] = _request.FirstName;
            entity["lastname"] = _request.LastName;

            _request.Match(entity).Should().BeFalse();
        }

        [Fact]
        public void Match_WithWrongEmailAndWithThreeAdditionalAttributes_ReturnsFalse()
        {
            var entity = new Entity();
            entity["emailaddress1"] = "wrong@email.com";
            entity["firstname"] = _request.FirstName;
            entity["lastname"] = _request.LastName;
            entity["birthdate"] = _request.DateOfBirth;

            _request.Match(entity).Should().BeFalse();
        }

        [Fact]
        public void Match_WithNullEmailAndWithThreeAdditionalAttributes_ReturnsFalse()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email;
            entity["firstname"] = _request.FirstName;
            entity["lastname"] = _request.LastName;
            entity["birthdate"] = _request.DateOfBirth;
            _request.Email = null;

            _request.Match(entity).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndThreeAdditionalAttributes_ReturnsTrue()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email;
            entity["firstname"] = _request.FirstName;
            entity["lastname"] = _request.LastName;
            entity["birthdate"] = _request.DateOfBirth;

            _request.Match(entity).Should().BeTrue();
        }

        [Fact]
        public void Match_WithCaseInsensitiveMatch_ReturnsTrue()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email.ToUpper();
            entity["firstname"] = _request.FirstName.ToUpper();
            entity["lastname"] = _request.LastName.ToUpper();

            _request.Match(entity).Should().BeTrue();
        }

        [Fact]
        public void Match_WithMatchingDateButDifferentTimes_ReturnsTrue()
        {
            var entity = new Entity();
            entity["emailaddress1"] = _request.Email;
            entity["firstname"] = _request.FirstName;
            entity["birthdate"] = _request.DateOfBirth?.AddMinutes(30);

            _request.Match(entity).Should().BeTrue();
        }

        [Fact]
        public void Slugify_ReturnsCorrectSlug()
        {
            _request.Slugify().Should().Be("email@address.com-first-last-01-01-2000");
        }

        [Fact]
        public void Slugify_WithNullAdditionalAttribute_OmitsNull()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "first" };
            request.Slugify().Should().Be("email@address.com-first");
        }

        [Fact]
        public void Slugify_WithMixedCasing_ReturnsLowerCase()
        {
            var request = new ExistingCandidateRequest { Email = "EMAIL@address.com", FirstName = "FIrst" };
            request.Slugify().Should().Be("email@address.com-first");
        }
    }
}
