using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateIdsRequestTests
    {
        [Fact]
        public void Constructor_WithInitializer()
        {
            var candidateIdsRequest = new CandidateIdsRequest { CandidateIds =  new[] { 1, 2, 3 }  };

            candidateIdsRequest.CandidateIds.Should().Equal(1, 2, 3);
        }
    }
}
