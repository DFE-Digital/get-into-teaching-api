using System;

namespace GetIntoTeachingApi.Models
{
    public class CandidateAccessTokenChallenge
    {
        public string Token { get; set; }
        public string Email { get; set; }

        public bool HasToken()
        {
            return !string.IsNullOrWhiteSpace(Token);
        }
    }
}
