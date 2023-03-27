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
