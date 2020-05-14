using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

namespace GetIntoTeachingApiTests.Services
{
    public class CrmServiceTests : IDisposable
    {
        private const string ConnectionString = "AuthType=ClientSecret; url=service_url; ClientId=client_id; ClientSecret=client_secret";
        private static Guid JaneDoeGuid = new Guid("bf927e43-5650-44aa-859a-8297139b8ddd");
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
            IQueryable<Entity> queryableCountries = MockCountries().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "dfe_country"))
                .Returns(queryableCountries);

            var result = _crm.GetLookupItems("dfe_country");

            result.Select(country => country.Value).Should().BeEquivalentTo(
                new[] { "Country 1", "Country 2", "Country 3" }
            );
        }

        [Fact]
        public void GetPickListItems_ReturnsAll()
        {
            IEnumerable<PickListItem> initialTeacherTrainingYears = MockInitialTeacherTrainingYears();
            _mockOrganizationalService.Setup(mock => mock.GetPickListItemsForAttribute(ConnectionString, "contact", "dfe_ittyear"))
                .Returns(initialTeacherTrainingYears);

            var result = _crm.GetPickListItems("contact", "dfe_ittyear");

            result.Select(year => year.Value).Should().BeEquivalentTo(
                new[] { "2010", "2011", "2012" }
            );
        }

        [Fact]
        public void GetLatestPrivacyPolicy_ReturnsMostRecentlyCreatedActiveWebPrivacyPolicy()
        {
            IQueryable<Entity> queryablePrivacyPolicies = MockPrivacyPolicies().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "dfe_privacypolicy"))
                .Returns(queryablePrivacyPolicies);

            var result = _crm.GetLatestPrivacyPolicy();

            result.Text.Should().Be("Latest Active Web");
        }

        [Fact]
        public void GetPrivacyPolicies_Returns3MostRecentActiveWebPrivacyPolicies()
        {
            IQueryable<Entity> queryablePrivacyPolicies = MockPrivacyPolicies().AsQueryable();
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "dfe_privacypolicy"))
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
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "contact"))
                .Returns(MockCandidates().AsQueryable());

            var result = _crm.GetCandidate(request);

            result?.FirstName.Should().Be(expectedFirstName);
        }

        [Fact]
        public void GetCandidate_RetrievesAssociatedQualifications()
        {
            var request = new ExistingCandidateRequest { Email = "jane@doe.com", FirstName = "Jane", LastName = "Doe" };
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "contact"))
                .Returns(MockCandidates().AsQueryable());
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "dfe_candidatequalification"))
                .Returns(MockCandidateQualifications().AsQueryable());

            var result = _crm.GetCandidate(request);

            result.Qualifications.Select(qualification => qualification.CategoryId).Should().BeEquivalentTo(new[] { 123, 456 });
        }

        [Fact]
        public void GetCandidate_RetrievesAssociatedPastTeachingPositions()
        {
            var request = new ExistingCandidateRequest { Email = "jane@doe.com", FirstName = "Jane", LastName = "Doe" };
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "contact"))
                .Returns(MockCandidates().AsQueryable());
            _mockOrganizationalService.Setup(mock => mock.CreateQuery(ConnectionString, "dfe_candidatepastteachingposition"))
                .Returns(MockCandidatePastTeachingPositions().AsQueryable());

            var result = _crm.GetCandidate(request);

            result.PastTeachingPositions.Select(position => position.EducationPhaseId).Should().BeEquivalentTo(new[] { 111, 222 });
        }

        private IEnumerable<Entity> MockCandidates()
        {
            var candidate1 = new Entity("contact");
            candidate1.Id = JaneDoeGuid;
            candidate1.Attributes["emailaddress1"] = "jane@doe.com";
            candidate1.Attributes["firstname"] = "Jane";
            candidate1.Attributes["lastname"] = "Doe";
            candidate1.Attributes["createdon"] = DateTime.Now;

            var candidate2 = new Entity("contact");
            candidate2.Attributes["emailaddress1"] = "john@doe.com";
            candidate2.Attributes["firstname"] = "New John";
            candidate2.Attributes["lastname"] = "Doe";
            candidate2.Attributes["createdon"] = DateTime.Now;

            var candidate3 = new Entity("contact");
            candidate3.Attributes["emailaddress1"] = "john@doe.com";
            candidate3.Attributes["firstname"] = "Old John";
            candidate3.Attributes["lastname"] = "Doe";
            candidate3.Attributes["createdon"] = DateTime.Now.AddDays(-5);

            return new[] { candidate1, candidate2, candidate3 };
        }

        private IEnumerable<Entity> MockCandidatePastTeachingPositions()
        {
            var position1 = new Entity("dfe_candidatepastteachingposition");
            position1.Attributes["dfe_contactid"] = JaneDoeGuid;
            position1.Attributes["dfe_educationphase"] = new OptionSetValue { Value = 111 };

            var position2 = new Entity("dfe_candidatepastteachingposition");
            position2.Attributes["dfe_contactid"] = JaneDoeGuid;
            position2.Attributes["dfe_educationphase"] = new OptionSetValue { Value = 222 }; ;

            var position3 = new Entity("dfe_candidatepastteachingposition");
            position3.Attributes["dfe_contactid"] = Guid.NewGuid();
            position3.Attributes["dfe_educationphase"] = new OptionSetValue { Value = 333 }; ;

            return new[] { position1, position2, position3 };
        }
        
        private IEnumerable<Entity> MockCandidateQualifications()
        {
            var qualification1 = new Entity("dfe_candidatequalification");
            qualification1.Attributes["dfe_contactid"] = JaneDoeGuid;
            qualification1.Attributes["dfe_category"] = new OptionSetValue { Value = 123 };

            var qualification2 = new Entity("dfe_candidatequalification");
            qualification2.Attributes["dfe_contactid"] = JaneDoeGuid;
            qualification2.Attributes["dfe_category"] = new OptionSetValue { Value = 456 }; ;

            var qualification3 = new Entity("dfe_candidatequalification");
            qualification3.Attributes["dfe_contactid"] = Guid.NewGuid();
            qualification3.Attributes["dfe_category"] = new OptionSetValue { Value = 789 }; ;

            return new[] { qualification1, qualification2, qualification3 };
        }

        private IEnumerable<Entity> MockPrivacyPolicies()
        {
            var policy1 = new Entity("dfe_privacypolicy");
            policy1.Attributes["dfe_details"] = "Latest Active Web";
            policy1.Attributes["dfe_policytype"] = new OptionSetValue { Value = (int) CrmService.PrivacyPolicyType.Web };
            policy1.Attributes["createdon"] = DateTime.UtcNow.AddDays(-10);
            policy1.Attributes["dfe_active"] = true;

            var policy2 = new Entity("dfe_privacypolicy");
            policy2.Attributes["dfe_details"] = "Not Web";
            policy2.Attributes["dfe_policytype"] = new OptionSetValue { Value = 123 };
            policy2.Attributes["createdon"] = DateTime.UtcNow.AddDays(-5);
            policy2.Attributes["dfe_active"] = true;

            var policy3 = new Entity("dfe_privacypolicy");
            policy3.Attributes["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy3.Attributes["dfe_details"] = "Not Active";
            policy3.Attributes["createdon"] = DateTime.UtcNow.AddDays(-3);
            policy3.Attributes["dfe_active"] = false;

            var policy4 = new Entity("dfe_privacypolicy");
            policy4.Attributes["dfe_details"] = "Not Latest 1";
            policy4.Attributes["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy4.Attributes["createdon"] = DateTime.UtcNow.AddDays(-15);
            policy4.Attributes["dfe_active"] = true;

            var policy5 = new Entity("dfe_privacypolicy");
            policy5.Attributes["dfe_details"] = "Not Latest 2";
            policy5.Attributes["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy5.Attributes["createdon"] = DateTime.UtcNow.AddDays(-20);
            policy5.Attributes["dfe_active"] = true;

            var policy6 = new Entity("dfe_privacypolicy");
            policy6.Attributes["dfe_details"] = "Not Latest 3";
            policy6.Attributes["dfe_policytype"] = new OptionSetValue { Value = (int)CrmService.PrivacyPolicyType.Web };
            policy6.Attributes["createdon"] = DateTime.UtcNow.AddDays(-25);
            policy6.Attributes["dfe_active"] = true;

            return new[] { policy1, policy2, policy3, policy4, policy5, policy6 };
        }

        private IEnumerable<Entity> MockCountries()
        {
            var country1 = new Entity("dfe_country");
            country1.Attributes["dfe_name"] = "Country 1";

            var country2 = new Entity("dfe_country");
            country2.Attributes["dfe_name"] = "Country 2";

            var country3 = new Entity("dfe_country");
            country3.Attributes["dfe_name"] = "Country 3";

            return new[] { country1, country2, country3 };
        }

        private IEnumerable<PickListItem> MockInitialTeacherTrainingYears()
        {
            var year1 = new PickListItem { PickListItemId = 1, DisplayLabel = "2010" };
            var year2 = new PickListItem { PickListItemId = 2, DisplayLabel = "2011" };
            var year3 = new PickListItem { PickListItemId = 3, DisplayLabel = "2012" };

            return new[] { year1, year2, year3 };
        }
    }
}
