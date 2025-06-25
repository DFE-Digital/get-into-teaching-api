namespace GetIntoTeachingApi.Models.Crm
{
    /// <summary>
    /// Interface that defines the contract on which objects
    /// that wish to invoke contact channel creation behaviour must adhere.
    /// </summary>
    public interface ICreateContactChannel
    {
        /// <summary>
        /// Provides the default read-only contact creation channel integer value. NB: this field will be deprecated.
        /// </summary>
        int? DefaultContactCreationChannel { get; }
        
        /// <summary>
        /// Provides the default read-only creation channel source identifier.
        /// </summary>
        int? DefaultCreationChannelSourceId { get; }
        
        /// <summary>
        /// Provides the default read-only creation channel service identifier.
        /// </summary>
        int? DefaultCreationChannelServiceId { get; }
        
        /// <summary>
        /// Provides the default read-only creation channel activity identifier.
        /// </summary>
        int? DefaultCreationChannelActivityId { get; }
        
        /// <summary>
        /// Provides the ability to assign and retrieve the channel source creation identifier.
        /// </summary>
        int? CreationChannelSourceId { get; set; }

        /// <summary>
        /// Provides the ability to assign and retrieve the channel service creation identifier.
        /// </summary>
        int? CreationChannelServiceId { get; set; }

        /// <summary>
        /// Provides the ability to assign and retrieve the channel activity creation identifier.
        /// </summary>
        int? CreationChannelActivityId { get; set; }
    }
}
