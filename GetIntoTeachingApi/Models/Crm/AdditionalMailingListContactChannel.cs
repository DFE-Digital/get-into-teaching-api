using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class AdditionalMailingListContactChannel : IAdditionalContactChannel
    {
        /// <summary>
        /// Provides the default read-only additional creation channel source identifier.
        /// </summary>
        public int? DefaultCreationChannelSourceId => 
            (int?)ContactChannelCreation.CreationChannelSource.GITWebsite;
        
        /// <summary>
        /// Provides the default read-only additional creation channel service identifier.
        /// </summary>
        public int? DefaultCreationChannelServiceId => 
            (int?)ContactChannelCreation.CreationChannelService.MailingList;
        
        /// <summary>
        /// Provides the default read-only additional creation channel activity identifier.
        /// </summary>
        public int? DefaultCreationChannelActivityId => null;
    }
}
