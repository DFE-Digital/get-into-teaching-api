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

        public string GenerateToken(string email)
        {
           var  totp = CreateTotp(email);
            return totp.ComputeTotp();
        }

        public bool IsValid(CandidateAccessTokenChallenge challenge)
        {
            return IsValid(challenge, DateTime.UtcNow);
        }

        public bool IsValid(CandidateAccessTokenChallenge challenge, DateTime timestamp)
        {
            if (!challenge.HasToken())
            {
                return false;
            }

            var totp = CreateTotp(challenge.Email);

            long timeWindowUsed;
            return totp.VerifyTotp(
                timestamp,
                challenge.Token,
                out timeWindowUsed,
                new VerificationWindow(previous: VerificationWindow, future: VerificationWindow)
            );
        }

        private Totp CreateTotp(string email)
        {
            return new Totp(CompoundSharedSecretBytes(email), totpSize: Length, step: StepInSeconds);
        }

        private byte[] CompoundSharedSecretBytes(string email)
        {
            return Encoding.ASCII.GetBytes(email + TotpSecretKey());
        }

        private string TotpSecretKey()
        {
            return Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
        }
    }
}
