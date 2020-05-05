using Swashbuckle.AspNetCore.Annotations;
using System;

namespace GetIntoTeachingApi.Models
{
    public class CandidateAccessTokenRequest
    {
        [SwaggerSchema("First name")]
        public string FirstName { get; set; }
        [SwaggerSchema("Last name")]
        public string LastName { get; set; }
        [SwaggerSchema("Email address", Format = "email")]
        public string Email { get; set; }
        [SwaggerSchema("Date of birth", Format = "date")]
        public DateTime? DateOfBirth { get; set; }
    }
}
