using System;
using System.Globalization;
using System.Linq;

namespace GetIntoTeachingApi.Models
{
    public class ExistingCandidateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Reference { get; set; }

        public string Slugify()
        {
            var attributes = new[] { Email }.Concat(AdditionalAttributeValues(FirstName, LastName, DateOfBirth));
            return string.Join("-", attributes).ToLower(CultureInfo.CurrentCulture);
        }

        private static string[] AdditionalAttributeValues(string firstName, string lastName, DateTime? dateOfBirth)
        {
            return new[]
                {
                    firstName,
                    lastName,
                    dateOfBirth?.Date.ToString("MM-dd-yyyy", CultureInfo.CurrentCulture),
                }
                .Where(s => s != null)
                .ToArray();
        }
    }
}
