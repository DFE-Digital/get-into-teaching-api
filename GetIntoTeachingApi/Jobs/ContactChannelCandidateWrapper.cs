using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Jobs;

public class ContactChannelCandidateWrapper : ICreateContactChannel
{
  
        /// <summary>
        /// Provides the default read-only contact creation channel integer value.
        /// </summary>
        public int? DefaultContactCreationChannel =>
            (int?)Candidate.Channel.ApplyForTeacherTraining;

        /// <summary>
        /// Provides the ability to assign and retrieve the channel source creation identifier.
        /// </summary>
        public int? CreationChannelSourceId { get; set; }

        /// <summary>
        /// Provides the ability to assign and retrieve the channel service creation identifier.
        /// </summary>
        public int? CreationChannelServiceId { get; set; }

        /// <summary>
        /// Provides the ability to assign and retrieve the channel activity creation identifier.
        /// </summary>
        public int? CreationChannelActivityId { get; set; }
        
 
        public Candidate ScopedCandidate { get; }
        
        // Todo: add some documentation
        public ContactChannelCandidateWrapper(Candidate candidate)
        {
               ScopedCandidate = candidate;
        }
}