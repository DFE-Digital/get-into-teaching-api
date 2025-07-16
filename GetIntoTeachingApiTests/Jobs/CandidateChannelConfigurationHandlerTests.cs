namespace GetIntoTeachingApiTests.Jobs;

using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

public class CandidateChannelConfigurationHandlerTests
{

    [Fact]
    public void DoesNotHaveAContactChannelCreationRecord_NullCandidate_ThrowsException()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        
        // Act
        Action action = () => verifier.DoesNotHaveAContactChannelCreationRecord(null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'candidate')");
    }
    
    [Fact]
    public void DoesNotHaveAContactChannelCreationRecord_WithoutExistingCreationChannelRecord_ReturnsTrue()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        var contactChannelCreations = new List<ContactChannelCreation> { };
        Candidate candidate = new Candidate() { ContactChannelCreations = contactChannelCreations };

        // Assert
        verifier.DoesNotHaveAContactChannelCreationRecord(candidate).Should().Be(true);
    }

    [Fact]
    public void DoesNotHaveAContactChannelCreationRecord_WithExistingCreationChannelRecord_ReturnsFalse()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        var contactChannelCreations = new List<ContactChannelCreation>
        {
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.Apply,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply,
                }
        };
        Candidate candidate = new Candidate() { ContactChannelCreations = contactChannelCreations };

        // Assert
        verifier.DoesNotHaveAContactChannelCreationRecord(candidate).Should().Be(false);
    }
    
    [Fact]
    public void DoesNotHaveAContactChannelCreationRecord_WithOtherExistingCreationChannelRecord_ReturnsTrue()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        var contactChannelCreations = new List<ContactChannelCreation>
        {
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.GITWebsite,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.MailingList,
            }
        };
        Candidate candidate = new Candidate() { ContactChannelCreations = contactChannelCreations };

        // Assert
        verifier.DoesNotHaveAContactChannelCreationRecord(candidate).Should().Be(true);
    }
    
    [Fact]
    public void DoesNotHaveAContactChannelCreationRecord_WithMultipleExistingCreationChannelRecord_ReturnsFalse()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        var contactChannelCreations = new List<ContactChannelCreation>
        {
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.GITWebsite,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.MailingList,
            },
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.SchoolExperience,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience,
            },
            new ContactChannelCreation() { 
                CreationChannelSourceId = (int)ContactChannelCreation.CreationChannelSource.Apply,
                CreationChannelServiceId = (int)ContactChannelCreation.CreationChannelService.CreatedOnApply,
            }
        };
        Candidate candidate = new Candidate() { ContactChannelCreations = contactChannelCreations };

        // Assert
        verifier.DoesNotHaveAContactChannelCreationRecord(candidate).Should().Be(false);
    }

    [Fact]
    public void InvokeConfigureChannel_NullCandidate_ThrowsException()
    {
        // Arrange
        CandidateChannelConfigurationHandler verifier = new();
        
        // Act
        Action action = () => verifier.InvokeConfigureChannel(null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'wrappedCandidate')");
    }
}