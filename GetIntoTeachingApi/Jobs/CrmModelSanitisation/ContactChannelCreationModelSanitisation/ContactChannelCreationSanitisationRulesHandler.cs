using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

/// <summary>
/// Coordinates the application of a series of sanitisation rules against a candidate contact channel creation request.
/// Implements a rule handler abstraction to encapsulate sequential sanitisation logic.
/// </summary>
public class ContactChannelCreationSanitisationRulesHandler
    : ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper>
{
    /// <summary>
    /// The set of sanitisation rules to apply to the incoming model.
    /// Each rule mutates the wrapper in accordance with its validation or transformation logic.
    /// </summary>
    private readonly IEnumerable<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> _sanitisationRules;

    /// <summary>
    /// Constructs the handler with the injected rule set.
    /// Encourages DI pattern for rule composition and test isolation.
    /// </summary>
    public ContactChannelCreationSanitisationRulesHandler(
        IEnumerable<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> sanitisationRules)
    {
        _sanitisationRules = sanitisationRules;
    }

    /// <summary>
    /// Applies all configured sanitisation rules to the provided model sequentially.
    /// Each rule may mutate the model — later rules act on prior mutations.
    /// </summary>
    /// <param name="model">
    /// The wrapper encapsulating a candidate's contact creation and associated history.
    /// </param>
    /// <returns>
    /// The resulting wrapper after rule evaluation, possibly containing modified state.
    /// </returns>
    public ContactChannelCreationSanitisationRequestWrapper SanitiseCrmModelWithRules(
        ContactChannelCreationSanitisationRequestWrapper model)
    {
        ArgumentNullException.ThrowIfNull(model);

        foreach (var candidateSanitisationRule in _sanitisationRules)
        {
            model = candidateSanitisationRule.SanitiseCrmModel(model);
        }

        return model;
    }
}