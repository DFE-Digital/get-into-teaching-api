namespace GetIntoTeachingApi.Models.Crm
{
    /// <summary>
    /// Interface that defines the contract on which objects
    /// that wish to invoke contact channel creation behaviour must adhere.
    /// </summary>
    public interface IAdditionalContactChannel
    {
        /// <summary>
        /// Provides the default read-only creation channel source identifier.
        /// </summary>
        int? DefaultCreationChannelSourceId { get; }
        
        /// <summary>
        /// Provides the default read-only creation channel service identifier.
        /// </summary>
        int? DefaultCreationChannelServiceId{ get; }
        
        /// <summary>
        /// Provides the default read-only creation channel activity identifier.
        /// </summary>
        int? DefaultCreationChannelActivityId { get; }
    }
}
