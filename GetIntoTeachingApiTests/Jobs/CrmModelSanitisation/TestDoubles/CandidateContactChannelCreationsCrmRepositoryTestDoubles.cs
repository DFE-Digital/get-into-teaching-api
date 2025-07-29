using GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;
using GetIntoTeachingApi.Models.Crm;
using Moq;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.TestDoubles;

/// <summary>
/// Provides test double helpers for mocking the candidate contact channel creation repository.
/// Used to simulate CRM repository behavior in unit tests, without requiring actual data access.
/// </summary>
internal class CandidateContactChannelCreationsCrmRepositoryTestDoubles
{
    /// <summary>
    /// Creates a baseline mock of ICandidateContactChannelCreationsRepository with no configured behavior.
    /// Intended for manual configuration or lightweight mocking.
    /// </summary>
    public static Mock<ICandidateContactChannelCreationsRepository> DefaultMock() => new();

    /// <summary>
    /// Creates a mock of ICandidateContactChannelCreationsRepository configured for both 
    /// retrieval and save operations. This combines the Get and Save setups into a single reusable mock.
    /// Useful for unit tests that validate read-modify-write flows.
    /// </summary>
    /// <param name="contactChannelCreations">
    /// A collection of ContactChannelCreation instances to return when GetContactChannelCreationsByCandidateId is invoked.
    /// </param>
    /// <returns>
    /// A Mock<ICandidateContactChannelCreationsRepository> with both Get and Save operations preconfigured and marked as verifiable.
    /// </returns>
    public static Mock<ICandidateContactChannelCreationsRepository> MockForGetAndSave(
        IEnumerable<ContactChannelCreation> contactChannelCreations,
        SaveResult saveResult)
    {
        // Create a baseline mock of the CRM repository
        var mockRepository = DefaultMock();
        // Apply setup to simulate retrieval of contact channel creations for any candidate ID
        MockForGet(mockRepository, contactChannelCreations);
        // Apply setup to expect a Save operation using any ContactChannelCreationSaveRequest
        MockForSave(mockRepository, saveResult);
        // Return the fully configured mock for use in composite test scenarios
        return mockRepository;
    }

    /// <summary>
    /// Creates a mock repository with predefined return data for GetContactChannelCreationsByCandidateId.
    /// This setup mimics the retrieval of contact channel creation data for a candidate by ID.
    /// </summary>
    /// <param name="contactChannelCreations">Collection of ContactChannelCreation objects to be returned.</param>
    /// <returns>Configured mock with Get setup and marked as verifiable for test assertions.</returns>
    public static Mock<ICandidateContactChannelCreationsRepository> MockForGet(
        Mock<ICandidateContactChannelCreationsRepository> mockRepository,
        IEnumerable<ContactChannelCreation> contactChannelCreations)
    {
        // Setup the mock to return the provided contactChannelCreations when queried with any candidate ID.
        mockRepository
            .Setup(repository => repository.GetContactChannelCreationsByCandidateId(It.IsAny<Guid>()))
            .Returns(contactChannelCreations)
            .Verifiable(); // Allows later verification that this method was invoked.

        return mockRepository;
    }

    /// <summary>
    /// Creates a mock repository that expects SaveContactChannelCreations to be called during the test.
    /// Useful for verifying save operations and testing side-effect execution paths.
    /// </summary>
    /// <returns>Configured mock with Save setup and marked as verifiable for assertion.</returns>
    public static Mock<ICandidateContactChannelCreationsRepository> MockForSave(
        Mock<ICandidateContactChannelCreationsRepository> mockRepository,
        SaveResult saveResult)
    {
        // Setup the mock to expect a save call with any valid save request object.
        mockRepository
            .Setup(repository =>
                repository.SaveContactChannelCreations(It.IsAny<ContactChannelCreationSaveRequest>()))
            .Returns(saveResult)
            .Verifiable(); // Enables verification that save was triggered in the test.

        return mockRepository;
    }
}