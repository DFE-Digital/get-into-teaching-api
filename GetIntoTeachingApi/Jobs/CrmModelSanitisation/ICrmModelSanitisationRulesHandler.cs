namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

/// <summary>
/// Defines a handler that applies a set of sanitisation rules to a CRM model.
/// </summary>
/// <typeparam name="TCrmModel">
/// The type of CRM model to be sanitised. Must be a reference type.
/// </typeparam>
public interface ICrmModelSanitisationRulesHandler<TCrmModel> where TCrmModel : class
{
    /// <summary>
    /// Applies all configured sanitisation rules to the specified CRM model instance.
    /// </summary>
    /// <param name="model">
    /// The <typeparamref name="TCrmModel"/> instance to be sanitised.
    /// </param>
    /// <returns>
    /// The sanitised <typeparamref name="TCrmModel"/> after rule application.
    /// </returns>
    TCrmModel SanitiseCrmModelWithRules(TCrmModel model);
}