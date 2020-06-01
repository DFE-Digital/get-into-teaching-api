using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "contact")]
    public class Candidate : BaseModel
    {
        [EntityField(Name = "dfe_preferredteachingsubject01", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [EntityField(Name = "dfe_preferrededucationphase01", Type = typeof(OptionSetValue))]
        public int? PreferredEducationPhaseId { get; set; }
        [EntityField(Name = "dfe_isinuk", Type = typeof(OptionSetValue))]
        public int? LocationId { get; set; }
        [EntityField(Name = "dfe_ittyear", Type = typeof(OptionSetValue))]
        public int? InitialTeacherTrainingYearId { get; set; }
        [EntityField(Name = "emailaddress1")]
        public string Email { get; set; }
        [EntityField(Name = "firstname")]
        public string FirstName { get; set; }
        [EntityField(Name = "lastname")]
        public string LastName { get; set; }
        [EntityField(Name = "birthdate")]
        public DateTime? DateOfBirth { get; set; }
        [EntityField(Name = "telephone1")]
        public string Telephone { get; set; }
        [EntityField(Name = "address1_line1")]
        public string AddressLine1 { get; set; }
        [EntityField(Name = "address1_line2")]
        public string AddressLine2 { get; set; }
        [EntityField(Name = "address1_line3")]
        public string AddressLine3 { get; set; }
        [EntityField(Name = "address1_city")]
        public string AddressCity { get; set; }
        [EntityField(Name = "address1_stateorprovince")]
        public string AddressState { get; set; }
        [EntityField(Name = "address1_postalcode")]
        public string AddressPostcode { get; set; }
        [EntityRelationship(Name = "dfe_contact_dfe_candidatequalification_ContactId", Type = typeof(CandidateQualification))]
        public List<CandidateQualification> Qualifications { get; set; }
        [EntityRelationship(Name = "dfe_contact_dfe_candidatepastteachingposition_ContactId", Type = typeof(CandidatePastTeachingPosition))]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        [EntityRelationship(Name = "dfe_contact_phonecall_contactid", Type = typeof(PhoneCall))]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        [EntityRelationship(Name = "dfe_contact_dfe_candidateprivacypolicy_Candidate", Type = typeof(CandidatePrivacyPolicy))]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate() : base() { }

        public Candidate(Entity entity, ICrmService crm) : base(entity, crm) { }

        protected override bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            if (propertyName != "PrivacyPolicy" || Id == null || PrivacyPolicy == null)
                return true;

            return crm.CandidateYetToAcceptPrivacyPolicy((Guid)Id, (Guid)PrivacyPolicy.AcceptedPolicyId);
        }
    }
}
