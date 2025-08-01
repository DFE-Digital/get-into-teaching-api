using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Represents the outcome of a domain save operation.
/// Encapsulates success state, diagnostic messaging, and any encountered validation or persistence errors.
/// </summary>
public class SaveResult
{
    /// <summary>
    /// Constructs a result object from the save operation.
    /// Applies null-safety defaults to error collection.
    /// </summary>
    /// <param name="isSuccessful">Indicates whether the save succeeded.</param>
    /// <param name="message">Contextual description of the outcome.</param>
    /// <param name="Errors">Optional list of errors; defaults to empty if omitted.</param>
    public SaveResult(bool isSuccessful, string message, List<SaveError> Errors = null!)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        this.Errors = Errors ?? [];
    }

    /// <summary>
    /// Indicates whether the save operation was successful.
    /// Drives control flow in calling services.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Optional message providing context about the operation result.
    /// Useful for logs, audit traces, or surface diagnostics.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Collection of errors that occurred during validation or persistence.
    /// Enables partial success handling or structured failure reporting.
    /// </summary>
    public List<SaveError> Errors { get; }

    /// <summary>
    /// Static factory method to construct a SaveResult.
    /// Encapsulates object creation and enforces error collection defaults.
    /// </summary>
    public static SaveResult Create(
        bool isSuccessful,
        string message,
        List<SaveError> Errors = null!)
    {
        return new SaveResult(isSuccessful, message, Errors);
    }
}

/// <summary>
/// Represents a granular save error tied to a specific target entity or input field.
/// Designed for structured reporting and end-user feedback.
/// </summary>
public class SaveError
{
    /// <summary>
    /// Optional identifier representing the item or field that failed.
    /// E.g., CandidateId, ContactChannelId, or property name.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Descriptive reason for the failure.
    /// Often used for logging, telemetry, or frontend display.
    /// </summary>
    public string Reason { get; set; }
}
