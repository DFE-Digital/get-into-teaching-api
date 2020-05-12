using GetIntoTeachingApi.Models;
using OtpNet;
using System;
using System.Text;

namespace GetIntoTeachingApi.Services
{
    public class CandidateAccessTokenService : ICandidateAccessTokenService
    {
        // The amount of time a user has to verify their access token is:
        // (VerificationWindow * StepsInSeconds) + Remaining Seconds in Current Step
        public static readonly int VerificationWindow = 2;
        public static readonly int StepInSeconds = 30;
        private static readonly int Length = 6;

        public string GenerateToken(ExistingCandidateRequest request)
        {
           var  totp = CreateTotp(request);
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

            long timeWindowUsed;
            return totp.VerifyTotp(
                timestamp,
                token,
                out timeWindowUsed,
                new VerificationWindow(previous: VerificationWindow, future: VerificationWindow)
            );
        }

        private Totp CreateTotp(ExistingCandidateRequest request)
        {
            return new Totp(CompoundSharedSecretBytes(request.Slugify()), totpSize: Length, step: StepInSeconds);
        }

        private byte[] CompoundSharedSecretBytes(string slug)
        {
            return Encoding.ASCII.GetBytes(slug + TotpSecretKey());
        }

        private string TotpSecretKey()
        {
            return Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
        }
    }
}
