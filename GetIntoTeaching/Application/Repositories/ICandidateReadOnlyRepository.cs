using GetIntoTeaching.Domain;

namespace GetIntoTeaching.Application.Repositories
{
    public interface ICandidateReadOnlyRepository
    {
        Task<Candidate> GetCandidateByIdentifiert(Guid identifier);
    }
}
