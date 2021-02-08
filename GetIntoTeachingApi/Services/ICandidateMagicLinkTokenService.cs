using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateMagicLinkTokenService
    {
        void GenerateToken(Candidate candidate);
        CandidateMagicLinkExchangeResult Exchange(string token);
    }
}
