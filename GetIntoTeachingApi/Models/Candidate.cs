﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Entity("contact")]
    public class Candidate : BaseModel
    {
        public enum Status
        {
            Active,
            Inactive,
        }

        [JsonIgnore]
        public string FullName => $"{this.FirstName} {this.LastName}";
        [EntityField("dfe_preferredteachingsubject01", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [EntityField("dfe_preferrededucationphase01", typeof(OptionSetValue))]
        public int? PreferredEducationPhaseId { get; set; }
        [EntityField("dfe_ittyear", typeof(OptionSetValue))]
        public int? InitialTeacherTrainingYearId { get; set; }
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("emailaddress1")]
        public string Email { get; set; }
        [EntityField("firstname")]
        public string FirstName { get; set; }
        [EntityField("lastname")]
        public string LastName { get; set; }
        [EntityField("birthdate")]
        public DateTime? DateOfBirth { get; set; }
        [EntityField("address1_telephone1")]
        public string Telephone { get; set; }
        [EntityField("address1_line1")]
        public string AddressLine1 { get; set; }
        [EntityField("address1_line2")]
        public string AddressLine2 { get; set; }
        [EntityField("address1_line3")]
        public string AddressLine3 { get; set; }
        [EntityField("address1_city")]
        public string AddressCity { get; set; }
        [EntityField("address1_stateorprovince")]
        public string AddressState { get; set; }
        [EntityField("address1_postalcode")]
        public string AddressPostcode { get; set; }

        [EntityRelationship("dfe_contact_dfe_candidatequalification_ContactId", typeof(CandidateQualification))]
        public List<CandidateQualification> Qualifications { get; set; }
        [EntityRelationship("dfe_contact_dfe_candidatepastteachingposition_ContactId", typeof(CandidatePastTeachingPosition))]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        [EntityRelationship("dfe_contact_phonecall_contactid", typeof(PhoneCall))]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        [EntityRelationship("dfe_contact_dfe_candidateprivacypolicy_Candidate", typeof(CandidatePrivacyPolicy))]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate()
            : base()
        {
        }

        public Candidate(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }

        protected override bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            if (value == null)
            {
                return false;
            }

            switch (propertyName)
            {
                case "PhoneCall":
                    value.PopulateWithCandidate(this);
                    return true;
                case "PrivacyPolicy" when Id != null:
                    return crm.CandidateYetToAcceptPrivacyPolicy((Guid)Id, value.AcceptedPolicyId);
                default:
                    return true;
            }
        }
    }
}
