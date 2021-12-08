using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.Crm
{
    [Entity("contact")]
    [SwaggerIgnore]
    public class Candidate : BaseModel
    {
        public static readonly Guid AdviserBusinessUnitId = new Guid("1A61F629-F502-E911-A972-000D3A23443B");

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
            Yes = 222750001,
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
            SchoolsExperience = 222750021,
            GetIntoTeachingCallback = 222750043,
            ApplyForTeacherTraining = 222750025,
        }

        public enum GcseStatus
        {
            HasOrIsPlanningOnRetaking = 222750000,
            NotAnswered = 222750001,
        }

        public enum MagicLinkTokenStatus
        {
            Pending = 222750002,
            Generated = 222750003,
            Exchanged = 222750004,
        }

        public enum RegistrationStatus
        {
            ReRegistered = 222750000,
        }

        public string FullName => $"{FirstName} {LastName}".NullIfEmptyOrWhitespace();
        [EntityField("dfe_preferredteachingsubject01", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? PreferredTeachingSubjectId { get; set; }
        [EntityField("dfe_preferredteachingsubject02", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? SecondaryPreferredTeachingSubjectId { get; set; }
        [EntityField("dfe_country", typeof(EntityReference), "dfe_country")]
        public Guid? CountryId { get; set; }
        [EntityField("owningbusinessunit", typeof(EntityReference), "businessunit")]
        public Guid? OwningBusinessUnitId { get; set; }
        [EntityField("masterid", typeof(EntityReference), "contact")]
        public Guid? MasterId { get; set; }
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
        public int? PreferredPhoneNumberTypeId { get; set; }
        [EntityField("preferredcontactmethodcode", typeof(OptionSetValue))]
        public int? PreferredContactMethodId { get; set; }
        [EntityField("msgdpr_gdprconsent", typeof(OptionSetValue))]
        public int? GdprConsentId { get; set; }
        [EntityField("dfe_websitemltokenstatus", typeof(OptionSetValue))]
        public int? MagicLinkTokenStatusId { get; set; }
        [EntityField("dfe_candidateadviserstatusreason", typeof(OptionSetValue))]
        public int? AdviserStatusId { get; set; }
        [EntityField("dfe_candidatereregisterstatus", typeof(OptionSetValue))]
        public int? RegistrationStatusId { get; set; }
        [EntityField("dfe_candidateapplystatus", typeof(OptionSetValue), null, new[] { "APPLY_API" })]
        public int? FindApplyStatusId { get; set; }
        [EntityField("dfe_candidateapplyphase", typeof(OptionSetValue), null, new[] { "APPLY_API" })]
        public int? FindApplyPhaseId { get; set; }
        [EntityField("dfe_waitingtobeassigneddate")]
        public DateTime? StatusIsWaitingToBeAssignedAt { get; set; }
        [EntityField("merged")]
        public bool Merged { get; set; }
        [EntityField("dfe_applyid", null, null, new[] { "APPLY_API" })]
        public string FindApplyId { get; set; }
        [EntityField("dfe_applylastmodifiedon", null, null, new[] { "APPLY_API" })]
        public DateTime? FindApplyUpdatedAt { get; set; }
        [EntityField("dfe_applycreatedon", null, null, new[] { "APPLY_API" })]
        public DateTime? FindApplyCreatedAt { get; set; }
        [EntityField("emailaddress1")]
        public string Email { get; set; }
        [EntityField("emailaddress2")]
        public string SecondaryEmail { get; set; }
        [EntityField("firstname")]
        public string FirstName { get; set; }
        [EntityField("lastname")]
        public string LastName { get; set; }
        [EntityField("birthdate")]
        public DateTime? DateOfBirth { get; set; }
        [EntityField("mobilephone")]
        public string MobileTelephone { get; set; }
        [EntityField("address1_telephone1")]
        public string AddressTelephone { get; set; }
        [EntityField("address1_line1")]
        public string AddressLine1 { get; set; }
        [EntityField("address1_line2")]
        public string AddressLine2 { get; set; }
        [EntityField("address1_line3")]
        public string AddressLine3 { get; set; }
        [EntityField("address1_city")]
        public string AddressCity { get; set; }
        [EntityField("address1_stateorprovince")]
        public string AddressStateOrProvince { get; set; }
        [EntityField("address1_postalcode")]
        public string AddressPostcode { get; set; }
        [EntityField("telephone1")]
        public string Telephone { get; set; }
        [EntityField("telephone2")]
        public string SecondaryTelephone { get; set; }
        [EntityField("dfe_hasdbscertificate")]
        public bool? HasDbsCertificate { get; set; }
        [EntityField("dfe_dateofissueofdbscertificate")]
        public DateTime? DbsCertificateIssuedAt { get; set; }
        [EntityField("dfe_notesforclassroomexperience")]
        public string ClassroomExperienceNotesRaw { get; set; }
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
        public bool? OptOutOfGdpr { get; set; }
        [EntityField("dfe_newregistrant")]
        public bool IsNewRegistrant { get; set; }
        [EntityField("dfe_websitemltoken")]
        public string MagicLinkToken { get; set; }
        [EntityField("dfe_websitemltokenexpirydate")]
        public DateTime? MagicLinkTokenExpiresAt { get; set; }
        [EntityField("dfe_welcomeguidestring")]
        public string WelcomeGuideVariant { get; set; }

        [EntityField("dfe_gitisttaserviceissubscriber")]
        public bool? HasTeacherTrainingAdviserSubscription { get; set; }
        [EntityField("dfe_gitisttaservicesubscriptionchannel", typeof(OptionSetValue))]
        public int? TeacherTrainingAdviserSubscriptionChannelId { get; set; }
        [EntityField("dfe_gitisttaservicestartdate")]
        public DateTime? TeacherTrainingAdviserSubscriptionStartAt { get; set; }
        [EntityField("dfe_gitisttaservicedonotbulkemail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_gitisttaservicedonotbulkpostalmail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_gitisttaservicedonotemail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_gitisttaservicedonotpostalmail")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_gitisttaservicedonotsendmm")]
        public bool? TeacherTrainingAdviserSubscriptionDoNotSendMm { get; set; }

        [EntityField("dfe_gitismailinglistserviceissubscriber")]
        public bool? HasMailingListSubscription { get; set; }
        [EntityField("dfe_gitismlservicesubscriptionchannel", typeof(OptionSetValue))]
        public int? MailingListSubscriptionChannelId { get; set; }
        [EntityField("dfe_gitismailinglistservicestartdate")]
        public DateTime? MailingListSubscriptionStartAt { get; set; }
        [EntityField("dfe_gitismailinglistservicedonotbulkemail")]
        public bool? MailingListSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_gitismlservicedonotbulkpostalmail")]
        public bool? MailingListSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_gitismailinglistservicedonotemail")]
        public bool? MailingListSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_gitismailinglistservicedonotpostalmail")]
        public bool? MailingListSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_gitismailinglistservicedonotsendmm")]
        public bool? MailingListSubscriptionDoNotSendMm { get; set; }

        [EntityField("dfe_gitiseventsserviceissubscriber")]
        public bool? HasEventsSubscription { get; set; }
        [EntityField("dfe_gitiseventsservicesubscriptionchannel", typeof(OptionSetValue))]
        public int? EventsSubscriptionChannelId { get; set; }
        [EntityField("dfe_gitiseventsservicesubscriptiontype", typeof(OptionSetValue))]
        public int? EventsSubscriptionTypeId { get; set; }
        [EntityField("dfe_gitiseventsservicestartdate")]
        public DateTime? EventsSubscriptionStartAt { get; set; }
        [EntityField("dfe_gitiseventsservicedonotbulkemail")]
        public bool? EventsSubscriptionDoNotBulkEmail { get; set; }
        [EntityField("dfe_gitiseventsservicedonotbulkpostalmail")]
        public bool? EventsSubscriptionDoNotBulkPostalMail { get; set; }
        [EntityField("dfe_gitiseventsservicedonotemail")]
        public bool? EventsSubscriptionDoNotEmail { get; set; }
        [EntityField("dfe_gitiseventsservicedonotpostalmail")]
        public bool? EventsSubscriptionDoNotPostalMail { get; set; }
        [EntityField("dfe_gitiseventsservicedonotsendmm")]
        public bool? EventsSubscriptionDoNotSendMm { get; set; }

        [EntityRelationship("msevtmgt_contact_msevtmgt_eventregistration_Contact", typeof(TeachingEventRegistration))]
        public List<TeachingEventRegistration> TeachingEventRegistrations { get; set; } = new List<TeachingEventRegistration>();
        [EntityRelationship("dfe_contact_dfe_candidatequalification_ContactId", typeof(CandidateQualification))]
        public List<CandidateQualification> Qualifications { get; set; } = new List<CandidateQualification>();
        [EntityRelationship("dfe_contact_dfe_candidatepastteachingposition_ContactId", typeof(CandidatePastTeachingPosition))]
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; } = new List<CandidatePastTeachingPosition>();
        [EntityRelationship("dfe_contact_dfe_applyapplicationform_contact", typeof(ApplicationForm))]
        public List<ApplicationForm> ApplicationForms { get; set; } = new List<ApplicationForm>();
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        [EntityRelationship("dfe_contact_phonecall_contactid", typeof(PhoneCall))]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        [EntityRelationship("dfe_contact_dfe_candidateprivacypolicy_Candidate", typeof(CandidatePrivacyPolicy))]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }
        [EntityRelationship("dfe_contact_dfe_candidateschoolexperience_ContactId", typeof(CandidateSchoolExperience))]
        public List<CandidateSchoolExperience> SchoolExperiences { get; set; } = new List<CandidateSchoolExperience>();

        public Candidate()
        {
        }

        public Candidate(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }

        public void AddClassroomExperienceNote(ClassroomExperienceNote note)
        {
            if (string.IsNullOrWhiteSpace(ClassroomExperienceNotesRaw))
            {
                ClassroomExperienceNotesRaw = ClassroomExperienceNote.Header;
            }

            ClassroomExperienceNotesRaw += note.ToString();
        }

        public bool HasTeacherTrainingAdviser()
        {
            return HasTeacherTrainingAdviserSubscription == true || OwningBusinessUnitId == AdviserBusinessUnitId;
        }

        public bool IsReturningToTeaching()
        {
            return TypeId == (int)Type.ReturningToTeacherTraining;
        }

        public bool IsInterestedInTeaching()
        {
            return TypeId == (int)Type.InterestedInTeacherTraining;
        }

        public bool HasGcseMathsAndEnglish()
        {
            return new[] { HasGcseEnglishId, HasGcseMathsId }.All(g => g == (int)GcseStatus.HasOrIsPlanningOnRetaking);
        }

        public bool IsPlanningToRetakeGcseMathsAndEnglish()
        {
            return new[] { PlanningToRetakeGcseMathsId, PlanningToRetakeGcseEnglishId }.All(g => g == (int)GcseStatus.HasOrIsPlanningOnRetaking);
        }

        public bool MagicLinkTokenExpired() => MagicLinkTokenExpiresAt == null || MagicLinkTokenExpiresAt < DateTime.UtcNow;
        public bool MagicLinkTokenAlreadyExchanged() => MagicLinkTokenStatusId == (int)MagicLinkTokenStatus.Exchanged;

        protected override bool ShouldMap(ICrmService crm)
        {
            IsNewRegistrant = Id == null;

            var changingEventSubscriptionType = !IsNewRegistrant && EventsSubscriptionTypeId != null;

            if (changingEventSubscriptionType && crm.CandidateAlreadyHasLocalEventSubscriptionType((Guid)Id))
            {
                // Never down-grade to a 'single event' subscription type from
                // a 'local event' subscription type.
                EventsSubscriptionTypeId = (int)SubscriptionType.LocalEvent;
            }

            return base.ShouldMap(crm);
        }

        protected override bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            if (value == null)
            {
                return false;
            }

            return propertyName switch
            {
                "PrivacyPolicy" when Id != null => crm.CandidateYetToAcceptPrivacyPolicy((Guid)Id, value.AcceptedPolicyId),
                _ => true,
            };
        }
    }
}
