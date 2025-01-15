using GetIntoTeaching.Application.Repositories;
using GetIntoTeaching.Core.Infrastructure.Persistence;
using GetIntoTeaching.Domain;

namespace GetIntoTeaching.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CandidateReadOnlyRepository : ICandidateReadOnlyRepository
    {
        private readonly ICrmQueryHandler _crmQueryHandler;

        /// <summary>
        /// 
        /// </summary>
        public CandidateReadOnlyRepository(ICrmQueryHandler crmQueryHandler)
        {
            _crmQueryHandler = crmQueryHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Candidate> GetCandidateByIdentifiert(Guid identifier)
        {
            throw new NotImplementedException();
        }
    }
}
