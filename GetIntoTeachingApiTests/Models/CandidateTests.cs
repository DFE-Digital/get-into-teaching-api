using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(Candidate);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "contact");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("PreferredTeachingSubjectId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferredteachingsubject01" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_teachingsubjectlist");
            type.GetProperty("SecondaryPreferredTeachingSubjectId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferredteachingsubject02" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_teachingsubjectlist");
            type.GetProperty("CountryId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_country" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_country");
            type.GetProperty("OwningBusinessUnitId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "owningbusinessunit" && a.Type == typeof(EntityReference) &&
                     a.Reference == "businessunit");
            type.GetProperty("MasterId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "masterid" && a.Type == typeof(EntityReference) &&
                     a.Reference == "contact");

            type.GetProperty("PreferredEducationPhaseId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferrededucationphase01" && a.Type == typeof(OptionSetValue));
            type.GetProperty("InitialTeacherTrainingYearId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_ittyear" && a.Type == typeof(OptionSetValue));
            type.GetProperty("ChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_channelcreation" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasGcseEnglishId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitehasgcseenglish" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasGcseMathsId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitehasgcsemaths" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasGcseScienceId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitehasgcsescience" && a.Type == typeof(OptionSetValue));
            type.GetProperty("PlanningToRetakeGcseEnglishId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websiteplanningretakeenglishgcse" && a.Type == typeof(OptionSetValue));
            type.GetProperty("PlanningToRetakeGcseMathsId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websiteplanningretakemathsgcse" && a.Type == typeof(OptionSetValue));
            type.GetProperty("PlanningToRetakeGcseScienceId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websiteplanningretakesciencegcse" && a.Type == typeof(OptionSetValue));
            type.GetProperty("ConsiderationJourneyStageId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitewhereinconsiderationjourney" && a.Type == typeof(OptionSetValue));
            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_typeofcandidate" && a.Type == typeof(OptionSetValue));
            type.GetProperty("AssignmentStatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_candidatestatus" && a.Type == typeof(OptionSetValue));
            type.GetProperty("AdviserEligibilityId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_iscandidateeligibleforadviser" && a.Type == typeof(OptionSetValue));
            type.GetProperty("AdviserRequirementId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_isadvisorrequiredos" && a.Type == typeof(OptionSetValue));
            type.GetProperty("GdprConsentId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msgdpr_gdprconsent" && a.Type == typeof(OptionSetValue));
            type.GetProperty("MagicLinkTokenStatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitemltokenstatus" && a.Type == typeof(OptionSetValue));


            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applyid");
            type.GetProperty("Merged").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "merged");
            type.GetProperty("Email").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "emailaddress1");
            type.GetProperty("SecondaryEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "emailaddress2");
            type.GetProperty("FirstName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "firstname");
            type.GetProperty("LastName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "lastname");
            type.GetProperty("DateOfBirth").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "birthdate");
            type.GetProperty("AddressTelephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_telephone1");
            type.GetProperty("AddressLine1").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line1");
            type.GetProperty("AddressLine2").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line2");
            type.GetProperty("AddressLine3").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line3");
            type.GetProperty("AddressCity").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_city");
            type.GetProperty("AddressStateOrProvince").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_stateorprovince");
            type.GetProperty("AddressPostcode").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_postalcode");
            type.GetProperty("Telephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "telephone1");
            type.GetProperty("SecondaryTelephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "telephone2");
            type.GetProperty("MobileTelephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "mobilephone");
            type.GetProperty("HasDbsCertificate").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_hasdbscertificate");
            type.GetProperty("DbsCertificateIssuedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_dateofissueofdbscertificate");
            type.GetProperty("ClassroomExperienceNotesRaw").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_notesforclassroomexperience");
            type.GetProperty("StatusIsWaitingToBeAssignedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_waitingtobeassigneddate");
            type.GetProperty("DoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkemail");
            type.GetProperty("DoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkpostalmail");
            type.GetProperty("DoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotemail");
            type.GetProperty("DoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotpostalmail");
            type.GetProperty("DoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotsendmm");
            type.GetProperty("OptOutOfSms").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_optoutsms");
            type.GetProperty("EligibilityRulesPassed").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_eligibilityrulespassed");
            type.GetProperty("PreferredPhoneNumberTypeId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_preferredphonenumbertype");
            type.GetProperty("PreferredContactMethodId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "preferredcontactmethodcode");
            type.GetProperty("IsNewRegistrant").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_newregistrant");
            type.GetProperty("TeacherId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_dfesnumber");
            type.GetProperty("StatusIsWaitingToBeAssignedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_waitingtobeassigneddate");
            type.GetProperty("OptOutOfGdpr").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msdyn_gdproptout");
            type.GetProperty("MagicLinkToken").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitemltoken");
            type.GetProperty("MagicLinkTokenExpiresAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitemltokenexpirydate");

            type.GetProperty("TeacherTrainingAdviserSubscriptionChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_gitisttaservicesubscriptionchannel" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasTeacherTrainingAdviserSubscription").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaserviceissubscriber");
            type.GetProperty("TeacherTrainingAdviserSubscriptionStartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicestartdate");
            type.GetProperty("TeacherTrainingAdviserSubscriptionDoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicedonotbulkemail");
            type.GetProperty("TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicedonotbulkpostalmail");
            type.GetProperty("TeacherTrainingAdviserSubscriptionDoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicedonotemail");
            type.GetProperty("TeacherTrainingAdviserSubscriptionDoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicedonotpostalmail");
            type.GetProperty("TeacherTrainingAdviserSubscriptionDoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitisttaservicedonotsendmm");

            type.GetProperty("MailingListSubscriptionChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_gitismlservicesubscriptionchannel" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasMailingListSubscription").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistserviceissubscriber");
            type.GetProperty("MailingListSubscriptionStartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistservicestartdate");
            type.GetProperty("MailingListSubscriptionDoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistservicedonotbulkemail");
            type.GetProperty("MailingListSubscriptionDoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismlservicedonotbulkpostalmail");
            type.GetProperty("MailingListSubscriptionDoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistservicedonotemail");
            type.GetProperty("MailingListSubscriptionDoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistservicedonotpostalmail");
            type.GetProperty("MailingListSubscriptionDoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitismailinglistservicedonotsendmm");

            type.GetProperty("EventsSubscriptionChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_gitiseventsservicesubscriptionchannel" && a.Type == typeof(OptionSetValue));
            type.GetProperty("EventsSubscriptionTypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_gitiseventsservicesubscriptiontype" && a.Type == typeof(OptionSetValue));
            type.GetProperty("HasEventsSubscription").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsserviceissubscriber");
            type.GetProperty("EventsSubscriptionStartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicestartdate");
            type.GetProperty("EventsSubscriptionDoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicedonotbulkemail");
            type.GetProperty("EventsSubscriptionDoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicedonotbulkpostalmail");
            type.GetProperty("EventsSubscriptionDoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicedonotemail");
            type.GetProperty("EventsSubscriptionDoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicedonotpostalmail");
            type.GetProperty("EventsSubscriptionDoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_gitiseventsservicedonotsendmm");

            type.GetProperty("Qualifications").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidatequalification_ContactId" &&
                     a.Type == typeof(CandidateQualification));
            type.GetProperty("PastTeachingPositions").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidatepastteachingposition_ContactId" &&
                     a.Type == typeof(CandidatePastTeachingPosition));
            type.GetProperty("PhoneCall").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_phonecall_contactid" && a.Type == typeof(PhoneCall));
            type.GetProperty("PrivacyPolicy").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidateprivacypolicy_Candidate" &&
                     a.Type == typeof(CandidatePrivacyPolicy));
        }

        [Theory]
        [InlineData(true, null, true)]
        [InlineData(null, "1A61F629-F502-E911-A972-000D3A23443B", true)]
        [InlineData(true, "1A61F629-F502-E911-A972-000D3A23443B", true)]
        [InlineData(false, "1A61F629-F502-E911-A972-000D3A23443B", true)]
        [InlineData(true, "2A61F629-F502-E911-A972-000D3A23443C", true)]
        [InlineData(false, null, false)]
        [InlineData(false, "2A61F629-F502-E911-A972-000D3A23443C", false)]
        [InlineData(null, "2A61F629-F502-E911-A972-000D3A23443C", false)]
        [InlineData(null, null, false)]
        public void HasTeacherTrainingAdviser_WithSubscription_ReturnsCorrectly(bool? hasSubscription, string owningBusinessUnitId, bool expectedOutcome)
        {
            Guid? id = null;

            if (owningBusinessUnitId != null)
            {
                id = new Guid(owningBusinessUnitId);
            }

            var candidate = new Candidate() {
                HasTeacherTrainingAdviserSubscription = hasSubscription,
                OwningBusinessUnitId = id,
            };

            candidate.HasTeacherTrainingAdviser().Should().Be(expectedOutcome);
        }

        [Fact]
        public void ToEntity_WhenRelationshipIsNull_DoesNotCreateEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Qualifications = new List<CandidateQualification>() { null }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));

            candidate.ToEntity(mockCrm.Object, context);

            mockService.Verify(m => m.NewEntity("dfe_candidatequalification", context), Times.Never);
        }

        [Fact]
        public void ToEntity_WithPropertyUsingDefaultMappingLogic_CreatesEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Qualifications = new List<CandidateQualification>() { new CandidateQualification() }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.MappableEntity("dfe_candidatequalification", null, context)).Returns(new Entity("dfe_candidatequalification"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.MappableEntity("dfe_candidatequalification", null, context), Times.Once);
        }

        [Fact]
        public void ToEntity_WhenPrivacyPolicyAlreadyAccepted_DoesNotCreatePrivacyPolicyEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", (Guid)candidate.Id, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.CandidateYetToAcceptPrivacyPolicy((Guid)candidate.Id,
                candidate.PrivacyPolicy.AcceptedPolicyId)).Returns(false);

            candidate.ToEntity(mockCrm.Object, context);

            mockService.Verify(m => m.NewEntity("dfe_candidateprivacypolicy", context), Times.Never);
        }

        [Fact]
        public void ToEntity_WithNewCandidateAndPrivacyPolicy_CreatesPrivacyPolicyEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.MappableEntity("dfe_candidateprivacypolicy", null, context)).Returns(new Entity("dfe_candidateprivacypolicy"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.MappableEntity("dfe_candidateprivacypolicy", null, context), Times.Once);
        }

        [Fact]
        public void ToEntity_WhenPrivacyPolicyIsNull_DoesNotCreatePrivacyPolicyEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { Id = Guid.NewGuid(), PrivacyPolicy = null };

            mockCrm.Setup(m => m.MappableEntity("contact", (Guid)candidate.Id, context)).Returns(new Entity("contact"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.CandidateYetToAcceptPrivacyPolicy(
                It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            mockService.Verify(m => m.NewEntity("dfe_candidateprivacypolicy", context), Times.Never);
        }

        [Fact]
        public void ToEntity_WhenChangingEventSubscriptionFromSingleToLocal_RetainsLocalSubscription()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { Id = Guid.NewGuid(), EventsSubscriptionTypeId = (int)Candidate.SubscriptionType.SingleEvent };
            var candidateEntity = new Entity("contact");
            mockCrm.Setup(m => m.MappableEntity("contact", candidate.Id, context)).Returns(candidateEntity);
            mockCrm.Setup(m => m.CandidateAlreadyHasLocalEventSubscriptionType((Guid)candidate.Id)).Returns(true);

            candidate.ToEntity(mockCrm.Object, context);

            candidateEntity.GetAttributeValue<bool>("dfe_newregistrant").Should().BeFalse();

            candidateEntity.GetAttributeValue<OptionSetValue>("dfe_gitiseventsservicesubscriptiontype")
                .Value.Should().Be((int)Candidate.SubscriptionType.LocalEvent);
        }

        [Fact]
        public void ToEntity_WithNullId_SetsIsNewRegistrantToTrue()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { Id = null };
            var candidateEntity = new Entity("contact");
            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(candidateEntity);

            candidate.ToEntity(mockCrm.Object, context);

            candidateEntity.GetAttributeValue<bool>("dfe_newregistrant").Should().BeTrue();
        }

        [Fact]
        public void ToEntity_WithId_SetsIsNewRegistrantToFalse()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            var candidateEntity = new Entity("contact");
            mockCrm.Setup(m => m.MappableEntity("contact", candidate.Id, context)).Returns(candidateEntity);

            candidate.ToEntity(mockCrm.Object, context);

            candidateEntity.GetAttributeValue<bool>("dfe_newregistrant").Should().BeFalse();
        }

        [Fact]
        public void FullName_ReturnsCorrectly()
        {
            var candidate = new Candidate() { FirstName = "John", LastName = "Doe" };

            candidate.FullName.Should().Be("John Doe");
        }

        [Theory]
        [InlineData((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, true)]
        [InlineData(-1, (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, false)]
        [InlineData((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, -1, false)]
        [InlineData(-1, -1, false)]
        public void HasGcseMathsAndEnglishId_ReturnsCorrectly(int hasGcseEnglishId, int hasGcseMathsId, bool expected)
        {
            var candidate = new Candidate() { HasGcseEnglishId = hasGcseEnglishId, HasGcseMathsId = hasGcseMathsId };

            candidate.HasGcseMathsAndEnglish().Should().Be(expected);
        }

        [Theory]
        [InlineData((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, true)]
        [InlineData(-1, (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, false)]
        [InlineData((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking, -1, false)]
        [InlineData(-1, -1, false)]
        public void IsPlanningToRetakeGcseMathsAndEnglishId_ReturnsCorrectly(int planningToRetakeGcseEnglishId, int planningToRetakeGcseMathsId, bool expected)
        {
            var candidate = new Candidate() { PlanningToRetakeGcseEnglishId = planningToRetakeGcseEnglishId, PlanningToRetakeGcseMathsId = planningToRetakeGcseMathsId };

            candidate.IsPlanningToRetakeGcseMathsAndEnglish().Should().Be(expected);
        }

        [Fact]
        public void IsReturningToTeaching_WhenTypeIsReturningToTeaching_ReturnsTrue()
        {
            var candidate = new Candidate() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            candidate.IsReturningToTeaching().Should().BeTrue();
        }

        [Fact]
        public void IsReturningToTeaching_WhenTypeIdIsInterestedInTeacherTraining_ReturnsFalse()
        {
            var candidate = new Candidate() { TypeId = (int)Candidate.Type.InterestedInTeacherTraining };

            candidate.IsReturningToTeaching().Should().BeFalse();
        }

        [Fact]
        public void IsInterestedInTeaching_WhenTypeIsInterestedInTeaching_ReturnsTrue()
        {
            var candidate = new Candidate() { TypeId = (int)Candidate.Type.InterestedInTeacherTraining };

            candidate.IsInterestedInTeaching().Should().BeTrue();
        }

        [Fact]
        public void IsInterestedInTeaching_WhenTypeIdIsIReturningToTeacherTraining_ReturnsFalse()
        {
            var candidate = new Candidate() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            candidate.IsInterestedInTeaching().Should().BeFalse();
        }

        [Fact]
        public void MagicLinkTokenExpired_WhenNull_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                MagicLinkTokenExpiresAt = null
            };

            candidate.MagicLinkTokenExpired().Should().BeTrue();
        }

        [Fact]
        public void MagicLinkTokenExpired_WhenExpiredInPast_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                MagicLinkTokenExpiresAt = DateTime.UtcNow.AddSeconds(-5)
            };

            candidate.MagicLinkTokenExpired().Should().BeTrue();
        }

        [Fact]
        public void MagicLinkTokenExpired_WhenExpiredInFuture_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                MagicLinkTokenExpiresAt = DateTime.UtcNow.AddSeconds(5)
            };

            candidate.MagicLinkTokenExpired().Should().BeFalse();
        }

        [Theory]
        [InlineData(Candidate.MagicLinkTokenStatus.Exchanged, true)]
        [InlineData(Candidate.MagicLinkTokenStatus.Generated, false)]
        [InlineData(null, false)]
        public void MagicLinkTokenAlreadyExchanged_ReturnsCorrectly(Candidate.MagicLinkTokenStatus? status, bool expected)
        {
            var candidate = new Candidate
            {
                MagicLinkTokenStatusId = (int?)status,
            };

            candidate.MagicLinkTokenAlreadyExchanged().Should().Be(expected);
        }

        [Fact]
        public void AddClassroomExperienceNote_WhenRawIsNull_PrependsHeader()
        {
            var candidate = new Candidate() { ClassroomExperienceNotesRaw = null };
            var note = new ClassroomExperienceNote()
            {
                Action = "REQUESTED",
                Date = new DateTime(2020, 2, 1),
                RecordedAt = new DateTime(2020, 3, 2),
                SchoolName = "School Name",
                SchoolUrn = 123456,
            };

            candidate.AddClassroomExperienceNote(note);

            candidate.ClassroomExperienceNotesRaw.Should().Be(
                "RECORDED   ACTION                 EXP DATE   URN    NAME\r\n\r\n" +
                "02/03/2020 REQUESTED              01/02/2020 123456 School Name\r\n");
        }

        [Fact]
        public void AddClassroomExperienceNote_WithExistingNotes_AppendsNote()
        {
            var candidate = new Candidate() { ClassroomExperienceNotesRaw = null };
            var firstNote = new ClassroomExperienceNote()
            {
                Action = "REQUESTED",
                Date = new DateTime(2020, 2, 1),
                RecordedAt = new DateTime(2020, 3, 2),
                SchoolName = "School Name",
                SchoolUrn = 123456,
            };

            candidate.AddClassroomExperienceNote(firstNote);


            var secondNote = new ClassroomExperienceNote()
            {
                Action = "CANCELLED BY SCHOOL",
                Date = new DateTime(2021, 10, 11),
                RecordedAt = new DateTime(2021, 12, 4),
                SchoolName = "School Name",
                SchoolUrn = 654321,
            };

            candidate.AddClassroomExperienceNote(secondNote);

            candidate.ClassroomExperienceNotesRaw.Should().Be(
                "RECORDED   ACTION                 EXP DATE   URN    NAME\r\n\r\n" +
                "02/03/2020 REQUESTED              01/02/2020 123456 School Name\r\n" +
                "04/12/2021 CANCELLED BY SCHOOL    11/10/2021 654321 School Name\r\n");
        }
    }
}
