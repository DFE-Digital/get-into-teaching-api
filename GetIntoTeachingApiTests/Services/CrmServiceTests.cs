using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Xunit;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

namespace GetIntoTeachingApiTests.Services
{
    public class CrmServiceTests : IDisposable
    {
        private const string ConnectionString = "AuthType=ClientSecret; url=service_url; ClientId=client_id; ClientSecret=client_secret";
        private static readonly Guid JaneDoeGuid = new Guid("bf927e43-5650-44aa-859a-8297139b8ddd");
        private readonly string _previousCrmServiceUrl;
        private readonly string _previousCrmClientId;
        private readonly string _previousCrmClientSecret;
        private readonly Mock<IOrganizationServiceAdapter> _mockOrganizationalService;
        private readonly ICrmService _crm;

        public CrmServiceTests()
        {
            _previousCrmServiceUrl = Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
            _previousCrmClientId = Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
            _previousCrmClientSecret = Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");

            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", "service_url");
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", "client_id");
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", "client_secret");

            _mockOrganizationalService = new Mock<IOrganizationServiceAdapter>();
            _crm = new CrmService(_mockOrganizationalService.Object);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", _previousCrmServiceUrl);
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", _previousCrmClientId);
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", _previousCrmClientSecret);
        }

        [Fact]
        public void GetLookupItems_ReturnsAll()
        {
            var queryableCountries = MockCountries().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("dfe_country", It.IsAny<OrganizationServiceContext>()))
                .Returns(queryableCountries);

            var result = _crm.GetLookupItems("dfe_country");

            result.Select(country => country.Value).Should().BeEquivalentTo(
                new object[] { "Country 1", "Country 2", "Country 3" }
            );
        }

        [Fact]
        public void GetPickListItems_ReturnsAll()
        {
            var initialTeacherTrainingYears = MockInitialTeacherTrainingYears();
            _mockOrganizationalService.Setup(mock => mock.GetPickListItemsForAttribute(ConnectionString, "contact", "dfe_ittyear"))
                .Returns(initialTeacherTrainingYears);

            var result = _crm.GetPickListItems("contact", "dfe_ittyear");

            result.Select(year => year.Value).Should().BeEquivalentTo(
                new object[] { "2010", "2011", "2012" }
            );
        }

        [Fact]
        public void GetLatestPrivacyPolicy_ReturnsMostRecentlyCreatedActiveWebPrivacyPolicy()
        {
            var queryablePrivacyPolicies = MockPrivacyPolicies().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("dfe_privacypolicy", It.IsAny<OrganizationServiceContext>()))
                .Returns(queryablePrivacyPolicies);

            var result = _crm.GetLatestPrivacyPolicy();

