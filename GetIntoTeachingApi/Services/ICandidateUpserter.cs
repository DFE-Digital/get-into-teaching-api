using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateUpserter
    {
        public void Upsert(Candidate candidate);
    }
}
