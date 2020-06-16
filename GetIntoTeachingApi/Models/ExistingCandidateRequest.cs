using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class ExistingCandidateRequest
    {
        private static readonly int MinimumAdditionalAttributeMatches = 2;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public bool Match(Entity entity)
        {
            if (entity == null)
            {
                return false;
            }

            return EmailMatchesCandidate(entity) && MinimumAdditionalAttributesMatch(entity);
        }

        public string Slugify()
        {
            var attributes = new[] { Email }.Concat(AdditionalAttributeValues(FirstName, LastName, DateOfBirth));
            return string.Join("-", attributes).ToLower();
        }

        private bool EmailMatchesCandidate(Entity entity)
        {
            return entity.GetAttributeValue<string>("emailaddress1").Equals(Email, StringComparison.OrdinalIgnoreCase);
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

        private bool MinimumAdditionalAttributesMatch(Entity entity)
        {
            var matches = AdditionalAttributeValues(FirstName, LastName, DateOfBirth).Intersect(
                AdditionalAttributeValues(entity.GetAttributeValue<string>("firstname"),
                    entity.GetAttributeValue<string>("lastname"),
                    entity.GetAttributeValue<DateTime>("birthdate")), StringComparer.OrdinalIgnoreCase);

            return matches.Count() >= MinimumAdditionalAttributeMatches;
        }
    }
}
