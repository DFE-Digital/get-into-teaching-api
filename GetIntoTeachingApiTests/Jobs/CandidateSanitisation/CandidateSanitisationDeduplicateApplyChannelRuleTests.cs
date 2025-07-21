using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Xunit;
using FluentAssertions;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;


public class CandidateSanitisationDeduplicateApplyChannelRuleTests
{
    private const int PreserveCount = 2;
    private const int FilterCount = 1;
    
    [Fact]
    public void SanitiseCandidate_CandidateWithoutCreatedOnApplyChannel_PreservesCreatedOnApplyCreationChannel()
    {
        //arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        Candidate updateCandidate = new Candidate();

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Count().Should().Be(0);
    }

    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndDoesNotExistOnCrm_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        Candidate updateCandidate = new Candidate()
        {
            Id = Guid.NewGuid(),
            ContactChannelCreations = BuildContactChannelCreations()
        };
        
        mockCrmService.Setup(crmService => crmService.GetCandidate(It.IsAny<Guid>())).Returns<Candidate>(null!);

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Count().Should().Be(PreserveCount);
    }
    
    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithoutCreatedOnApply_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        Candidate updateCandidate = new Candidate()
        {
            Id = Guid.NewGuid(),
            ContactChannelCreations = BuildContactChannelCreations()
        };
        Candidate crmCandidate = new Candidate()
        {
            Id = updateCandidate.Id,
            ContactChannelCreations = []
        };
        
        mockCrmService.Setup(crmService => crmService.GetCandidate(It.IsAny<Guid>())).Returns(crmCandidate);

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Count().Should().Be(PreserveCount);
    }
    
    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithCreatedOnApply_RemovesCreatedOnApplyCreationChannel()
    {
        //arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        Candidate updateCandidate = new Candidate()
        {
            Id = Guid.NewGuid(),
            ContactChannelCreations = BuildContactChannelCreations()
        };
        mockCrmService.Setup(crmService => crmService.GetCandidate(It.IsAny<Guid>())).Returns(updateCandidate);

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Count().Should().Be(FilterCount);
    }

    private List<ContactChannelCreation> BuildContactChannelCreations()
    {
        return new List<ContactChannelCreation>
        {
            new ContactChannelCreation()
            {
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.Apply,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply,
            },
            new ContactChannelCreation()
            {
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.SchoolExperience,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience,
            }
        };
    }

}