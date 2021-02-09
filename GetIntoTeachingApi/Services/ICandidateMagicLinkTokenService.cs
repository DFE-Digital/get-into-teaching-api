using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateMagicLinkTokenService
    {
        void GenerateToken(Candidate candidate);
        Candidate Exchange(string token);
    }
}
