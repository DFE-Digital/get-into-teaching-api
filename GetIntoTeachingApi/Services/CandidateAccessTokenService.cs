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

        public CandidateAccessTokenService(IEnv env)
        {
            _env = env;
        }

        public string GenerateToken(ExistingCandidateRequest request)
        {
            var totp = CreateTotp(request);
            return totp.ComputeTotp();
        }

        public bool IsValid(string token, ExistingCandidateRequest request)
        {
            return IsValid(token, request, DateTime.UtcNow);
        }

        public bool IsValid(string token, ExistingCandidateRequest request, DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var totp = CreateTotp(request);

            return totp.VerifyTotp(
                timestamp,
                token,
                out _,
                new VerificationWindow(previous: VerificationWindow, future: 0));
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
