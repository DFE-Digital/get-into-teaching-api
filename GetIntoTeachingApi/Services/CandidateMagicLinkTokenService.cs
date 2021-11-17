using System;
using System.Linq;
using System.Security.Cryptography;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Services
{
    public class CandidateMagicLinkTokenService : ICandidateMagicLinkTokenService
    {
        public static readonly TimeSpan TokenTimeSpan = new (48, 0, 0);
        private readonly ICrmService _crm;
        private readonly IDateTimeProvider _dateTime;

        public CandidateMagicLinkTokenService(ICrmService crm, IDateTimeProvider dateTime)
        {
            _crm = crm;
            _dateTime = dateTime;
        }

        public void GenerateToken(Candidate candidate)
        {
            candidate.MagicLinkToken = CreateToken();
            candidate.MagicLinkTokenExpiresAt = _dateTime.UtcNow.AddHours(TokenTimeSpan.TotalHours);
            candidate.MagicLinkTokenStatusId = (int)Candidate.MagicLinkTokenStatus.Generated;
        }

        public CandidateMagicLinkExchangeResult Exchange(string token)
        {
            var matchingCandidates = _crm.MatchCandidates(token);

            // Return null if there are no matches and also in the very
            // unlikely case a token has been duplicated.
            if (matchingCandidates.Count() != 1)
            {
                return new CandidateMagicLinkExchangeResult(null);
            }

            var candidate = matchingCandidates.First();
            var result = new CandidateMagicLinkExchangeResult(candidate);

            candidate.MagicLinkTokenStatusId = (int)Candidate.MagicLinkTokenStatus.Exchanged;

            return result;
        }

        private static string CreateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);

            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
    }
}
