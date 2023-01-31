using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using Microsoft.Xrm.Sdk;
using Xunit;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateMatchbackResponseTests
    {
        private readonly CandidateMatchbackResponse _response;
        private readonly Candidate _candidate;

        public CandidateMatchbackResponseTests()
        {
            _candidate = new Candidate() { Id = Guid.NewGuid() };
            _response = new CandidateMatchbackResponse(_candidate);
        }

        [Fact]
        public void CandidateId_ReturnsCandidateId()
        {
            _response.CandidateId.Should().Be((Guid)_candidate.Id);
        }
    }
}
