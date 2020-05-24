using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.OData.Edm;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "contact")]
    [Table("contacts")]
    public class Candidate : BaseModel
    {
        [Column("contactid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [EntityField(Name = "dfe_preferredteachingsubject01", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        [Column("dfe_preferredteachingsubject01")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [Column("dfe_preferrededucationphase01")]
        [EntityField(Name = "dfe_preferrededucationphase01", Type = typeof(OptionSetValue))]
        public int? PreferredEducationPhaseId { get; set; }
        [Column("dfe_isinuk")]
        [EntityField(Name = "dfe_isinuk", Type = typeof(OptionSetValue))]
        public int? LocationId { get; set; }
        [Column("dfe_ittyear")]
        [EntityField(Name = "dfe_ittyear", Type = typeof(OptionSetValue))]
        public int? InitialTeacherTrainingYearId { get; set; }
        [Column("createdon")]
        public DateTime? CreatedAt { get; set; }
        [Column("emailaddress1")]
        [EntityField(Name = "emailaddress1")]
        public string Email { get; set; }
        [Column("firstname")]
        [EntityField(Name = "firstname")]
        public string FirstName { get; set; }
        [Column("lastname")]
        [EntityField(Name = "lastname")]
        public string LastName { get; set; }
        [Column("birthdate")]
        [EntityField(Name = "birthdate")]
        public Date? DateOfBirth { get; set; }
        [Column("telephone1")]
        [EntityField(Name = "telephone1")]
        public string Telephone { get; set; }
        [Column("address1_line1")]
        [EntityField(Name = "address1_line1")]
        public string AddressLine1 { get; set; }
        [Column("address1_line2")]
        [EntityField(Name = "address1_line2")]
        public string AddressLine2 { get; set; }
        [Column("address1_line3")]
        [EntityField(Name = "address1_line3")]
        public string AddressLine3 { get; set; }
        [Column("address1_city")]
        [EntityField(Name = "address1_city")]
        public string AddressCity { get; set; }
        [Column("address1_stateorprovince")]
        [EntityField(Name = "address1_stateorprovince")]
        public string AddressState { get; set; }
        [Column("address1_postalcode")]
        [EntityField(Name = "address1_postalcode")]
        public string AddressPostcode { get; set; }
        [Column("dfe_contact_dfe_candidatequalification_ContactId")]
        [EntityRelationship(Name = "dfe_contact_dfe_candidatequalification_ContactId", Type = typeof(CandidateQualification))]
        public List<CandidateQualification> Qualifications { get; set; }
        [Column("dfe_contact_dfe_candidatepastteachingposition_ContactId")]
        [EntityRelationship(Name = "dfe_contact_dfe_candidatepastteachingposition_ContactId", Type = typeof(CandidatePastTeachingPosition))]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        [Column("dfe_contact_phonecall_contactid")]
        [EntityRelationship(Name = "dfe_contact_phonecall_contactid", Type = typeof(PhoneCall))]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        [Column("dfe_contact_dfe_candidateprivacypolicy_Candidate")]
        [EntityRelationship(Name = "dfe_contact_dfe_candidateprivacypolicy_Candidate", Type = typeof(CandidatePrivacyPolicy))]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate() : base() { }

        public Candidate(Entity entity, ICrmService crm) : base(entity, crm) { }

        protected override bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            if (propertyName != "PrivacyPolicy" || Id == null || PrivacyPolicy?.AcceptedPolicyId == null)
                return true;

            return crm.CandidateYetToAcceptPrivacyPolicy((Guid)Id, (Guid)PrivacyPolicy.AcceptedPolicyId);
        }
    }
}
