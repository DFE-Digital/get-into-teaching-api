using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;
using GetIntoTeachingApi.Jobs.UpsertStrategies;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.TestDoubles;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs.UpsertStrategies;

public class ContactChannelCreationSanitisationUpsertStrategyTests
{
    [Fact]
    public void TryUpsert_WithValidSaveRequestModelNoDuplicates_ShouldCallSaveAndReturnAppropriatelyConfiguredSaveResponse()
    {
        // arrange
        ContactChannelCreation creationChannel =
            ContactChannelCreationTestDouble.BuildSingleContactChannel(
                ContactChannelCreation.CreationChannelSource.SchoolExperience,
                ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience);

        List<ContactChannelCreation> candidateContactChannelCreations =
            ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub();

        ContactChannelCreationSanitisationRequestWrapper wrapper =
            new(
                creationChannel,
                candidateContactChannelCreations.AsReadOnly());

        SaveResult saveResult = new SaveResultBuilder()
            .WithSuccess(true)
            .WithMessage("Validation succeeded")
            .Build();

        Mock<ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>> mockRulesHandler =
            ContactChannelCreationDuplicateSanitisationRulesHandlerTestDouble.MockForPreserveChannel(wrapper);

        Mock<ICandidateContactChannelCreationsRepository> mockRepository =
            CandidateContactChannelCreationsCrmRepositoryTestDoubles.MockForGetAndSave(
                candidateContactChannelCreations, saveResult);

        ContactChannelCreationSanitisationUpsertStrategy contactChannelCreationSanitisationUpsertStrategy =
            new(
                mockRulesHandler.Object,
                mockRepository.Object
            );

        // act
        bool result =
            contactChannelCreationSanitisationUpsertStrategy.TryUpsert(creationChannel, out string logMessage);

        // assert/verify
        Assert.True(result, "Expected save to succeed when sanitisation rules indicate preservation.");

        mockRepository.Verify(repository =>
            repository.GetContactChannelCreationsByCandidateId(It.IsAny<Guid>()), Times.Once);

        mockRepository.Verify(repository =>
            repository.SaveContactChannelCreations(It.IsAny<ContactChannelCreationSaveRequest>()), Times.Once);
    }

    [Fact]
    public void TryUpsert_WithValidSaveRequestModelButDuplicate_ShouldNotCallSaveAndReturnAppropriatelyConfiguredSaveResponse()
    {
        // arrange
        ContactChannelCreation creationChannel =
            ContactChannelCreationTestDouble.BuildSingleContactChannel(
                ContactChannelCreation.CreationChannelSource.SchoolExperience,
                ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience);

        List<ContactChannelCreation> candidateContactChannelCreations =
            ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub();

        ContactChannelCreationSanitisationRequestWrapper wrapper =
            new(
                creationChannel,
                candidateContactChannelCreations.AsReadOnly());

        SaveResult saveResult =
            new SaveResultBuilder()
                .WithSuccess(false)
                .WithMessage("Validation failed")
                .WithError("CandidateId", "Missing candidate identifier")
                .Build();

        Mock<ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>> mockRulesHandler =
            ContactChannelCreationDuplicateSanitisationRulesHandlerTestDouble.MockForRemoveChannel(wrapper);

        Mock<ICandidateContactChannelCreationsRepository> mockRepository =
            CandidateContactChannelCreationsCrmRepositoryTestDoubles.MockForGetAndSave(
                candidateContactChannelCreations, saveResult);

        ContactChannelCreationSanitisationUpsertStrategy contactChannelCreationSanitisationUpsertStrategy =
            new(
                mockRulesHandler.Object,
                mockRepository.Object
            );

        // act
        bool result =
            contactChannelCreationSanitisationUpsertStrategy.TryUpsert(creationChannel, out string logMessage);

        // assert/verify
        Assert.False(result, "Expected save to fail when sanitisation rules indicate removal.");

        mockRepository.Verify(repository =>
            repository.GetContactChannelCreationsByCandidateId(It.IsAny<Guid>()), Times.Once);

        mockRepository.Verify(repository =>
            repository.SaveContactChannelCreations(It.IsAny<ContactChannelCreationSaveRequest>()), Times.Never);
    }

    [Fact]
    public void TryUpsert_WithNullSaveRequestModel_ShouldThrowArgumentNullException()
    {
        // arrange
        Mock<ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>> mockRulesHandler =
            ContactChannelCreationDuplicateSanitisationRulesHandlerTestDouble.DefaultMock();
        Mock<ICandidateContactChannelCreationsRepository> mockRepository =
            CandidateContactChannelCreationsCrmRepositoryTestDoubles.DefaultMock();

        ContactChannelCreationSanitisationUpsertStrategy contactChannelCreationSanitisationUpsertStrategy =
            new(
                mockRulesHandler.Object,
                mockRepository.Object
            );

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            contactChannelCreationSanitisationUpsertStrategy.TryUpsert(model: null!, out string logMessage));
    }
}
