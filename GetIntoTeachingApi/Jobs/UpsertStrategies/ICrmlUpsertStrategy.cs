namespace GetIntoTeachingApi.Jobs.UpsertStrategies;

/// <summary>
/// Represents an abstraction for an upsert strategy within the CRML domain.
/// Encapsulates the logic to insert or update a TModel entity while generating a corresponding log message.
/// </summary>
/// <typeparam name="TModel">
/// The type of the model to be upserted—typically a domain entity or aggregate root.
/// </typeparam>
public interface ICrmlUpsertStrategy<TModel>
{
    /// <summary>
    /// Attempts to upsert the specified model instance.
    /// Returns a success flag and outputs a log message describing the action taken.
    /// </summary>
    /// <param name="model">The entity to upsert.</param>
    /// <param name="logMessage">Outputs details of the operation (e.g., created vs updated).</param>
    /// <returns>True if the upsert was successful; false otherwise.</returns>
    bool TryUpsert(TModel model, out string logMessage);
}