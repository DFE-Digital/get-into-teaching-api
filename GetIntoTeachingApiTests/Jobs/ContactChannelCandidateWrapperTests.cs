using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class ContactChannelCandidateWrapperTests
    {
        private readonly Candidate _candidate;
        
        public ContactChannelCandidateWrapperTests()
        {
            _candidate = new Candidate();
        }
        
        [Fact]
        public void Constructor_WithCandidate_MapsToScopedCandidate()
        {
            var contactChannelCandidateWrapper = new ContactChannelCandidateWrapper(_candidate);
            contactChannelCandidateWrapper.ScopedCandidate.Should().Be(_candidate);
        }
    }    
}
