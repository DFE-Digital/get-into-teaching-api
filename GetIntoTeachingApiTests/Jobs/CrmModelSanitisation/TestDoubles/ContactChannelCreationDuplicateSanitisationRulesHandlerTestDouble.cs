using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using Moq;
using System;

namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.TestDoubles;

/// <summary>
/// Provides test double implementations for mocking sanitisation rule handlers
/// targeting ContactChannelCreationSanitisationRequestWrapper scenarios.
/// </summary>
internal static class ContactChannelCreationDuplicateSanitisationRulesHandlerTestDouble
{
    /// <summary>
    /// Creates a default mock of ICrmModelSanitisationRulesHandler for the ContactChannel wrapper.
    /// No setup applied—intended for manual configuration or baseline mocking.
    /// </summary>
    public static Mock<ICrmModelSanitisationRulesHandler<
        ContactChannelCreationSanitisationRequestWrapper>> DefaultMock() => new();

    /// <summary>
    /// Creates a configured mock by applying the specified sanitisation action to the wrapper,
    /// and ensuring the SanitiseCrmModelWithRules method returns the mutated wrapper as-is.
    /// Useful for both preservation and removal test flows.
    /// </summary>
    /// <param name="sanitiseAction">An action that mutates the wrapper (e.g., preserve/remove logic).</param>
    /// <param name="wrapper">The sanitisation wrapper instance to mutate and pass into mock.</param>
    /// <returns>A mock configured to return the wrapper after sanitisation.</returns>
    private static Mock<ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>> CreateMockWith(
        Action<ContactChannelCreationSanitisationRequestWrapper> sanitiseAction,
        ContactChannelCreationSanitisationRequestWrapper wrapper)
    {
        // Apply the desired mutation to the wrapper (preserve or remove channel).
        sanitiseAction(wrapper);

        // Create a default mock and set it to return the mutated wrapper when invoked.
        var mock = DefaultMock();

        mock.Setup(handler => handler.SanitiseCrmModelWithRules(
                It.IsAny<ContactChannelCreationSanitisationRequestWrapper>()))
            .Returns(wrapper)
            .Verifiable(); // Marks this setup as verifiable for test assertions

        return mock;
    }

    /// <summary>
    /// Provides a mock setup that preserves the creation channel within the wrapper.
    /// Used to test that channel data is retained during sanitisation.
    /// </summary>
    /// <param name="wrapper">The wrapper instance to preserve channel on.</param>
    /// <returns>Mocked rules handler returning the preserved wrapper.</returns>
    public static Mock<ICrmModelSanitisationRulesHandler<
        ContactChannelCreationSanitisationRequestWrapper>> MockForPreserveChannel(
            ContactChannelCreationSanitisationRequestWrapper wrapper) =>
            CreateMockWith(wrapper => wrapper.PreserveCreationChannel(), wrapper);

    /// <summary>
    /// Provides a mock setup that removes the creation channel within the wrapper.
    /// Used to test that channel data is excluded during sanitisation.
    /// </summary>
    /// <param name="wrapper">The wrapper instance to remove channel from.</param>
    /// <returns>Mocked rules handler returning the modified wrapper.</returns>
    public static Mock<ICrmModelSanitisationRulesHandler<
        ContactChannelCreationSanitisationRequestWrapper>> MockForRemoveChannel(
            ContactChannelCreationSanitisationRequestWrapper wrapper) =>
            CreateMockWith(wrapper => wrapper.RemoveCreationChannel(), wrapper);
}