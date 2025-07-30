namespace GetIntoTeachingApi.Jobs.CandidateSanitisation;

/// <summary>
/// Defines a contract for sanitisation rules that modify CRM model instances.
/// </summary>
/// <typeparam name="TCrmModel">
/// The type of CRM model to be sanitised. Must be a reference type.
/// </typeparam>
public interface ICrmModelSanitisationRule<TCrmModel> where TCrmModel : class
{
    /// <summary>
    /// Applies sanitisation logic to the provided CRM model instance.
    /// </summary>
    /// <param name="model">
    /// The CRM model to be sanitised.
    /// </param>
    /// <returns>
    /// The updated <typeparamref name="TCrmModel"/> after sanitisation.
    /// </returns>
    TCrmModel SanitiseCrmModel(TCrmModel model);
}