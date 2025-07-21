using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Moq;
using System;

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
    public static Mock<ICrmService> DefaultMock() => new Mock<ICrmService>();

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
            .Returns(candidate);

        return mockCrmService;
    }
}