using System;
using System.Text;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using OtpNet;

namespace GetIntoTeachingApi.Services
{
    public class CandidateAccessTokenService : ICandidateAccessTokenService
    {
        // The amount of time a user has to verify their access token is:
        // (VerificationWindow * StepsInSeconds) + Remaining Seconds in Current Step
        public static readonly int VerificationWindow = 30;
        public static readonly int StepInSeconds = 30;
        private const int Length = 6;
        private readonly IEnv _env;
        private readonly IMetricService _metrics;

        public CandidateAccessTokenService(IEnv env, IMetricService metrics)
        {
            _env = env;
            _metrics = metrics;
        }

        public string GenerateToken(ExistingCandidateRequest request, Guid candidateId)
        {
            var totp = CreateTotp(request).ComputeTotp();

            _metrics.GeneratedTotps.WithLabels(candidateId.ToString(), totp).Inc();

            return totp;
        }

        public bool IsValid(string token, ExistingCandidateRequest request, Guid candidateId)
        {
            return IsValid(token, request, candidateId, DateTime.UtcNow);
        }

        public bool IsValid(string token, ExistingCandidateRequest request, Guid candidateId, DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var totp = CreateTotp(request);

            var valid = totp.VerifyTotp(
                timestamp,
                token,
                out _,
                new VerificationWindow(previous: VerificationWindow, future: 0));

            _metrics.VerifiedTotps.WithLabels(candidateId.ToString(), token, valid.ToString()).Inc();

            return valid;
        }

        private Totp CreateTotp(ExistingCandidateRequest request)
        {
            return new Totp(CompoundSharedSecretBytes(request.Slugify()), totpSize: Length, step: StepInSeconds);
        }

        private byte[] CompoundSharedSecretBytes(string slug)
        {
            return Encoding.ASCII.GetBytes(slug + TotpSecretKey());
        }

        private string TotpSecretKey() => _env.TotpSecretKey;
    }
}
