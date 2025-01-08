using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CandidateName : ValueObject<CandidateName>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Firstname { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="surname"></param>
        /// <exception cref="PupilRegistrationException"></exception>
        public CandidateName(string firstname, string surname)
        {
            if (string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentException("The 'Candidates firstname' field is required.");
            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("The 'Candidates surname' field is required.");

            Firstname = firstname;
            Surname = surname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetPropertiesForEqualityCheck()
        {
            yield return Firstname;
            yield return Surname;
        }
    }
}
