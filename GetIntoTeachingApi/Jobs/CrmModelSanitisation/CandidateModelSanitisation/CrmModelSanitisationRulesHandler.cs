using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

/// <summary>
/// Provides a handler for applying a sequence of sanitisation rules to a <see cref="Candidate"/> CRM model.
/// </summary>
public class CrmModelSanitisationRulesHandler : ICrmModelSanitisationRulesHandler<Candidate>
{
    private readonly IEnumerable<ICrmModelSanitisationRule<Candidate>> _sanitisationRules;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmModelSanitisationRulesHandler"/> class.
    /// </summary>
    /// <param name="sanitisationRules">
    /// A collection of rules that should be applied to a <see cref="Candidate"/> during sanitisation.
    /// </param>
    public CrmModelSanitisationRulesHandler(IEnumerable<ICrmModelSanitisationRule<Candidate>> sanitisationRules)
    {
        _sanitisationRules = sanitisationRules;
    }

    /// <summary>
    /// Applies all configured sanitisation rules to the specified CRM model.
    /// </summary>
    /// <param name="model">
    /// The <see cref="Candidate"/> instance to sanitise.
    /// </param>
    /// <returns>
    /// The updated <see cref="Candidate"/> after all rules have been applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="model"/> is <c>null</c>.
    /// </exception>
    public Candidate SanitiseCrmModelWithRules(Candidate model)
    {
        ArgumentNullException.ThrowIfNull(model);

        foreach (ICrmModelSanitisationRule<Candidate> candidateSanitisationRule in _sanitisationRules)
        {
            model = candidateSanitisationRule.SanitiseCrmModel(model);
        }

        return model;
    }
}
