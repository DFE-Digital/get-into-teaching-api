using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventRegistrationRequest
    {
        [SwaggerSchema("Set to register an existing candidate for a teaching event.")]
        public Guid? CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string AddressPostcode { get; set; }
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }
    }
}
