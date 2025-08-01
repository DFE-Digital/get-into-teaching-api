using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Fluent builder for constructing SaveResult instances.
/// Supports chained setup of success status, diagnostic messaging, and validation errors.
/// </summary>
public class SaveResultBuilder
{
    private bool _isSuccessful;
    private string _message = string.Empty;
    private readonly List<SaveError> _errors = [];

    /// <summary>
    /// Sets the success status for the SaveResult.
    /// </summary>
    public SaveResultBuilder WithSuccess(bool isSuccessful)
    {
        _isSuccessful = isSuccessful;
        return this;
    }

    /// <summary>
    /// Sets the contextual message describing the outcome.
    /// </summary>
    public SaveResultBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    /// <summary>
    /// Adds a single SaveError to the error collection.
    /// </summary>
    public SaveResultBuilder WithError(string target, string reason)
    {
        _errors.Add(new SaveError { Target = target, Reason = reason });
        return this;
    }

    /// <summary>
    /// Adds multiple SaveErrors at once.
    /// </summary>
    public SaveResultBuilder WithErrors(IEnumerable<SaveError> errors)
    {
        _errors.AddRange(errors);
        return this;
    }

    /// <summary>
    /// Builds and returns the SaveResult instance.
    /// </summary>
    public SaveResult Build() => new SaveResult(_isSuccessful, _message, _errors);
}
