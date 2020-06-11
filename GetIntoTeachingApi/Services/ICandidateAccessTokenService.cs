using GetIntoTeachingApi.Models;
using System;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateAccessTokenService
    {
        string GenerateToken(ExistingCandidateRequest request);
        bool IsValid(string token, ExistingCandidateRequest request, DateTime timestamp);
        bool IsValid(string token, ExistingCandidateRequest request);
    }
}
