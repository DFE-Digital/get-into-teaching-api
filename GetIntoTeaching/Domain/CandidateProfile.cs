using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CandidateProfile : AggregateRoot<CandidateProfileIdentifier>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidateProfileIdentifier"></param>
        /// <param name="candidate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CandidateProfile
        (
            CandidateProfileIdentifier candidateProfileIdentifier,
            Candidate candidate
        )
            : base(candidateProfileIdentifier)
        {
            TrainingCandidate = candidate ??
                throw new ArgumentNullException(nameof(candidate));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        public CandidateProfile(Candidate candidate)
            : this(new CandidateProfileIdentifier(Guid.NewGuid()), candidate){
        }

        /// <summary>
        /// 
        /// </summary>
        public Candidate TrainingCandidate { get; }
    }
}