            result.Text.Should().Be("Latest Active Web");
        }

        [Fact]
        public void GetPrivacyPolicies_Returns3MostRecentActiveWebPrivacyPolicies()
        {
            var queryablePrivacyPolicies = MockPrivacyPolicies().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("dfe_privacypolicy", It.IsAny<OrganizationServiceContext>()))
                .Returns(queryablePrivacyPolicies);

            var result = _crm.GetPrivacyPolicies();

            result.Count().Should().Be(3);
            result.Select(policy => policy.Text).Should().BeEquivalentTo(new string[] { "Latest Active Web", "Not Latest 1", "Not Latest 2" });
        }

        [Theory]
        [InlineData("john@doe.com", "New John", "Doe", "New John")]
        [InlineData("JOHN@doe.com", "New John", "Doe", "New John")]
        [InlineData("jane@doe.com", "Jane", "Doe", "Jane")]
        [InlineData("bob@doe.com", "Bob", "Doe", null)]
        public void GetCandidate_MatchesOnNewestCandidateWithEmail(
            string email,
            string firstName,
            string lastName,
            string expectedFirstName
        )
        {
            var request = new ExistingCandidateRequest { Email = email, FirstName = firstName, LastName = lastName };
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("contact", It.IsAny<OrganizationServiceContext>()))
                .Returns(MockCandidates().AsQueryable());
            _mockOrganizationalService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), It.IsAny<OrganizationServiceContext>()));
            _mockOrganizationalService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), It.IsAny<OrganizationServiceContext>()));

            var result = _crm.GetCandidate(request);

            result?.FirstName.Should().Be(expectedFirstName);
        }

        [Fact]
        public void GetCandidateQualifications_RetrievesQualifications()
        {
            var candidate = new Candidate() { Id = JaneDoeGuid };
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("dfe_candidatequalification", It.IsAny<OrganizationServiceContext>()))
                .Returns(MockCandidateQualifications().AsQueryable());

            var result = _crm.GetCandidateQualifications(candidate);

            result.Select(qualification => qualification.CategoryId).Should().BeEquivalentTo(new[] { 123, 456 });
        }

        [Fact]
        public void GetCandidate_RetrievesAssociatedPastTeachingPositions()
        {
            var candidate = new Candidate() { Id = JaneDoeGuid };
            _mockOrganizationalService.Setup(mock => mock.CreateQuery("dfe_candidatepastteachingposition", It.IsAny<OrganizationServiceContext>()))
                .Returns(MockCandidatePastTeachingPositions().AsQueryable());

            var result = _crm.GetCandidatePastTeachingPositions(candidate);

            result.Select(position => position.EducationPhaseId).Should().BeEquivalentTo(new[] { 111, 222 });
        }

        [Fact]
        public void UpsertCandidate_InsertsCandidate()
        {
            var candidate = new Candidate() { FirstName = "first" };
            var entity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(entity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            entity["firstname"].Should().Be(candidate.FirstName);
        }

        [Fact]
        public void UpsertCandidate_UpdatesCandidate()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid(), FirstName = "first" };
            var entity = new Entity() {EntityState = EntityState.Changed};
            _mockOrganizationalService.Setup(mock => mock.BlankExistingEntity("contact", 
                (Guid)candidate.Id, It.IsAny<OrganizationServiceContext>())).Returns(entity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            entity["firstname"].Should().Be(candidate.FirstName);
        }

        [Fact]
        public void UpsertCandidate_InsertsCandidateQualification()
        {
            var qualification = new CandidateQualification { CategoryId = 123 };
            var candidate = new Candidate() { Qualifications = new List<CandidateQualification> { qualification } };
            var qualificationEntity = new Entity() {EntityState = EntityState.Created};
            var candidateEntity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.NewEntity("dfe_candidatequalification", null)).Returns(qualificationEntity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.AddLink(candidateEntity,
                new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), qualificationEntity, It.IsAny<OrganizationServiceContext>()));
            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            qualificationEntity.GetAttributeValue<OptionSetValue>("dfe_category").Value.Should().Be(qualification.CategoryId);
        }

        [Fact]
        public void UpsertCandidate_UpdatesCandidateQualification()
        {
            var qualification = new CandidateQualification { Id = Guid.NewGuid(), CategoryId = 123 };
            var candidate = new Candidate() { Qualifications = new List<CandidateQualification> { qualification } };
            var entity = new Entity() { EntityState = EntityState.Changed };
            var candidateEntity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.BlankExistingEntity("dfe_candidatequalification",
                (Guid)qualification.Id, It.IsAny<OrganizationServiceContext>())).Returns(entity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            entity.GetAttributeValue<OptionSetValue>("dfe_category").Value.Should().Be(qualification.CategoryId);
        }

        [Fact]
        public void UpsertCandidate_InsertsCandidatePastTeachingPosition()
        {
            var position = new CandidatePastTeachingPosition { SubjectTaughtId = Guid.NewGuid() };
            var candidate = new Candidate() { PastTeachingPositions = new List<CandidatePastTeachingPosition> { position } };
            var candidateEntity = new Entity() { EntityState = EntityState.Created };
            var positionEntity = new Entity() {EntityState = EntityState.Created};
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.NewEntity("dfe_candidatepastteachingposition", null)).Returns(positionEntity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.AddLink(candidateEntity,
                new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), positionEntity, It.IsAny<OrganizationServiceContext>()));
            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            positionEntity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id.Should().Be((Guid)position.SubjectTaughtId);
        }

        [Fact]
        public void UpsertCandidate_UpdatesCandidatePastTeachingPosition()
        {
            var position = new CandidatePastTeachingPosition { Id = Guid.NewGuid(), SubjectTaughtId = Guid.NewGuid() };
            var candidate = new Candidate() { PastTeachingPositions = new List<CandidatePastTeachingPosition> { position } };
            var entity = new Entity() { EntityState = EntityState.Changed };
            var candidateEntity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.BlankExistingEntity("dfe_candidatepastteachingposition",
                (Guid)position.Id, It.IsAny<OrganizationServiceContext>())).Returns(entity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id.Should().Be((Guid)position.SubjectTaughtId);
        }

        [Fact]
        public void UpsertCandidate_InsertsCandidatePrivacyPolicy()
        {
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            var candidate = new Candidate() { PrivacyPolicy = policy };
            var candidateEntity = new Entity() {EntityState = EntityState.Created};
            var policyEntity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.NewEntity("dfe_candidateprivacypolicy", null)).Returns(policyEntity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.AddLink(candidateEntity,
                new Relationship("dfe_contact_dfe_candidateprivacypolicy_Candidate"), policyEntity, It.IsAny<OrganizationServiceContext>()));
            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            policyEntity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id.Should().Be((Guid)policy.AcceptedPolicyId);
        }

        [Fact]
        public void UpsertCandidate_InsertsPhoneCall()
        {
            var phoneCall = new PhoneCall() { ScheduledAt = DateTime.Now.AddDays(3) };
            var candidate = new Candidate() { PhoneCall = phoneCall };
            var candidateEntity = new Entity() { EntityState = EntityState.Created };
            var phoneCallEntity = new Entity() { EntityState = EntityState.Created };
            _mockOrganizationalService.Setup(mock => mock.NewEntity("contact", null)).Returns(candidateEntity);
            _mockOrganizationalService.Setup(mock => mock.NewEntity("phonecall", null)).Returns(phoneCallEntity);

            _crm.UpsertCandidate(candidate);

            _mockOrganizationalService.Verify(mock => mock.AddLink(candidateEntity,
                new Relationship("dfe_contact_phonecall_contactid"), phoneCallEntity, It.IsAny<OrganizationServiceContext>()));
            _mockOrganizationalService.Verify(mock => mock.SaveChanges(It.IsAny<OrganizationServiceContext>()));
            phoneCallEntity["scheduledstart"].Should().Be(phoneCall.ScheduledAt);
        }

        private IEnumerable<Entity> MockCandidates()
        {
            var candidate1 = new Entity("contact");
            candidate1.Id = JaneDoeGuid;
            candidate1["emailaddress1"] = "jane@doe.com";
            candidate1["firstname"] = "Jane";
            candidate1["lastname"] = "Doe";
            candidate1["createdon"] = DateTime.Now;

            var candidate2 = new Entity("contact");
            candidate2["emailaddress1"] = "john@doe.com";
            candidate2["firstname"] = "New John";
            candidate2["lastname"] = "Doe";
            candidate2["createdon"] = DateTime.Now;

            var candidate3 = new Entity("contact");
            candidate3["emailaddress1"] = "john@doe.com";
            candidate3["firstname"] = "Old John";
            candidate3["lastname"] = "Doe";
            candidate3["createdon"] = DateTime.Now.AddDays(-5);

            return new[] { candidate1, candidate2, candidate3 };
        }

        private IEnumerable<Entity> MockCandidatePastTeachingPositions()
        {
            var position1 = new Entity("dfe_candidatepastteachingposition");
            position1["dfe_contactid"] = JaneDoeGuid;
            position1["dfe_educationphase"] = new OptionSetValue { Value = 111 };

            var position2 = new Entity("dfe_candidatepastteachingposition");
            position2["dfe_contactid"] = JaneDoeGuid;
            position2["dfe_educationphase"] = new OptionSetValue { Value = 222 }; ;

            var position3 = new Entity("dfe_candidatepastteachingposition");
            position3["dfe_contactid"] = Guid.NewGuid();
            position3["dfe_educationphase"] = new OptionSetValue { Value = 333 }; ;

            return new[] { position1, position2, position3 };
        }

        private IEnumerable<Entity> MockCandidateQualifications()
        {
            var qualification1 = new Entity("dfe_candidatequalification");
            qualification1["dfe_contactid"] = JaneDoeGuid;
            qualification1["dfe_category"] = new OptionSetValue { Value = 123 };

            var qualification2 = new Entity("dfe_candidatequalification");
            qualification2["dfe_contactid"] = JaneDoeGuid;
            qualification2["dfe_category"] = new OptionSetValue { Value = 456 }; ;

            var qualification3 = new Entity("dfe_candidatequalification");
            qualification3["dfe_contactid"] = Guid.NewGuid();
            qualification3["dfe_category"] = new OptionSetValue { Value = 789 }; ;

            return new[] { qualification1, qualification2, qualification3 };
        }

        private IEnumerable<Entity> MockPrivacyPolicies()
        {
            var policy1 = new Entity("dfe_privacypolicy");
            policy1["dfe_details"] = "Latest Active Web";
            policy1["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy1["createdon"] = DateTime.UtcNow.AddDays(-10);
            policy1["dfe_active"] = true;

            var policy2 = new Entity("dfe_privacypolicy");
            policy2["dfe_details"] = "Not Web";
            policy2["dfe_policytype"] = new OptionSetValue { Value = 123 };
            policy2["createdon"] = DateTime.UtcNow.AddDays(-5);
            policy2["dfe_active"] = true;

            var policy3 = new Entity("dfe_privacypolicy");
            policy3["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy3["dfe_details"] = "Not Active";
            policy3["createdon"] = DateTime.UtcNow.AddDays(-3);
            policy3["dfe_active"] = false;

            var policy4 = new Entity("dfe_privacypolicy");
            policy4["dfe_details"] = "Not Latest 1";
            policy4["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy4["createdon"] = DateTime.UtcNow.AddDays(-15);
            policy4["dfe_active"] = true;

            var policy5 = new Entity("dfe_privacypolicy");
            policy5["dfe_details"] = "Not Latest 2";
            policy5["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy5["createdon"] = DateTime.UtcNow.AddDays(-20);
            policy5["dfe_active"] = true;

            var policy6 = new Entity("dfe_privacypolicy");
            policy6["dfe_details"] = "Not Latest 3";
            policy6["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy6["createdon"] = DateTime.UtcNow.AddDays(-25);
            policy6["dfe_active"] = true;

            return new[] { policy1, policy2, policy3, policy4, policy5, policy6 };
        }

        private IEnumerable<Entity> MockCountries()
        {
            var country1 = new Entity("dfe_country");
            country1["dfe_name"] = "Country 1";

            var country2 = new Entity("dfe_country");
            country2["dfe_name"] = "Country 2";

            var country3 = new Entity("dfe_country");
            country3["dfe_name"] = "Country 3";

            return new[] { country1, country2, country3 };
        }

        private List<PickListItem> MockInitialTeacherTrainingYears()
        {
            var year1 = new PickListItem { PickListItemId = 1, DisplayLabel = "2010" };
            var year2 = new PickListItem { PickListItemId = 2, DisplayLabel = "2011" };
            var year3 = new PickListItem { PickListItemId = 3, DisplayLabel = "2012" };

            return new List<PickListItem> { year1, year2, year3 };
        }
    }
}