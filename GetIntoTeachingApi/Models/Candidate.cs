using System;
using System.Collections.Generic;
using System.Linq;
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

        public enum AdviserEligibility
        {
            Yes = 222750000,
        }

        public enum AdviserRequirement
        {
            Yes = 222750000,
        }

        public enum AssignmentStatus
        {
            WaitingToBeAssigned = 222750001,
        }

        public enum Type
        {
            InterestedInTeacherTraining = 222750000,
            ReturningToTeacherTraining = 222750001,
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

        public enum SubscriptionChannel
        {
            TeacherTrainingAdviser = 222750000,
            MailingList = 222750000,
            Events = 222750000,
        }

        public enum SubscriptionType
        {
            SingleEvent = 222750001,
            LocalEvent = 222750000,
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
            NotAnswered = 222750001,
        }

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
        public int? PlanningToRetakeGcseScienceId { get; set; }
        [EntityField("dfe_websitewhereinconsiderationjourney", typeof(OptionSetValue))]
        public int? ConsiderationJourneyStageId { get; set; }
        [EntityField("dfe_typeofcandidate", typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [EntityField("dfe_candidatestatus", typeof(OptionSetValue))]
        public int? AssignmentStatusId { get; set; }
        [EntityField("dfe_iscandidateeligibleforadviser", typeof(OptionSetValue))]
        public int? AdviserEligibilityId { get; set; }
        [EntityField("dfe_isadvisorrequiredos", typeof(OptionSetValue))]
        public int? AdviserRequirementId { get; set; }
        [EntityField("dfe_preferredphonenumbertype", typeof(OptionSetValue))]
        public int? PreferredPhoneNumberTypeId { get; set; } = (int)PhoneNumberType.Home;
        [EntityField("preferredcontactmethodcode", typeof(OptionSetValue))]
        public int? PreferredContactMethodId { get; set; } = (int)ContactMethod.Any;
        [EntityField("msgdpr_gdprconsent", typeof(OptionSetValue))]
        public int? GdprConsentId { get; set; } = (int)GdprConsent.Consent;
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
        [EntityField("address1_city")]
        public string AddressCity { get; set; }
        [EntityField("address1_postalcode")]
        public string AddressPostcode { get; set; }
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
        [EntityField("msdyn_gdproptout")]
        public bool? OptOutOfGdpr { get; set; } = false;
        [EntityField("dfe_newregistrant")]
        public bool IsNewRegistrant { get; set; }

        [EntityField("dfe_GITISTTAServiceIsSubscriber")]
        public bool? HasTeacherTrainingAdviserSubscription { get; set; }
        [EntityField("dfe_GITISTTAServiceSubscriptionChannel", typeof(OptionSetValue))]
        public int? TeacherTrainingAdviserSubscriptionChannelId { get; set; }
        [EntityField("dfe_GITISTTAServiceStartDate")]
        public DateTime? TeacherTrainingAdviserSubscriptionStartAt { get; set; }
        [EntityField("dfe_GITISTTAServiceDoNotBulkEmail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_GITISTTAServiceDoNotBulkPostalMail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_GITISTTAServiceDoNotEmail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_GITISTTAServiceDoNotPostalMail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_GITISTTAServiceDoNotSendMM")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotSendMm { get; set; }

        [EntityField("dfe_GITISMailingListServiceIsSubscriber")]
        public bool? HasMailingListSubscription { get; set; }
        [EntityField("dfe_GITISMailingListServiceSubscriptionChannel", typeof(OptionSetValue))]
        public int? MailingListSubscriptionChannelId { get; set; }
        [EntityField("dfe_GITISMailingListServiceStartDate")]
        public DateTime? MailingListSubscriptionStartAt { get; set; }
        [EntityField("dfe_GITISMailingListServiceDoNotBulkEmail")]
        public bool? MailingListSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_GITISMailingListServiceDoNotBulkPostalMail")]
        public bool? MailingListSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_GITISMailingListServiceDoNotEmail")]
        public bool? MailingListSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_GITISMailingListServiceDoNotPostalMail")]
        public bool? MailingListSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_GITISMailingListServiceDoNotSendMM")]
        public bool? MailingListSubscriptionDoNotSendMm { get; set; }

        [EntityField("dfe_GITISEventsServiceIsSubscriber")]
        public bool? HasEventsSubscription { get; set; }
        [EntityField("dfe_GITISEventsServiceSubscriptionChannel", typeof(OptionSetValue))]
        public int? EventsSubscriptionChannelId { get; set; }
        [EntityField("GITISEventsServiceSubscriptionType", typeof(OptionSetValue))]
        public int? EventsSubscriptionTypeId { get; set; }
        [EntityField("dfe_GITISEventsServiceStartDate")]
        public DateTime? EventsSubscriptionStartAt { get; set; }
        [EntityField("dfe_GITISEventsServiceDoNotBulkEmail")]
        public bool? EventsSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_GITISEventsServiceDoNotBulkPostalMail")]
        public bool? EventsSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_GITISEventsServiceDoNotEmail")]
        public bool? EventsSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_GITISEventsServiceDoNotPostalMail")]
        public bool? EventsSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_GITISEventsServiceDoNotSendMM")]
        public bool? EventsSubscriptionDoNotSendMm { get; set; }

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

        public bool IsReturningToTeaching()
        {
            return PastTeachingPositions.Count > 0;
        }

        public bool HasGcseMathsAndEnglish()
        {
            return new[] { HasGcseEnglishId, HasGcseMathsId }.All(g => g == (int)GcseStatus.HasOrIsPlanningOnRetaking);
        }

        public bool IsPlanningToRetakeGcseMathsAndEnglish()
        {
            return new[] { PlanningToRetakeGcseMathsId, PlanningToRetakeGcseEnglishId }.All(g => g == (int)GcseStatus.HasOrIsPlanningOnRetaking);
        }

        protected override bool ShouldMap(ICrmService crm)
        {
            IsNewRegistrant = Id == null;

            if (AssignmentStatusId == (int)AssignmentStatus.WaitingToBeAssigned)
            {
                StatusIsWaitingToBeAssignedAt = DateTime.UtcNow;
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
                default:
                    return true;
            }
        }
    }
}
