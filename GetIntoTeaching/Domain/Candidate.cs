using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Candidate : Entity<CandidateIdentifier>
    {
        /// <summary>
        /// 
        /// </summary>
        public CandidateName Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Candidate(CandidateName name) : this(new CandidateIdentifier(Guid.NewGuid()), name){
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidateIdentifier"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Candidate(CandidateIdentifier candidateIdentifier, CandidateName name) :
            base(candidateIdentifier)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
