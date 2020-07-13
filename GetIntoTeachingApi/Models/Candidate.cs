using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Crm.Sdk;
using Microsoft.EntityFrameworkCore.Internal;
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

        public enum AssignmentStatus
        {
            WaitingToBeAssigned = 222750001,
        }

        public enum GdprConsent
        {
            Consent = 587030001,
        }

        public enum PhoneNumberType
        {
            Home = 222750001,
        }

        public enum ContactMethod
        {
            Any = 1,
        }

        public enum PreferredEducationPhase
        {
            Primary = 222750000,
            Secondary = 222750001,
        }

        public enum Channel
        {
            MailingList = 222750028,
            Event = 222750029,
            TeacherTrainingAdviser = 222750027,
        }

        public enum GcseStatus
        {
            HasOrIsPlanningOnRetaking = 222750000,
        }

        [JsonIgnore]
        public string FullName => $"{this.FirstName} {this.LastName}";
        [EntityField("dfe_preferredteachingsubject01", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [EntityField("dfe_country", typeof(EntityReference), "dfe_country")]
        public Guid? CountryId { get; set; }
        [EntityField("dfe_preferrededucationphase01", typeof(OptionSetValue))]
        public int? PreferredEducationPhaseId { get; set; }
        [EntityField("dfe_ittyear", typeof(OptionSetValue))]
        public int? InitialTeacherTrainingYearId { get; set; }
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("dfe_websitehasgcseenglish", typeof(OptionSetValue))]
        public int? HasGcseEnglishId { get; set; }
        [EntityField("dfe_websitehasgcsemaths", typeof(OptionSetValue))]
        public int? HasGcseMathsId { get; set; }
        [EntityField("dfe_websitehasgcsescience", typeof(OptionSetValue))]
        public int? HasGcseScienceId { get; set; }
        [EntityField("dfe_websiteplanningretakeenglishgcse", typeof(OptionSetValue))]
        public int? PlanningToRetakeGcseEnglishId { get; set; }
        [EntityField("dfe_websiteplanningretakemathsgcse", typeof(OptionSetValue))]
        public int? PlanningToRetakeGcseMathsId { get; set; }
        [EntityField("dfe_websiteplanningretakesciencegcse", typeof(OptionSetValue))]
        public int? PlanningToRetakeCgseScienceId { get; set; }
        [EntityField("dfe_websitedescribeyourself", typeof(OptionSetValue))]
        public int? DescribeYourselfOptionId { get; set; }
        [EntityField("dfe_websitewhereinconsiderationjourney", typeof(OptionSetValue))]
        public int? ConsiderationJourneyStageId { get; set; }
        [EntityField("dfe_typeofcandidate", typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [EntityField("dfe_candidatestatus", typeof(OptionSetValue))]
        public int? StatusId { get; set; }
        [EntityField("dfe_iscandidateeligibleforadviser", typeof(OptionSetValue))]
        public int? AdviserEligibilityId { get; set; }
        [EntityField("dfe_isadvisorrequiredos", typeof(OptionSetValue))]
        public int? AdviserRequirementId { get; set; }
        [JsonIgnore]
        [EntityField("dfe_preferredphonenumbertype", typeof(OptionSetValue))]
        public int? PreferredPhoneNumberTypeId { get; set; } = (int)PhoneNumberType.Home;
        [JsonIgnore]
        [EntityField("preferredcontactmethodcode", typeof(OptionSetValue))]
        public int? PreferredContactMethodId { get; set; } = (int)ContactMethod.Any;
        [JsonIgnore]
        [EntityField("msgdpr_gdprconsent", typeof(OptionSetValue))]
        public int? GdprConsentId { get; set; } = (int)GdprConsent.Consent;
        [JsonIgnore]
        [EntityField("dfe_waitingtobeassigneddate")]
        public DateTime? StatusIsWaitingToBeAssignedAt { get; set; }
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
        [EntityField("dfe_websitecallbackdescription")]
        public string CallbackInformation { get; set; }
        [EntityField("dfe_dfesnumber")]
        public string TeacherId { get; set; }
        [EntityField("dfe_eligibilityrulespassed")]
        public string EligibilityRulesPassed { get; set; }
        [EntityField("donotbulkemail")]
        public bool? DoNotBulkEmail { get; set; }
        [EntityField("donotbulkpostalmail")]
        public bool? DoNotBulkPostalMail { get; set; }
        [EntityField("donotemail")]
        public bool? DoNotEmail { get; set; }
        [EntityField("donotpostalmail")]
        public bool? DoNotPostalMail { get; set; }
        [EntityField("donotsendmm")]
        public bool? DoNotSendMm { get; set; }
        [EntityField("dfe_optoutsms")]
        public bool? OptOutOfSms { get; set; }
        [JsonIgnore]
        [EntityField("msdyn_gdproptout")]
        public bool? OptOutOfGdpr { get; set; } = false;
        [JsonIgnore]
        [EntityField("dfe_newregistrant")]
        public bool IsNewRegistrant { get; set; }

        [EntityRelationship("dfe_contact_dfe_servicesubscription_contact", typeof(Subscription))]
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        [EntityRelationship("msevtmgt_contact_msevtmgt_eventregistration_Contact", typeof(TeachingEventRegistration))]
        public List<TeachingEventRegistration> TeachingEventRegistrations { get; set; } = new List<TeachingEventRegistration>();
        [EntityRelationship("dfe_contact_dfe_candidatequalification_ContactId", typeof(CandidateQualification))]
        public List<CandidateQualification> Qualifications { get; set; } = new List<CandidateQualification>();
        [EntityRelationship("dfe_contact_dfe_candidatepastteachingposition_ContactId", typeof(CandidatePastTeachingPosition))]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; } = new List<CandidatePastTeachingPosition>();
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

        public bool HasActiveSubscriptionToService(Subscription.ServiceType service)
        {
            return GetActiveSubscriptionToService(service) != null;
        }

        public Subscription GetActiveSubscriptionToService(Subscription.ServiceType service)
        {
            return Subscriptions.FirstOrDefault(s => s.StatusId == (int)Subscription.SubscriptionStatus.Active && s.TypeId == (int)service);
        }

        protected override bool ShouldMap(ICrmService crm)
        {
            IsNewRegistrant = Id == null;

            if (StatusId == (int)AssignmentStatus.WaitingToBeAssigned)
            {
                StatusIsWaitingToBeAssignedAt = DateTime.Now;
            }

            return base.ShouldMap(crm);
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
                case "Subscriptions" when Id != null && value.Id == null:
                    return crm.CandidateYetToSubscribeToServiceOfType((Guid)Id, value.TypeId);
                case "TeachingEventRegistrations" when Id != null && value.Id == null:
                    return crm.CandidateYetToRegisterForTeachingEvent((Guid)Id, value.EventId);
                default:
                    return true;
            }
        }
    }
}
