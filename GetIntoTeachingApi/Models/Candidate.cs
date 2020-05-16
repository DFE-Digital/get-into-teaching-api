using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class Candidate : BaseModel
    {
        [Entity(Name = "dfe_preferredteachingsubject01", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [Entity(Name = "dfe_preferrededucationphase01", Type = typeof(OptionSetValue))]
        public int? PreferredEducationPhaseId { get; set; }
        [Entity(Name = "dfe_isinuk", Type = typeof(OptionSetValue))]
        public int? LocationId { get; set; }
        [Entity(Name = "dfe_ittyear", Type = typeof(OptionSetValue))]
        public int? InitialTeacherTrainingYearId { get; set; }
        [Entity(Name = "emailaddress1")]
        public string Email { get; set; }
        [Entity(Name = "firstname")]
        public string FirstName { get; set; }
        [Entity(Name = "lastname")]
        public string LastName { get; set; }
        [Entity(Name = "birthdate")]
        public DateTime? DateOfBirth { get; set; }
        [Entity(Name = "telephone1")]
        public string Telephone { get; set; }
        [Entity(Flatten = true)]
        public Address Address { get; set; }
        public List<CandidateQualification> Qualifications { get; set; }
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate() : base()
        {
            Qualifications = new List<CandidateQualification>();
            PastTeachingPositions = new List<CandidatePastTeachingPosition>();
        }

        public Candidate(Entity entity) : base(entity) { }
    }
}
