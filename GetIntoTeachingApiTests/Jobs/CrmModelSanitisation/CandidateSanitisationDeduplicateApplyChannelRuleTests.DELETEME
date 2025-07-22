using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using Moq;
using System;
using Xunit;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

public class CandidateSanitisationDeduplicateApplyChannelRuleTests
{
    /// <summary>
    /// The preserve count.
    /// </summary>
    private const int PreserveCount = 2;
    /// <summary>
    /// The filter count.
    /// </summary>
    private const int FilterCount = 1;

    [Fact]
    public void SanitiseCandidate_CandidateWithoutCreatedOnApplyChannel_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Mock<ICrmService> mockCrmService = CrmServiceTestDouble.DefaultMock();
        CandidateSanitisationDeduplicateApplyChannelRule rule = new(mockCrmService.Object);
        Candidate updateCandidate = new();

        // act
        Candidate returnedCandidate = rule.SanitiseCrmModel(updateCandidate);

        // assert
        returnedCandidate.ContactChannelCreations.Count.Should().Be(0);
    }

    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndDoesNotExistOnCrm_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Candidate updateCandidate =
            new CandidateBuilder()
                .WithId(Guid.NewGuid())
                .WithContactChannels(ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub())
                .Build();

        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(null!);

        CandidateSanitisationDeduplicateApplyChannelRule rule = new(mockCrmService.Object);

        // act
        Candidate returnedCandidate = rule.SanitiseCrmModel(updateCandidate);

        // assert
        returnedCandidate.ContactChannelCreations.Count.Should().Be(PreserveCount);
    }

    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithoutCreatedOnApply_PreservesCreatedOnApplyCreationChannel()
    {
        // arrange
        Candidate updateCandidate =
            new CandidateBuilder()
                .WithId(Guid.NewGuid())
                .WithContactChannels(ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub())
                .Build();

        Candidate crmCandidate =
            new CandidateBuilder()
                .WithId(updateCandidate.Id.Value)
                .Build();

        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(crmCandidate);

        CandidateSanitisationDeduplicateApplyChannelRule rule = new(mockCrmService.Object);

        //act
        Candidate returnedCandidate = rule.SanitiseCrmModel(updateCandidate);

        //assert
        returnedCandidate.ContactChannelCreations.Count.Should().Be(PreserveCount);
    }

    [Fact]
    public void SanitiseCandidate_CandidateWithCreatedOnApplyChannelAndExistsOnCrmWithCreatedOnApply_RemovesCreatedOnApplyCreationChannel()
    {
        //arrange
        Candidate updateCandidate =
            new CandidateBuilder()
                .WithId(Guid.NewGuid())
                .WithContactChannels(ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub())
                .Build();

        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(updateCandidate);

        CandidateSanitisationDeduplicateApplyChannelRule rule = new(mockCrmService.Object);

        //act
        Candidate returnedCandidate = rule.SanitiseCrmModel(updateCandidate);

        //assert
        returnedCandidate.ContactChannelCreations.Count.Should().Be(FilterCount);
    }
}