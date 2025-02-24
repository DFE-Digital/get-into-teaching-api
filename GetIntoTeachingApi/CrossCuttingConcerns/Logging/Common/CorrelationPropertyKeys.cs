namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Common
{
    /// <summary>
    /// Shared property keys used to define request correlation specific properties.
    /// </summary>
    public readonly struct CorrelationPropertyKeys
    {
        /// <summary>
        /// The correlation Id (GUID) property name key defined for each http request.
        /// </summary>
        public static readonly string PerRequestCorrelationIdPropertyNameKey = "PerRequestCorrelationId";
    }
}
