using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateUpserter
    {
        public void Upsert(Candidate candidate);
    }
}
