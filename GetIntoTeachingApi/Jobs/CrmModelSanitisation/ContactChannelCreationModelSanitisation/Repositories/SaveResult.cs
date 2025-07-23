using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

/// <summary>
/// Represents the outcome of a save operation, including status flags and error details.
/// Suitable for write-side commands in domain or application layers.
/// </summary>
public class SaveResult
{
    /// <summary>
    /// Indicates whether the save operation succeeded.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Optional message providing context about the result (e.g., validation summary or audit trace).
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Collection of failed items and associated reasons, useful for partial success handling.
    /// </summary>
    public List<SaveError> Errors { get; set; } = [];
}

/// <summary>
/// Represents a discrete save error tied to a specific input or rule violation.
/// </summary>
public class SaveError
{
    /// <summary>
    /// Optional identifier pointing to the item that failed (e.g., channel ID or candidate ID).
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Descriptive message explaining the reason for failure (e.g., invalid format, missing data).
    /// </summary>
    public string Reason { get; set; }
}
