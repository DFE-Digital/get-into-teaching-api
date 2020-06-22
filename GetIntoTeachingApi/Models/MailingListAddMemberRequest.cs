using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class MailingListAddMemberRequest
    {
        [SwaggerSchema("Set to add an existing candidate to the mailing list.")]
        public Guid? CandidateId { get; set; }
        public Guid PreferredTeachingSubjectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string AddressPostcode { get; set; }
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }
    }
}
