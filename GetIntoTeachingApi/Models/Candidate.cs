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

        [Column("dfe_PreferredTeachingSubject01")]
        public TeachingSubject PreferredTeachingSubject { get; set; }
        [Column("dfe_preferrededucationphase01")]
        public int? PreferredEducationPhaseId { get; set; }
        [Column("dfe_isinuk")]
        public int? LocationId { get; set; }
        [Column("dfe_ittyear")]
        public int? InitialTeacherTrainingYearId { get; set; }
        [Column("createdon")]
        public DateTime? CreatedAt { get; set; }
        [Column("emailaddress1")]
        public string Email { get; set; }
        [Column("firstname")]
        public string FirstName { get; set; }
        [Column("lastname")]
        public string LastName { get; set; }
        [Column("birthdate")]
        public Date? DateOfBirth { get; set; }
        [Column("telephone1")]
        public string Telephone { get; set; }
        [Column("address1_line1")]
        public string AddressLine1 { get; set; }
        [Column("address1_line2")]
        public string AddressLine2 { get; set; }
        [Column("address1_line3")]
        public string AddressLine3 { get; set; }
        [Column("address1_city")]
        public string AddressCity { get; set; }
        [Column("address1_stateorprovince")]
        public string AddressState { get; set; }
        [Column("address1_postalcode")]
        public string AddressPostcode { get; set; }
        [Column("dfe_contact_dfe_candidatequalification_ContactId")]
        public List<CandidateQualification> Qualifications { get; set; }
        [Column("dfe_contact_dfe_candidatepastteachingposition_ContactId")]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        [Column("dfe_contact_phonecall_contactid")]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        [Column("dfe_contact_dfe_candidateprivacypolicy_Candidate")]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate() : base() { }

        public Candidate(Entity entity, ICrmService crm) : base(entity, crm) { }

        /* TODO: protected override bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            if (propertyName != "PrivacyPolicy" || Id == null || PrivacyPolicy?.AcceptedPolicyId == null)
                return true;

            return crm.CandidateYetToAcceptPrivacyPolicy((Guid)Id, (Guid)PrivacyPolicy.AcceptedPolicyId);
        }*/
    }
}
