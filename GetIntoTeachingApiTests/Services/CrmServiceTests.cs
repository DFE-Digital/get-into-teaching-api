using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Xunit;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

namespace GetIntoTeachingApiTests.Services
{
    public class CrmServiceTests : IDisposable
    {
        private const string ConnectionString = "AuthType=ClientSecret; url=service_url; ClientId=client_id; ClientSecret=client_secret";
        private static readonly Guid JaneDoeGuid = new Guid("bf927e43-5650-44aa-859a-8297139b8ddd");
        private static readonly Guid FindEventGuid = new Guid("ff927e43-5650-44aa-859a-8297139b8eee");
        private readonly string _previousCrmServiceUrl;
        private readonly string _previousCrmClientId;
        private readonly string _previousCrmClientSecret;
        private readonly Mock<IOrganizationServiceAdapter> _mockService;
        private readonly OrganizationServiceContext _context;
        private readonly ICrmService _crm;

        public CrmServiceTests()
        {
            _previousCrmServiceUrl = Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
            _previousCrmClientId = Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
            _previousCrmClientSecret = Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");

            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", "service_url");
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", "client_id");
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", "client_secret");

            var mockPostcodeService = new Mock<IPostcodeService>();
            _mockService = new Mock<IOrganizationServiceAdapter>();
            _context = new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
            _mockService.Setup(mock => mock.Context(ConnectionString)).Returns(_context);
            var cache = new CrmCache(new Mock<ILogger<CrmCache>>().Object);
            _crm = new CrmService(_mockService.Object, cache, mockPostcodeService.Object);
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
            var queryableCountries = MockCountries();
            _mockService.Setup(mock => mock.CreateQuery("dfe_country", _context))
                .Returns(queryableCountries);

            var result = _crm.GetLookupItems("dfe_country");

            result.Select(country => country.Value).Should().BeEquivalentTo(
                new object[] { "Country 1", "Country 2", "Country 3" }, 
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetLookupItems_IsCached()
        {
            var queryableCountries = MockCountries();
            _mockService.Setup(mock => mock.CreateQuery("dfe_country", _context))
                .Returns(queryableCountries);

            var result1 = _crm.GetLookupItems("dfe_country");
            var result2 = _crm.GetLookupItems("dfe_country");

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.CreateQuery("dfe_country", _context), Times.Once);
        }

        [Fact]
        public void GetPickListItems_ReturnsAll()
        {
            var initialTeacherTrainingYears = MockInitialTeacherTrainingYears();
            _mockService.Setup(mock => mock.GetPickListItemsForAttribute(ConnectionString, "contact", "dfe_ittyear"))
                .Returns(initialTeacherTrainingYears);

            var result = _crm.GetPickListItems("contact", "dfe_ittyear");

            result.Select(year => year.Value).Should().BeEquivalentTo(new object[] { "2010", "2011", "2012" }, 
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetPickListItems_IsCached()
        {
            var initialTeacherTrainingYears = MockInitialTeacherTrainingYears();
            _mockService.Setup(mock => mock.GetPickListItemsForAttribute(ConnectionString, "contact", "dfe_ittyear"))
                .Returns(initialTeacherTrainingYears);

            var result1 = _crm.GetPickListItems("contact", "dfe_ittyear");
            var result2 = _crm.GetPickListItems("contact", "dfe_ittyear");

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.GetPickListItemsForAttribute(ConnectionString, "contact", "dfe_ittyear"), Times.Once);
        }

        [Fact]
        public void SearchTeachingEvents_ReturnsMatchingEventsInOrder()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "CA4 8LE", TypeId = 123 };
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result = _crm.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_IsCached()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "KY12 8FE" };
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result1 = _crm.SearchTeachingEvents(request);
            var result2 = _crm.SearchTeachingEvents(request);

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.RetrieveMultiple(ConnectionString,
                It.Is<QueryExpression>(q => q.EntityName == "msevtmgt_event")), Times.Once);
        }

        [Fact]
        public void GetTeachingEvents_ReturnsMatchingEvent()
        {
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result = _crm.GetTeachingEvent(FindEventGuid);

            result.Id.Should().Be(FindEventGuid);
        }

        [Fact]
        public void GetTeachingEvent_IsCached()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "KY12 8FE" };
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result1 = _crm.SearchTeachingEvents(request);
            var result2 = _crm.SearchTeachingEvents(request);

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.RetrieveMultiple(ConnectionString,
                It.Is<QueryExpression>(q => q.EntityName == "msevtmgt_event")), Times.Once);
        }

        [Fact]
        public void GetUpcomingTeachingEvents_ReturnsUpcomingEventsInOrder()
        {
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result = _crm.GetUpcomingTeachingEvents(3);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] {"Event 2", "Event 4", "Event 1"}, 
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetUpcomingTeachingEvents_IsCached()
        {
            _mockService.Setup(mock => mock.RetrieveMultiple(ConnectionString, It.Is<QueryExpression>(
                q => q.EntityName == "msevtmgt_event"))).Returns(MockTeachingEvents());

            var result1 = _crm.GetUpcomingTeachingEvents(3);
            var result2 = _crm.GetUpcomingTeachingEvents(3);

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.RetrieveMultiple(ConnectionString, 
                It.Is<QueryExpression>(q => q.EntityName == "msevtmgt_event")), Times.Once);
        }

        [Fact]
        public void GetPrivacyPolicies_Returns3MostRecentActiveWebPrivacyPolicies()
        {
            var queryablePrivacyPolicies = MockPrivacyPolicies();
            _mockService.Setup(mock => mock.CreateQuery("dfe_privacypolicy", _context))
                .Returns(queryablePrivacyPolicies);

            var result = _crm.GetPrivacyPolicies().ToList();

            result.Select(policy => policy.Text).Should().BeEquivalentTo(
                new object[] { "Latest Active Web", "Not Latest 1", "Not Latest 2" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetPrivacyPolicies_IsCached()
        {
            var queryablePrivacyPolicies = MockPrivacyPolicies();
            _mockService.Setup(mock => mock.CreateQuery("dfe_privacypolicy", _context))
                .Returns(queryablePrivacyPolicies);

            var result1 = _crm.GetPrivacyPolicies();
            var result2 = _crm.GetPrivacyPolicies();

            result1.Should().BeEquivalentTo(result2);
            _mockService.Verify(mock => mock.CreateQuery("dfe_privacypolicy", _context), Times.Once);
        }

        [Fact]
        public void CandidateYetToAcceptPrivacyPolicy_WhenAlreadyAccepted_ReturnsFalse()
        {
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            var candidate = new Candidate() { Id = Guid.NewGuid(), PrivacyPolicy = policy };

            var entity = new Entity();
            entity["dfe_candidate"] = new EntityReference("dfe_candidate", (Guid)candidate.Id);
            entity["dfe_privacypolicynumber"] = new EntityReference("dfe_privacypolicynumber", policy.AcceptedPolicyId);

            _mockService.Setup(m => m.CreateQuery("dfe_candidateprivacypolicy", _context))
                .Returns(new List<Entity> { entity }.AsQueryable());

            var result = _crm.CandidateYetToAcceptPrivacyPolicy((Guid)candidate.Id, policy.AcceptedPolicyId);

            result.Should().BeFalse();
        }

        [Fact]
        public void CandidateYetToAcceptPrivacyPolicy_WhenNotYetAccepted_ReturnsTrue()
        {
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            var candidate = new Candidate() { Id = Guid.NewGuid(), PrivacyPolicy = policy };

            var entity = new Entity();
            entity["dfe_candidate"] = new EntityReference("dfe_candidate", (Guid)candidate.Id);
            entity["dfe_privacypolicynumber"] = new EntityReference("dfe_privacypolicynumber", policy.AcceptedPolicyId);

            _mockService.Setup(m => m.CreateQuery("dfe_candidateprivacypolicy", _context))
                .Returns(new List<Entity> { entity }.AsQueryable());

            var result = _crm.CandidateYetToAcceptPrivacyPolicy((Guid)candidate.Id, Guid.NewGuid());

            result.Should().BeTrue();
        }

        [Fact]
        public void CandidateYetToRegisterForTeachingEvent_WhenAlreadyRegistered_ReturnsFalse()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };

            var entity = new Entity();
            entity["msevtmgt_contactid"] = new EntityReference("dfe_candidate", (Guid)candidate.Id);
            entity["msevtmgt_eventid"] = new EntityReference("msevtmgt_event", (Guid)teachingEvent.Id);

            _mockService.Setup(m => m.CreateQuery("msevtmgt_eventregistration", _context))
                .Returns(new List<Entity> { entity }.AsQueryable());

            var result = _crm.CandidateYetToRegisterForTeachingEvent((Guid)candidate.Id, (Guid)teachingEvent.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public void CandidateYetToRegisterForTeachingEvent_WhenNotYetRegistered_ReturnsTrue()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };

            var entity = new Entity();
            entity["msevtmgt_contactid"] = new EntityReference("dfe_candidate", (Guid)candidate.Id);
            entity["msevtmgt_eventid"] = new EntityReference("msevtmgt_event", (Guid)teachingEvent.Id);

            _mockService.Setup(m => m.CreateQuery("dfe_candidateprivacypolicy", _context))
                .Returns(new List<Entity> { entity }.AsQueryable());

            var result = _crm.CandidateYetToRegisterForTeachingEvent((Guid)candidate.Id, Guid.NewGuid());

            result.Should().BeTrue();
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
            _mockService.Setup(mock => mock.CreateQuery("contact", _context)).Returns(MockCandidates());
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(), 
                new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), _context));

            var result = _crm.GetCandidate(request);

            result?.FirstName.Should().Be(expectedFirstName);
        }

        [Fact]
        public void Save_MapsEntityAndSavesContext()
        {
            var mockCandidate = new Mock<Candidate>();

            _crm.Save(mockCandidate.Object);

            mockCandidate.Verify(mock => mock.ToEntity(_crm, _context));
            _mockService.Verify(mock => mock.SaveChanges(_context));
        }

        [Fact]
        public void AddLink_ProxiesToService()
        {
            var source = new Entity("parent");
            var target = new Entity("child");
            var relationship = new Relationship("child");

            _crm.AddLink(source, relationship, target, _context);

            _mockService.Verify(mock => mock.AddLink(source, relationship, target, _context));
        }

        [Fact]
        public void RelatedEntities_ProxiesToService()
        {
            var entity = new Entity("parent");
            const string attributeName = "children";
            var relatedEntities = new List<Entity>() { new Entity("mock") };
            _mockService.Setup(mock => mock.RelatedEntities(entity, attributeName)).Returns(relatedEntities);

            var result = _crm.RelatedEntities(entity, attributeName, "mock");

            result.Should().BeEquivalentTo(relatedEntities);
        }

        [Fact]
        public void RelatedEntities_ExtractsFromParent()
        {
            var childId = Guid.NewGuid();
            var entity = new Entity("parent");
            entity.Attributes.Add("children.childid", new AliasedValue("child", "children", childId));
            const string attributeName = "children";
            var relatedEntity = new Entity() { Id = childId };
            relatedEntity.Attributes.Add("childid", childId);

            var result = _crm.RelatedEntities(entity, attributeName, "child");

            result.Should().BeEquivalentTo(new List<Entity>() { relatedEntity });
        }

        [Fact]
        public void MappableEntity_CallsNewEntityOnServiceWhenIdNull()
        {
            const string entityName = "entity";
            var newEntity = new Entity("mock");
            _mockService.Setup(mock => mock.NewEntity(entityName, _context)).Returns(newEntity);

            var result = _crm.MappableEntity(entityName, null, _context);

            result.Should().Be(newEntity);
        }

        [Fact]
        public void MappableEntity_CallsBlankExistingEntityOnServiceWhenIdNotNull()
        {
            const string entityName = "entity";
            var id = Guid.NewGuid();
            var existingEntity = new Entity("mock");
            _mockService.Setup(mock => mock.BlankExistingEntity(entityName, id, _context))
                .Returns(existingEntity);

            var result = _crm.MappableEntity(entityName, id, _context);

            result.Should().Be(existingEntity);
        }

        private static IQueryable<Entity> MockTeachingEvents()
        {
            var event1 = new Entity("msevtmgt_event");
            event1["msevtmgt_name"] = "Event 1";
            event1["msevtmgt_eventstartdate"] = DateTime.Now.AddDays(5);

            var event2 = new Entity("msevtmgt_event") { Id = FindEventGuid };
            event2["msevtmgt_name"] = "Event 2";
            event2["dfe_event_type"] = new OptionSetValue(123);
            event2["msevtmgt_eventstartdate"] = DateTime.Now.AddDays(1);

            var event3 = new Entity("msevtmgt_event");
            event3["msevtmgt_name"] = "Event 3";
            event3["msevtmgt_eventstartdate"] = DateTime.Now.AddDays(10);

            var event4 = new Entity("msevtmgt_event");
            event4["msevtmgt_name"] = "Event 4";
            event4["dfe_event_type"] = new OptionSetValue(123);
            event4["msevtmgt_eventstartdate"] = DateTime.Now.AddDays(3);

            var event5 = new Entity("msevtmgt_event");
            event5["msevtmgt_name"] = "Event 5";
            event5["msevtmgt_eventstartdate"] = DateTime.Now.AddDays(15);

            return new[] { event1, event2, event3, event4, event5 }.AsQueryable();
        }

        private static IQueryable<Entity> MockCandidates()
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

            return new[] { candidate1, candidate2, candidate3 }.AsQueryable();
        }

        private static IQueryable<Entity> MockPrivacyPolicies()
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

            return new[] { policy1, policy2, policy3, policy4, policy5, policy6 }.AsQueryable();
        }

        private static IQueryable<Entity> MockCountries()
        {
            var country1 = new Entity("dfe_country");
            country1["dfe_name"] = "Country 1";

            var country2 = new Entity("dfe_country");
            country2["dfe_name"] = "Country 2";

            var country3 = new Entity("dfe_country");
            country3["dfe_name"] = "Country 3";

            return new[] { country1, country2, country3 }.AsQueryable();
        }

        private static List<PickListItem> MockInitialTeacherTrainingYears()
        {
            var year1 = new PickListItem { PickListItemId = 1, DisplayLabel = "2010" };
            var year2 = new PickListItem { PickListItemId = 2, DisplayLabel = "2011" };
            var year3 = new PickListItem { PickListItemId = 3, DisplayLabel = "2012" };

            return new List<PickListItem> { year1, year2, year3 };
        }
    }
}