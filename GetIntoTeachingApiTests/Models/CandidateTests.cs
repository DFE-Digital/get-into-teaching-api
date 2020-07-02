using System;
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
            type.GetProperty("DoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkemail");
            type.GetProperty("DoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkpostalmail");
            type.GetProperty("DoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotemail");
            type.GetProperty("DoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotpostalmail");
            type.GetProperty("CallbackInformation").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_websitecallbackdescription");
            type.GetProperty("EligibilityRulesPassed").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_eligibilityrulespassed");
            type.GetProperty("PreferredPhoneNumberType").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "_dfe_preferredphonenumbertype_label");
            type.GetProperty("PreferredContactMethod").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "_preferredcontactmethodcode_label");
            type.GetProperty("IsNewRegistrant").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_newregistrant");

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
        public void FullName_ReturnsCorrectly()
        {
            var candidate = new Candidate() { FirstName = "John", LastName = "Doe" };

            candidate.FullName.Should().Be("John Doe");
        }

        [Fact]
        public void DoNotBulkEmail_DefaultValue_IsCorrect()
        {
            new Candidate().DoNotBulkEmail.Should().BeFalse();
        }

        [Fact]
        public void DoNotEmail_DefaultValue_IsCorrect()
        {
            new Candidate().DoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void DoNotBulkPostalMail_DefaultValue_IsCorrect()
        {
            new Candidate().DoNotBulkPostalMail.Should().BeFalse();
        }

        [Fact]
        public void DoNotPostalMail_DefaultValue_IsCorrect()
        {
            new Candidate().DoNotPostalMail.Should().BeFalse();
        }

        [Fact]
        public void PreferredPhoneNumberType_DefaultValue_IsCorrect()
        {
            new Candidate().PreferredPhoneNumberType.Should().Be("Home");
        }

        [Fact]
        public void PreferredContactMethod_DefaultValue_IsCorrect()
        {
            new Candidate().PreferredContactMethod.Should().Be("Any");
        }
    }
}
