using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace GetIntoTeachingApi.Models
{
    public class CandidateAccessTokenRequest
    {
        private static readonly int MinimumAdditionalAttributeMatches = 2;

        [SwaggerSchema("First name")]
        public string FirstName { get; set; }
        [SwaggerSchema("Last name")]
        public string LastName { get; set; }
        [SwaggerSchema("Email address", Format = "email")]
        public string Email { get; set; }
        [SwaggerSchema("Date of birth", Format = "date")]
        public DateTime? DateOfBirth { get; set; }

        public bool Match(Candidate candidate)
        {
            if (candidate == null || !candidate.Email.Equals(Email, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var additionalAttributeMatches = new[] {
                (FirstName != null && FirstName.Equals(candidate.FirstName, StringComparison.OrdinalIgnoreCase)),
                (LastName != null && LastName.Equals(candidate.LastName, StringComparison.OrdinalIgnoreCase)),
                (DateOfBirth != null && DateOfBirth?.Date == candidate.DateOfBirth?.Date) 
            };

            return additionalAttributeMatches.Where(m => m).Count() >= MinimumAdditionalAttributeMatches;
        }
    }
}
