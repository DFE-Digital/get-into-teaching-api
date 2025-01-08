using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CandidateIdentifier : ValueObject<CandidateIdentifier>
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CandidateId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidateId"></param>
        /// <exception cref="ArgumentException"></exception>
        public CandidateIdentifier(Guid candidateId)
        {
            if (candidateId == Guid.Empty)
                throw new ArgumentException("Candidate must have a valid GUID assigned.");

            CandidateId = candidateId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetPropertiesForEqualityCheck()
        {
            yield return CandidateId;
        }
    }
}
