using FluentAssertions;
using GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

public class CandidateContactChannelCreationsCrmRepositoryTests
{
    [Fact]
    public void GetContactChannelCreationsByCandidateId_WithExisitingCreationChannels_ReturnsConfiguredContactCreationChannelCollection()
    {
        // arrange
        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(
                contactChannelCreations:
                ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub());

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        // act
        IEnumerable<ContactChannelCreation> contactChannelCreations =
            repository.GetContactChannelCreationsByCandidateId(candidateId: Guid.NewGuid());

        // assert/verify
        contactChannelCreations.Should().NotBeNull().And.NotBeEmpty();

        mockCrmService.Verify(crmService =>
            crmService.GetCandidateContactCreations(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void GetContactChannelCreationsByCandidateId_WithNullCreationChannels_ReturnsEmptyContactCreationChannelCollection()
    {
        // arrange
        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(contactChannelCreations: null!);

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        // act
        IEnumerable<ContactChannelCreation> contactChannelCreations =
            repository.GetContactChannelCreationsByCandidateId(candidateId: Guid.NewGuid());

        // verify
        contactChannelCreations.Should().NotBeNull().And.BeEmpty();
        mockCrmService.Verify(crmService =>
            crmService.GetCandidateContactCreations(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void GetContactChannelCreationsByCandidateId_WithEmptyCandidateIDGuid_ThrowsArgumentNullException()
    {
        // arrange
        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(contactChannelCreations: null!);

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        // act/assert
        Assert.Throws<ArgumentException>(() =>
            repository.GetContactChannelCreationsByCandidateId(candidateId: Guid.Empty));
    }

    [Fact]
    public void SaveContactChannelCreations_WithValidCandidateIDGuid_CallsSave()
    {
        // arrange
        ContactChannelCreationSaveRequest saveRequest =
            ContactChannelCreationSaveRequest.Create(
                candidateId: Guid.NewGuid(),
                candidateContactChannelCreation: ContactChannelCreationTestDouble.BuildSingleContactChannel(
                    ContactChannelCreation.CreationChannelSource.SchoolExperience,
                    ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience),
                candidateContactChannelCreations: ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub().AsReadOnly());

        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(saveRequest);

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        // act
        repository.SaveContactChannelCreations(saveRequest);

        // verify
        mockCrmService.Verify(crmService =>
            crmService.Save(It.IsAny<BaseModel>()), Times.Once);
    }

    [Fact]
    public void SaveContactChannelCreations_WithEmptyCandidateIDGuid_ThrowsArgumentNullException()
    {
        // arrange
        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockFor(contactChannelCreations: null!);

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        ContactChannelCreationSaveRequest saveRequest =
            ContactChannelCreationSaveRequest.Create(
                candidateId: Guid.Empty,
                candidateContactChannelCreation: ContactChannelCreationTestDouble.BuildSingleContactChannel(
                    ContactChannelCreation.CreationChannelSource.SchoolExperience,
                    ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience),
                candidateContactChannelCreations: ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub().AsReadOnly());

        // act/assert
        Assert.Throws<ArgumentException>(() =>
            repository.SaveContactChannelCreations(saveRequest));
    }

    [Fact]
    public void SaveContactChannelCreations_()
    {
        // arrange
        ContactChannelCreationSaveRequest saveRequest =
            ContactChannelCreationSaveRequest.Create(
                candidateId: Guid.NewGuid(),
                candidateContactChannelCreation: ContactChannelCreationTestDouble.BuildSingleContactChannel(
                    ContactChannelCreation.CreationChannelSource.SchoolExperience,
                    ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience),
                candidateContactChannelCreations: ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub().AsReadOnly());

        Mock<ICrmService> mockCrmService =
            CrmServiceTestDouble.MockForException();

        CandidateContactChannelCreationsCrmRepository repository = new(mockCrmService.Object);

        // act/assert
        Assert.Throws<InvalidOperationException>(() =>
            repository.SaveContactChannelCreations(saveRequest));
    }
}