using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class ExistingCandidateRequest
    {
        private const int MinimumAdditionalAttributeMatches = 2;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Reference { get; set; }

        public bool IsFullMatch(Entity entity)
        {
            return IsEmailMatch(entity) && MinimumAdditionalAttributesMatch(entity);
        }

        public bool IsEmailMatch(Entity entity)
        {
            if (entity == null)
            {
                return false;
            }

            return EmailMatchesCandidate(entity);
        }

        public string Slugify()
        {
            var attributes = new[] { Email }.Concat(AdditionalAttributeValues(FirstName, LastName, DateOfBirth));
            return string.Join("-", attributes).ToLower();
        }

        private bool EmailMatchesCandidate(Entity entity)
        {
            return entity.GetAttributeValue<string>("emailaddress1").Trim().Equals(Email, StringComparison.OrdinalIgnoreCase);
        }

        private string[] AdditionalAttributeValues(string firstName, string lastName, DateTime? dateOfBirth)
        {
            return new[]
                {
                    firstName,
                    lastName,
                    dateOfBirth?.Date.ToString("MM-dd-yyyy"),
                }
                .Where(s => s != null)
                .ToArray();
        }

        private bool MinimumAdditionalAttributesMatch(Entity entity)
        {
            var matches = AdditionalAttributeValues(FirstName, LastName, DateOfBirth).Intersect(
                AdditionalAttributeValues(
                    entity.GetAttributeValue<string>("firstname")?.Trim(),
                    entity.GetAttributeValue<string>("lastname")?.Trim(),
                    entity.GetAttributeValue<DateTime>("birthdate")), StringComparer.OrdinalIgnoreCase);

            return matches.Count() >= MinimumAdditionalAttributeMatches;
        }
    }
}
