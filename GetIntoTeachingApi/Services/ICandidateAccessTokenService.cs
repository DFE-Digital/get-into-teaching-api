using System;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface ICandidateAccessTokenService
    {
        string GenerateToken(ExistingCandidateRequest request, Guid candidateId);
        bool IsValid(string token, ExistingCandidateRequest request, Guid candidateId, DateTime timestamp);
        bool IsValid(string token, ExistingCandidateRequest request, Guid candidateId);
    }
}
