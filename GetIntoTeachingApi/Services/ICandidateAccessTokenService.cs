using GetIntoTeachingApi.Models;
using System;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateAccessTokenService
    {
        public string GenerateToken(string email);
        public bool IsValid(CandidateAccessTokenChallenge challenge, DateTime timestamp);
        public bool IsValid(CandidateAccessTokenChallenge challenge);
    }
}
