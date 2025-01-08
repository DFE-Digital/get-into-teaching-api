using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CandidateProfileIdentifier : ValueObject<CandidateProfileIdentifier>
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CandidateProfileId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidateProfileId"></param>
        /// <exception cref="ArgumentException"></exception>
        public CandidateProfileIdentifier(Guid candidateProfileId)
        {
            if (candidateProfileId == Guid.Empty)
                throw new ArgumentException("Candidate Profile must have a valid GUID assigned.");

            CandidateProfileId = candidateProfileId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetPropertiesForEqualityCheck()
        {
            yield return CandidateProfileId;
        }
    }
}
