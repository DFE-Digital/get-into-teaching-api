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

            type.GetProperty("PreferredTeachingSubjectId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferredteachingsubject01" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_teachingsubjectlist");
            type.GetProperty("CountryId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_country" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_country");

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
            type.GetProperty("PlanningToRetakeCgseScienceId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websiteplanningretakesciencegcse" && a.Type == typeof(OptionSetValue));
            type.GetProperty("DescribeYourselfOptionId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitedescribeyourself" && a.Type == typeof(OptionSetValue));
            type.GetProperty("ConsiderationJourneyStageId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_websitewhereinconsiderationjourney" && a.Type == typeof(OptionSetValue));
            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_typeofcandidate" && a.Type == typeof(OptionSetValue));
            type.GetProperty("StatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_candidatestatus" && a.Type == typeof(OptionSetValue));
            type.GetProperty("AdviserEligibilityId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_iscandidateeligibleforadviser" && a.Type == typeof(OptionSetValue));
            type.GetProperty("AdviserRequirementId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_isadvisorrequiredos" && a.Type == typeof(OptionSetValue));
            type.GetProperty("GdprConsentId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msgdpr_gdprconsent" && a.Type == typeof(OptionSetValue));

            type.GetProperty("Email").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "emailaddress1");
            type.GetProperty("FirstName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "firstname");
            type.GetProperty("LastName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "lastname");
            type.GetProperty("DateOfBirth").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "birthdate");
            type.GetProperty("Telephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_telephone1");
            type.GetProperty("AddressLine1").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line1");
            type.GetProperty("AddressLine2").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line2");
            type.GetProperty("AddressLine3").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line3");
            type.GetProperty("AddressCity").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_city");
            type.GetProperty("AddressState").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_stateorprovince");
            type.GetProperty("AddressPostcode").Should()
                .BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_postalcode");
            type.GetProperty("StatusIsWaitingToBeAssignedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_waitingtobeassigneddate");
            type.GetProperty("DoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkemail");
            type.GetProperty("DoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkpostalmail");
            type.GetProperty("DoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotemail");
            type.GetProperty("DoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotpostalmail");
            type.GetProperty("DoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotsendmm");
            type.GetProperty("OptOutOfSms").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_optoutsms");
            type.GetProperty("CallbackInformation").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitecallbackdescription");
            type.GetProperty("EligibilityRulesPassed").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_eligibilityrulespassed");
            type.GetProperty("PreferredPhoneNumberTypeId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_preferredphonenumbertype");
            type.GetProperty("PreferredContactMethodId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "preferredcontactmethodcode");
            type.GetProperty("IsNewRegistrant").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_newregistrant");
            type.GetProperty("TeacherId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_dfesnumber");
            type.GetProperty("StatusIsWaitingToBeAssignedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_waitingtobeassigneddate");
            type.GetProperty("OptOutOfGdpr").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msdyn_gdproptout");

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

        [Fact]
        public void ToEntity_WhenRelationshipIsNull_DoesNotCreateEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Subscriptions = new List<Subscription>() { null }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));

            candidate.ToEntity(mockCrm.Object, context);

            mockService.Verify(m => m.NewEntity("dfe_servicesubscription", context), Times.Never);
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

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.MappableEntity("dfe_candidatequalification", null, context), Times.Once);
        }

        [Fact]
        public void ToEntity_WhenAlreadySubscribedToService_DoesNotCreateSubscriptionEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                Subscriptions = new List<Subscription>() { new Subscription() { TypeId = (int)Subscription.ServiceType.Event } }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", (Guid)candidate.Id, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.CandidateYetToSubscribeToServiceOfType((Guid)candidate.Id, (int)Subscription.ServiceType.Event)).Returns(false);

            candidate.ToEntity(mockCrm.Object, context);

            mockService.Verify(m => m.NewEntity("dfe_servicesubscription", context), Times.Never);
        }

        [Fact]
        public void ToEntity_WithNewCandidateAndSubscription_CreatesSubscriptionEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                Subscriptions = new List<Subscription>() { new Subscription() { TypeId = (int)Subscription.ServiceType.Event } }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.MappableEntity("dfe_servicesubscription", null, context)).Returns(new Entity("dfe_servicesubscription"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.MappableEntity("dfe_servicesubscription", null, context), Times.Once);
        }

        [Fact]
        public void ToEntity_WithExistingCandidateAndSubscription_CreatesSubscriptionEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var subscriptionId = Guid.NewGuid();

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                Subscriptions = new List<Subscription>() { new Subscription() { Id = subscriptionId, TypeId = (int)Subscription.ServiceType.Event } }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", (Guid)candidate.Id, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.MappableEntity("dfe_servicesubscription", subscriptionId, context)).Returns(new Entity("dfe_servicesubscription"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.MappableEntity("dfe_servicesubscription", subscriptionId, context), Times.Once);
        }

        [Fact]
        public void ToEntity_WhenAlreadyRegisteredForEvent_DoesNotCreateTeachingEventEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var teachingEvent = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                TeachingEventRegistrations = new List<TeachingEventRegistration>() { teachingEvent }
            };

            mockCrm.Setup(m => m.MappableEntity("contact", (Guid)candidate.Id, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent((Guid)candidate.Id, (Guid)teachingEvent.EventId)).Returns(false);

            candidate.ToEntity(mockCrm.Object, context);

            mockService.Verify(m => m.NewEntity("msevtmgt_eventregistration", context), Times.Never);
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
        public void ToEntity_WithPhoneCall_PopulatesFromCandidate()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate()
            {
                FirstName = "John",
                LastName = "Doe",
                Telephone = "123456789",
                PhoneCall = new PhoneCall() { ScheduledAt = DateTime.Now }
            };

            var phoneCallEntity = new Entity("phonecall");

            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(new Entity("contact"));
            mockCrm.Setup(m => m.MappableEntity("phonecall", null, context)).Returns(phoneCallEntity);

            candidate.ToEntity(mockCrm.Object, context);

            phoneCallEntity.GetAttributeValue<string>("phonenumber").Should().Be(candidate.Telephone);
            phoneCallEntity.GetAttributeValue<string>("subject").Should().Be("Scheduled phone call requested by John Doe");
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
        public void ToEntity_WithStatusIdOfWaitingToBeAssigned_SetsStatusIsWaitingToBeAssignedAtToNow()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { StatusId = (int)Candidate.AssignmentStatus.WaitingToBeAssigned };
            var candidateEntity = new Entity("contact");
            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(candidateEntity);

            candidate.ToEntity(mockCrm.Object, context);

            candidateEntity.GetAttributeValue<DateTime>("dfe_waitingtobeassigneddate")
                .Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(20));
        }

        [Fact]
        public void ToEntity_WithNullStatusId_DoesNotSetStatusIsWaitingToBeAssigned()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var candidate = new Candidate() { StatusId = null };
            var candidateEntity = new Entity("contact");
            mockCrm.Setup(m => m.MappableEntity("contact", null, context)).Returns(candidateEntity);

            candidate.ToEntity(mockCrm.Object, context);

            candidateEntity.GetAttributeValue<DateTime?>("dfe_waitingtobeassigneddate").Should().BeNull();
        }

        [Fact]
        public void FullName_ReturnsCorrectly()
        {
            var candidate = new Candidate() { FirstName = "John", LastName = "Doe" };

            candidate.FullName.Should().Be("John Doe");
        }

        [Fact]
        public void PreferredPhoneNumberType_DefaultValue_IsCorrect()
        {
            new Candidate().PreferredPhoneNumberTypeId.Should().Be((int)Candidate.PhoneNumberType.Home);
        }

        [Fact]
        public void PreferredContactMethod_DefaultValue_IsCorrect()
        {
            new Candidate().PreferredContactMethodId.Should().Be((int)Candidate.ContactMethod.Any);
        }

        [Fact]
        public void GdprConsentId_DefaultValue_IsCorrect()
        {
            new Candidate().GdprConsentId.Should().Be((int)Candidate.GdprConsent.Consent);
        }

        [Fact]
        public void OptOutOfGdpr_DefaultValue_IsCorrect()
        {
            new Candidate().OptOutOfGdpr.Should().BeFalse();
        }

        [Fact]
        public void HasActiveSubscriptionToService_ReturnsCorrectly()
        {
            var activeSubscription = new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.Event,
                StatusId = (int)Subscription.SubscriptionStatus.Active
            };
            var inactiveSubscription = new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.MailingList,
                StatusId = (int)Subscription.SubscriptionStatus.Inactive
            };
            var candidate = new Candidate();
            candidate.Subscriptions.AddRange(new List<Subscription> { activeSubscription, inactiveSubscription });

            candidate.HasActiveSubscriptionToService(Subscription.ServiceType.Event).Should().BeTrue();
            candidate.HasActiveSubscriptionToService(Subscription.ServiceType.MailingList).Should().BeFalse();
            candidate.HasActiveSubscriptionToService(Subscription.ServiceType.TeacherTrainingAdviser).Should().BeFalse();
        }
    }
}
