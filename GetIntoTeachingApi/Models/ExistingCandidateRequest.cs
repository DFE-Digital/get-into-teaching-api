using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;

namespace GetIntoTeachingApi.Models
{
    public class ExistingCandidateRequest
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
            if (candidate == null) return false;

            return EmailMatchesCandidate(candidate) && MinimumAdditionalAttributesMatch(candidate);
        }

        public string Slugify()
        {
            var attributes = new[] {Email}.Concat(AdditionalAttributeValues(FirstName, LastName, DateOfBirth));
            return string.Join("-", attributes).ToLower();
        }

        private bool EmailMatchesCandidate(Candidate candidate)
        {
            return candidate.Email.Equals(Email, StringComparison.OrdinalIgnoreCase);
        }

        private string[] AdditionalAttributeValues(string firstName, string lastName, DateTime? dateOfBirth)
        {
            return new[]
                {
                    firstName,
                    lastName,
                    dateOfBirth?.Date.ToString("MM-dd-yyyy")
                }
                .Where(s => s != null)
                .ToArray();
        }

        private bool MinimumAdditionalAttributesMatch(Candidate candidate)
        {
            var matches = AdditionalAttributeValues(FirstName, LastName, DateOfBirth).Intersect(
                AdditionalAttributeValues(candidate.FirstName, candidate.LastName, candidate.DateOfBirth),
                StringComparer.OrdinalIgnoreCase
            );

            return matches.Count() >= MinimumAdditionalAttributeMatches;
        }
    }
}
