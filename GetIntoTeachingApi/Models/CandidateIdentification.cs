using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class CandidateIdentification
    {
        [SwaggerSchema("First name")]
        public string FirstName { get; set; }
        [SwaggerSchema("Last name")]
        public string LastName { get; set; }
        [SwaggerSchema("Email address", Format = "email")]
        public string Email { get; set; }
    }
}
