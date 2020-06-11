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

            type.GetProperty("PreferredEducationPhaseId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferrededucationphase01" && a.Type == typeof(OptionSetValue));
            type.GetProperty("LocationId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_isinuk" && a.Type == typeof(OptionSetValue));
            type.GetProperty("InitialTeacherTrainingYearId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_ittyear" && a.Type == typeof(OptionSetValue));
            type.GetProperty("ChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_channelcreation" && a.Type == typeof(OptionSetValue));

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

            mockService.Setup(m => m.BlankExistingEntity("candidate", (Guid)candidate.Id, context))
                .Returns(new Entity("contact"));
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
            var candidate = new Candidate()  { Id = Guid.NewGuid(), PrivacyPolicy = null };
            mockService.Setup(m => m.BlankExistingEntity("candidate", (Guid)candidate.Id, context))
                .Returns(new Entity("contact"));

            candidate.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.CandidateYetToAcceptPrivacyPolicy(
                It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            mockService.Verify(m => m.NewEntity("dfe_candidateprivacypolicy", context), Times.Never);
        }
    }
}
