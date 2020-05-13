using GetIntoTeachingApi.Models;
using System;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateAccessTokenService
    {
        public string GenerateToken(ExistingCandidateRequest request);
        public bool IsValid(string token, ExistingCandidateRequest request, DateTime timestamp);
        public bool IsValid(string token, ExistingCandidateRequest request);
    }
}
