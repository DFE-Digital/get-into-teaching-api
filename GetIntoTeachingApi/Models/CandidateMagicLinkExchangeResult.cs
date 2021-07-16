using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Models
{
    public class CandidateMagicLinkExchangeResult
    {
        public enum ExchangeStatus
        {
            Valid,
            Invalid,
            Expired,
            AlreadyExchanged,
        }

        public bool Success => Status == ExchangeStatus.Valid;
        [JsonIgnore]
        public Candidate Candidate { get; }
        public ExchangeStatus Status { get; }

        public CandidateMagicLinkExchangeResult(Candidate candidate)
        {
            Candidate = candidate;

            if (Candidate == null)
            {
                Status = ExchangeStatus.Invalid;
            }
            else if (Candidate.MagicLinkTokenAlreadyExchanged())
            {
                Status = ExchangeStatus.AlreadyExchanged;
            }
            else if (Candidate.MagicLinkTokenExpired())
            {
                Status = ExchangeStatus.Expired;
            }
            else
            {
                Status = ExchangeStatus.Valid;
            }
        }
    }
}
