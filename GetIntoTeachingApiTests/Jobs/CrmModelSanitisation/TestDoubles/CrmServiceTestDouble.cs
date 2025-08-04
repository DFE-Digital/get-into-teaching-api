using GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;

/// <summary>
/// Provides test doubles for the <see cref="ICrmService"/> to support unit testing of candidate sanitisation logic.
/// </summary>
internal static class CrmServiceTestDouble
{
    /// <summary>
    /// Creates a default, unconfigured mock of <see cref="ICrmService"/>.
    /// </summary>
    /// <returns>A new <see cref="Mock{ICrmService}"/> instance.</returns>
    public static Mock<ICrmService> DefaultMock() => new();

    /// <summary>
    /// Creates a mock <see cref="ICrmService"/> that returns the specified candidate
    /// when queried via <c>GetCandidate(Guid)</c>.
    /// </summary>
    /// <param name="candidate">The candidate to return from the mock service.</param>
    /// <returns>A configured <see cref="Mock{ICrmService}"/> instance.</returns>
    public static Mock<ICrmService> MockFor(Candidate candidate)
    {
        var mockCrmService = DefaultMock();

        mockCrmService
            .Setup(service => service.GetCandidate(It.IsAny<Guid>()))
            .Returns(candidate)
            .Verifiable();

        return mockCrmService;
    }

    /// <summary>
    /// Creates a mock instance of ICrmService with a predefined response for GetCandidateContactCreations.
    /// Useful for unit tests where a specific set of ContactChannelCreation results needs to be returned.
    /// </summary>
    /// <param name="contactChannelCreations">The list of contact channel creation records to be returned by the mock.</param>
    /// <returns>A configured Mock<ICrmService> instance with mocked behavior.</returns>
    public static Mock<ICrmService> MockFor(IEnumerable<ContactChannelCreation> contactChannelCreations)
    {
        // Instantiate a default mock of ICrmService (helper method assumed to provide basic setup)
        var mockCrmService = DefaultMock();

        // Configure the mock to return the provided contactChannelCreations
        // whenever GetCandidateContactCreations is called with any Guid parameter
        mockCrmService
            .Setup(service => service.GetCandidateContactCreations(It.IsAny<Guid>()))
            .Returns(contactChannelCreations)
            .Verifiable();

        // Return the configured mock to be used in test scenarios
        return mockCrmService;
    }

    /// <summary>
    /// Creates a mock of ICrmService configured to verify that the Save method is invoked with any BaseModel.
    /// Intended for unit tests that exercise save behavior without requiring actual CRM implementation.
    /// </summary>
    /// <param name="contactChannelCreationSaveRequest">
    /// A placeholder parameter; not currently used within the method, but may inform future setup customization.
    /// </param>
    /// <returns>
    /// A Mock<ICrmService> instance with verifiable Save setup for assertion in test scenarios.
    /// </returns>
    public static Mock<ICrmService> MockFor(ContactChannelCreationSaveRequest contactChannelCreationSaveRequest)
    {
        // Create a default mock of ICrmService using a helper method
        var mockCrmService = DefaultMock();

        // Configure the mock to expect a call to Save with any BaseModel instance
        // Marked as verifiable for later assertion during test verification phase
        mockCrmService
            .Setup(service => service.Save(It.IsAny<BaseModel>()))
            .Verifiable();

        // Return the configured mock for use in unit tests
        return mockCrmService;
    }

    /// <summary>
    /// Creates a mock of ICrmService configured to throw an InvalidOperationException 
    /// whenever the Save method is invoked with any BaseModel instance.
    /// This setup simulates failure scenarios to validate exception handling logic in consuming code.
    /// </summary>
    public static Mock<ICrmService> MockForException()
    {
        // Create a default mock of ICrmService using a helper method
        var mockCrmService = DefaultMock();

        // Configure the mock so that calling Save with any BaseModel triggers an InvalidOperationException
        // This helps simulate a failure path in unit tests for robustness checks
        mockCrmService
            .Setup(service => service.Save(It.IsAny<BaseModel>()))
            .Throws<Exception>();

        // Return the mock with configured behavior for use in test assertions
        return mockCrmService;
    }
}