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
        returnedCandidate.Should().BeSameAs(updateCandidate);
    }

    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndDoesNotExistOnCrm_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        var contactChannelCreations = new List<ContactChannelCreation>
        {
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.Apply,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply,
            }
        };
        Candidate updateCandidate = new Candidate()
        {
            Id = Guid.NewGuid(),
            ContactChannelCreations = contactChannelCreations
        };
        
        mockCrmService.Setup(crmService => crmService.GetCandidate(It.IsAny<Guid>())).Returns<Candidate>(null!);

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Should().HaveCount(1);
    }
    
    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithoutCreatedOnApply_PreservesCreatedOnApplyCreationChannel()
    {
        
    }
    
    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithCreatedOnApply_RemovesCreatedOnApplyCreationChannel()
    {
        //arrange
        Mock<ICrmService> mockCrmService = new();
        ICandidateSanitisationRule rule = new CandidateSanitisationDeduplicateApplyChannelRule(mockCrmService.Object);
        var contactChannelCreations = new List<ContactChannelCreation>
        {
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.Apply,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply,
            }
        };
        Candidate updateCandidate = new Candidate()
        {
            Id = Guid.NewGuid(),
            ContactChannelCreations = contactChannelCreations
        };
        mockCrmService.Setup(crmService => crmService.GetCandidate(It.IsAny<Guid>())).Returns(updateCandidate);

        //act
        Candidate returnedCandidate = rule.SanitiseCandidate(updateCandidate);
        
        //assert
        returnedCandidate.ContactChannelCreations.Should().BeEmpty();
    }

}