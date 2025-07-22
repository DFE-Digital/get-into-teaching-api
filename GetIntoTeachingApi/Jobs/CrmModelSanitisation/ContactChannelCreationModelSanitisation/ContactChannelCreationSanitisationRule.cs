using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

/// <summary>
/// Represents a sanitisation rule for <see cref="ContactChannelCreation"/> CRM records.
/// </summary>
public class ContactChannelCreationSanitisationRule : ICrmModelSanitisationRule<ContactChannelCreation>
{
    /// <summary>
    /// Applies sanitisation logic to a <see cref="ContactChannelCreation"/> model instance.
    /// </summary>
    /// <param name="model">
    /// The <see cref="ContactChannelCreation"/> record to sanitise.
    /// </param>
    /// <returns>
    /// The sanitised <see cref="ContactChannelCreation"/> record.
    /// </returns>
    /// <exception cref="System.NotImplementedException">
    /// Always thrown until sanitisation logic is implemented.
    /// </exception>
    public ContactChannelCreation SanitiseCrmModel(ContactChannelCreation model)
    {
        throw new System.NotImplementedException();
    }
}