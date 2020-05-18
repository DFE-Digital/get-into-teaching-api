using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
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
                a => a.Name == "dfe_preferredteachingsubject01" && a.Type == typeof(EntityReference) && a.Reference == "dfe_teachingsubjectlist");
            
            type.GetProperty("PreferredEducationPhaseId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_preferrededucationphase01" && a.Type == typeof(OptionSetValue));
            type.GetProperty("LocationId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_isinuk" && a.Type == typeof(OptionSetValue));
            type.GetProperty("InitialTeacherTrainingYearId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_ittyear" && a.Type == typeof(OptionSetValue));

            type.GetProperty("Email").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "emailaddress1");
            type.GetProperty("FirstName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "firstname");
            type.GetProperty("LastName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "lastname");
            type.GetProperty("DateOfBirth").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "birthdate");
            type.GetProperty("Telephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "telephone1");
            type.GetProperty("AddressLine1").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line1");
            type.GetProperty("AddressLine2").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line2");
            type.GetProperty("AddressLine3").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_line3");
            type.GetProperty("AddressCity").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_city");
            type.GetProperty("AddressState").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_stateorprovince");
            type.GetProperty("AddressPostcode").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "address1_postalcode");

            type.GetProperty("Qualifications").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidatequalification_ContactId" && a.Type == typeof(CandidateQualification));
            type.GetProperty("PastTeachingPositions").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidatepastteachingposition_ContactId" && a.Type == typeof(CandidatePastTeachingPosition));
            type.GetProperty("PhoneCall").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_phonecall_contactid" && a.Type == typeof(PhoneCall));
            type.GetProperty("PrivacyPolicy").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_contact_dfe_candidateprivacypolicy_Candidate" && a.Type == typeof(CandidatePrivacyPolicy));
        }
    }
}
