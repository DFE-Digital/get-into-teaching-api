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
using Microsoft.Xrm.Sdk.Query;
using Xunit;
using System.Linq.Dynamic.Core;

namespace GetIntoTeachingApiTests.Services
{
    public class CrmServiceTests
    {
        private static readonly Guid JaneDoeGuid = new Guid("bf927e43-5650-44aa-859a-8297139b8ddd");
        private static readonly Guid JohnDoeGuid = new Guid("cf927e43-5650-44aa-859a-8297139b8eee");
        private static readonly string JaneDoeMagicLinkToken = "7898be2c4699719c8eca56ebd1fb8e6e";
        private readonly Mock<IOrganizationServiceAdapter> _mockService;
        private readonly OrganizationServiceContext _context;
        private readonly ICrmService _crm;

        public CrmServiceTests()
        {
            _mockService = new Mock<IOrganizationServiceAdapter>();
            _context = new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
            _mockService.Setup(mock => mock.Context()).Returns(_context);
            _crm = new CrmService(_mockService.Object);
        }

        [Fact]
        public void CheckStatus_WhenHealthy_ReturnsOk()
        {
            _mockService.Setup(mock => mock.CheckStatus()).Returns(HealthCheckResponse.StatusOk);

            _crm.CheckStatus().Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public void IsHealthy_WhenUnhealthy_ReturnsError()
        {
            _mockService.Setup(mock => mock.CheckStatus()).Returns("this is an error");

            _crm.CheckStatus().Should().Be("this is an error");
        }

        [Fact]
        public void GetLookupItems_ReturnsMatchingOrderedByIdAscending()
        {
            var queryableCountries = MockCountries();
            _mockService.Setup(mock => mock.CreateQuery("dfe_country", _context))
                .Returns(queryableCountries);

            var result = _crm.GetLookupItems("dfe_country").ToList();

            result.Select(country => country.Value).Should().BeEquivalentTo(
                new object[] { "Country 1", "Country 2", "Country 3" },
                options => options.WithStrictOrdering());
            result.Select(country => country.EntityName).Should().OnlyContain(name => name == "dfe_country");
        }

        [Fact]
        public void GetPickListItems_ReturnsMatchingOrderedByIdAscending()
        {
            var initialTeacherTrainingYears = MockInitialTeacherTrainingYears();
            _mockService.Setup(mock => mock.GetPickListItemsForAttribute("contact", "dfe_ittyear"))
                .Returns(initialTeacherTrainingYears);

            var result = _crm.GetPickListItems("contact", "dfe_ittyear").ToList();

            result.Select(year => year.Value).Should().BeEquivalentTo(new object[] { "2010", "2011", "2012" },
                options => options.WithStrictOrdering());
            result.Select(year => year.EntityName).Should().OnlyContain(name => name == "contact");
            result.Select(year => year.AttributeName).Should().OnlyContain(name => name == "dfe_ittyear");
        }

        [Fact]
        public void GetTeachingEvents_ReturnsAllNonDraftFutureDatedTeachingEventsOfTheCorrectType()
        {
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyTeachingEventsQueryExpression(q)))).Returns(MockTeachingEvents());

            var result = _crm.GetTeachingEvents();

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 1", "Event 2", "Event 3" },
                options => options.WithStrictOrdering());
        }

        private static bool VerifyTeachingEventsQueryExpression(QueryExpression query)
        {
            var hasEntityName = query.EntityName == "msevtmgt_event";
            var conditions = query.Criteria.Filters.First().Conditions;
            var status = new HashSet<object> { (int)TeachingEvent.Status.Open, (int)TeachingEvent.Status.Closed };
            var hasStatusCondition = conditions.Where(c => c.AttributeName == "dfe_eventstatus" &&
                c.Operator == ConditionOperator.In && c.Values.ToHashSet().IsSubsetOf(status)).Any();
            var hasFutureDatedCondition = conditions.Where(c => c.AttributeName == "msevtmgt_eventenddate" &&
                c.Operator == ConditionOperator.GreaterThan).Any();
            var types = new HashSet<object>() {
                (int)TeachingEvent.EventType.TrainToTeachEvent,
                (int)TeachingEvent.EventType.OnlineEvent,
                (int)TeachingEvent.EventType.ApplicationWorkshop,
                (int)TeachingEvent.EventType.SchoolOrUniversityEvent,
            };
            var hasTypeCondition = conditions.Where(c => c.AttributeName == "dfe_event_type" &&
                c.Operator == ConditionOperator.In && c.Values.ToHashSet().IsSubsetOf(types)).Any();
            var hasReadabaleIdCondition = conditions.Where(c => c.AttributeName == "dfe_websiteeventpartialurl" &&
                c.Operator == ConditionOperator.NotNull).Any();

            return hasEntityName && hasStatusCondition && hasFutureDatedCondition && hasTypeCondition && hasReadabaleIdCondition;
        }

        [Fact]
        public void GetCallbackBookingQuotas_ReturnsFutureQuotasUpTo14DaysInAdvanceExcludingFullyBooked()
        {
            var queryableQuotas = MockCallbackBookingQuotas();
            _mockService.Setup(mock => mock.CreateQuery("dfe_callbackbookingquota", _context))
                .Returns(queryableQuotas);

            var result = _crm.GetCallbackBookingQuotas().ToList();

            result.Select(quota => quota.NumberOfBookings).Should().BeEquivalentTo(
                new object[] { 4, 3, 2 },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetCallbackBookingQuota_ReturnsQuotaMatchingScheduledAt()
        {
            var queryableQuotas = MockCallbackBookingQuotas();
            _mockService.Setup(mock => mock.CreateQuery("dfe_callbackbookingquota", _context))
                .Returns(queryableQuotas);
            var quota = queryableQuotas.ToArray()[3];
            var startAt = quota.GetAttributeValue<DateTime>("dfe_starttime");

            var result = _crm.GetCallbackBookingQuota(startAt);

            result.StartAt.Should().Be(startAt);
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
        public void CandidateAlreadyHasLocalEventSubscriptionType_WhenHasLocalEventSubscription_ReturnsTrue()
        {
            var ids = new Guid[] { JaneDoeGuid };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(candidates);

            var result = _crm.CandidateAlreadyHasLocalEventSubscriptionType(JaneDoeGuid);

            result.Should().BeTrue();
        }

        [Fact]
        public void CandidateAlreadyHasLocalEventSubscriptionType_WhenHasSingleEventSubscription_ReturnsFalse()
        {
            var ids = new Guid[] { JohnDoeGuid };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(candidates);

            var result = _crm.CandidateAlreadyHasLocalEventSubscriptionType(JohnDoeGuid);

            result.Should().BeFalse();
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

        [Fact]
        public void GetCandidate_WithId_ReturnsCorrectly()
        {
            var ids = new Guid[] { JaneDoeGuid };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(candidates);

            var result = _crm.GetCandidate(JaneDoeGuid);

            result.Id.Should().Be(JaneDoeGuid);
        }

        [Fact]
        public void GetCandidate_WithNonExistentId_ReturnsNull()
        {
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, new Guid[0])))).Returns(new Entity[0]);

            var result = _crm.GetCandidate(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public void GetCandidates_ReturnsMatchingCandidates()
        {
            var ids = new Guid[] { JaneDoeGuid, JohnDoeGuid };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(candidates);

            var result = _crm.GetCandidates(ids);

            result.First().Id.Should().Be(JaneDoeGuid);
            result.Last().Id.Should().Be(JohnDoeGuid);
        }

        [Fact]
        public void GetCandidates_WithMissingCandidateIds_ReturnsMatchingCandidates()
        {
            var ids = new Guid[] { Guid.NewGuid(), JaneDoeGuid };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(candidates);

            var result = _crm.GetCandidates(ids);

            result.Count().Should().Be(1);
            result.First().Id.Should().Be(JaneDoeGuid);
        }

        [Fact]
        public void GetCandidate_WithNoMatches_ReturnsEmpty()
        {
            var ids = new Guid[] { Guid.NewGuid() };
            var candidates = MockCandidates().Where(c => ids.Contains(c.Id));
            _mockService.Setup(mock => mock.RetrieveMultiple(It.Is<QueryExpression>(
                q => VerifyCandidatesQueryExpression(q, ids)))).Returns(new Entity[0]);

            var result = _crm.GetCandidates(ids);

            result.Should().BeEmpty();
        }

        private static bool VerifyCandidatesQueryExpression(QueryExpression query, IEnumerable<Guid> ids)
        {
            var objectIds = ids.Select(id => (object)id).ToArray();
            var hasEntityName = query.EntityName == "contact";
            var conditions = query.Criteria.Filters.First().Conditions;
            var hasIdsCondition = conditions.Where(c => c.AttributeName == "contactid" &&
                c.Operator == ConditionOperator.In && c.Values.ToArray().SequenceEqual(objectIds)).Any();

            return hasEntityName && hasIdsCondition;
        }

        [Theory]
        [InlineData("john@doe.com", "New John", "Doe", "New John")]
        [InlineData("JOHN@doe.com", "New John", "Doe", "New John")]
        [InlineData("jane@doe.com", "Jane", "Doe", "Jane")]
        [InlineData(" jane@doe.com ", " Jane ", " Doe ", "Jane")]
        [InlineData("bob@doe.com", "Bob", "Doe", null)]
        [InlineData("inactive@doe.com", "Inactive", "Doe", null)]
        public void MatchCandidate_WithExistingCandidateRequest_MatchesOnNewestCandidateWithEmail(
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
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_servicesubscription_contact"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), _context));

            var result = _crm.MatchCandidate(request);

            result?.FirstName.Should().Be(expectedFirstName);
        }

        [Fact]
        public void MatchCandidate_WithMasterChildRecords_ReturnsMasterRecord()
        {
            var request = new ExistingCandidateRequest
            {
                Email = "master@record.com", 
                LastName = "Record", 
                DateOfBirth = new DateTime(2000, 1, 1)
            };
            _mockService.Setup(mock => mock.CreateQuery("contact", _context)).Returns(MockCandidates());
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_servicesubscription_contact"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), _context));

            var result = _crm.MatchCandidate(request);

            result?.FirstName.Should().Be("Master");
        }

        [Theory]
        [InlineData("7898be2c4699719c8eca56ebd1fb8e6e", new string[] { "Jane" })]
        [InlineData(" 7898be2c4699719c8eca56ebd1fb8e6e", new string[0])]
        [InlineData("6898be2c4699719c8eca56ebd1fb8e7d", new string[0])]
        [InlineData("", new string[0])]
        [InlineData(null, new string[0])]
        [InlineData("duplicated-token", new string[] { "Old John", "New John" })]
        public void MatchCandidates_WithMagicLinkToken_ReturnsMatchingCandidates(string token, IEnumerable<string> expectedFirstNames)
        {
            _mockService.Setup(mock => mock.CreateQuery("contact", _context)).Returns(MockCandidates());
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("dfe_contact_dfe_servicesubscription_contact"), _context));
            _mockService.Setup(mock => mock.LoadProperty(It.IsAny<Entity>(),
                new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), _context));

            var result = _crm.MatchCandidates(token);

            result.Select(c => c.FirstName).Should().BeEquivalentTo(expectedFirstNames);
        }

        [Fact]
        public void Save_MapsEntityAndSavesContext()
        {
            var entity = new Entity() { Id = Guid.NewGuid() };
            var mockCandidate = new Mock<Candidate>();
            // The id is actually set on SaveChanges, but mocked here for ease.
            mockCandidate.Setup(mock => mock.ToEntity(_crm, _context)).Returns(entity);

            _crm.Save(mockCandidate.Object);

            _mockService.Verify(mock => mock.SaveChanges(_context), Times.Once);
            mockCandidate.Object.Id.Should().Be(entity.Id);
        }

        [Fact]
        public void Save_WhenToEntityReturnsNull_DoesNotSaveContext()
        {
            var entity = new Entity() { Id = Guid.NewGuid() };
            var mockCandidate = new Mock<Candidate>();
            mockCandidate.Setup(m => m.ToEntity(_crm, _context)).Returns<Entity>(null);

            _crm.Save(mockCandidate.Object);

            _mockService.Verify(mock => mock.SaveChanges(_context), Times.Never);
            mockCandidate.Object.Id.Should().BeNull();
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
            event1["dfe_externaleventtitle"] = "Event 1";

            var event2 = new Entity("msevtmgt_event");
            event2["dfe_externaleventtitle"] = "Event 2";

            var event3 = new Entity("msevtmgt_event");
            event3["dfe_externaleventtitle"] = "Event 3";

            return new[] { event1, event2, event3 }.AsQueryable();
        }

        private static IQueryable<Entity> MockCandidates()
        {
            var candidate1 = new Entity("contact")
            {
                Id = JaneDoeGuid
            };
            candidate1["contactid"] = new EntityReference("contactid", JaneDoeGuid);
            candidate1["statecode"] = Candidate.Status.Active;
            candidate1["emailaddress1"] = "jane@doe.com";
            candidate1["firstname"] = "Jane";
            candidate1["lastname"] = "Doe";
            candidate1["modifiedon"] = DateTime.UtcNow;
            candidate1["dfe_websitemltoken"] = JaneDoeMagicLinkToken;
            candidate1["dfe_duplicatescorecalculated"] = 10.0;
            candidate1["dfe_gitiseventsservicesubscriptiontype"] = new OptionSetValue((int)Candidate.SubscriptionType.LocalEvent);

            var candidate2 = new Entity("contact")
            {
                Id = JohnDoeGuid
            };
            candidate2["contactid"] = new EntityReference("contactid", JohnDoeGuid);
            candidate2["statecode"] = Candidate.Status.Active;
            candidate2["emailaddress1"] = "john@doe.com";
            candidate2["firstname"] = "New John";
            candidate2["lastname"] = "Doe";
            candidate2["modifiedon"] = DateTime.UtcNow;
            candidate2["dfe_websitemltoken"] = "duplicated-token";
            candidate2["dfe_duplicatescorecalculated"] = 9.5;
            candidate2["dfe_gitiseventsservicesubscriptiontype"] = new OptionSetValue((int)Candidate.SubscriptionType.SingleEvent);

            var candidate3 = new Entity("contact");
            candidate3["statecode"] = Candidate.Status.Active;
            candidate3["emailaddress1"] = "john@doe.com";
            candidate3["firstname"] = "Old John";
            candidate3["lastname"] = "Doe";
            candidate3["dfe_websitemltoken"] = "duplicated-token";
            candidate3["modifiedon"] = DateTime.UtcNow.AddDays(-5);
            candidate3["dfe_duplicatescorecalculated"] = 8.3;

            var candidate4 = new Entity("contact");
            candidate4["statecode"] = Candidate.Status.Inactive;
            candidate4["emailaddress1"] = "inactive@doe.com";
            candidate4["firstname"] = "Inactive";
            candidate4["lastname"] = "Doe";
            candidate4["modifiedon"] = DateTime.UtcNow;
            candidate4["dfe_duplicatescorecalculated"] = 7.1;

            var candidate5 = new Entity("contact");
            candidate5["statecode"] = Candidate.Status.Inactive;
            candidate5["emailaddress1"] = "master@record.com";
            candidate5["firstname"] = "Child";
            candidate5["lastname"] = "Record";
            candidate5["modifiedon"] = DateTime.UtcNow;
            candidate5["birthdate"] = new DateTime(2000, 1, 1);
            candidate5["dfe_duplicatescorecalculated"] = 2.4;

            var candidate6 = new Entity("contact");
            candidate6["statecode"] = Candidate.Status.Inactive;
            candidate6["emailaddress1"] = "master@record.com";
            candidate6["firstname"] = "Child1";
            candidate6["lastname"] = "Record";
            candidate6["modifiedon"] = DateTime.UtcNow;
            candidate6["birthdate"] = new DateTime(2000, 1, 1);
            candidate6["dfe_duplicatescorecalculated"] = null;

            var candidate7 = new Entity("contact");
            candidate7["statecode"] = Candidate.Status.Inactive;
            candidate7["emailaddress1"] = "master@record.com";
            candidate7["firstname"] = "Master";
            candidate7["lastname"] = "Record";
            candidate7["modifiedon"] = DateTime.UtcNow.AddDays(-5);
            candidate7["birthdate"] = new DateTime(2000, 1, 1);
            candidate7["dfe_duplicatescorecalculated"] = 3.7;

            return new[]
            {
                candidate1, 
                candidate2,
                candidate3, 
                candidate4, 
                candidate5, 
                candidate6,
                candidate7
            }.AsQueryable();
        }

        private static IQueryable<Entity> MockCallbackBookingQuotas()
        {
            var quota1 = new Entity("dfe_callbackbookingquota");
            quota1["dfe_starttime"] = DateTime.UtcNow.AddDays(-1);
            quota1["dfe_websitenumberofbookings"] = 1;
            quota1["dfe_websitequota"] = 10;

            var quota2 = new Entity("dfe_callbackbookingquota");
            quota2["dfe_starttime"] = DateTime.UtcNow.AddDays(10);
            quota2["dfe_websitenumberofbookings"] = 2;
            quota2["dfe_websitequota"] = 10;

            var quota3 = new Entity("dfe_callbackbookingquota");
            quota3["dfe_starttime"] = DateTime.UtcNow.AddDays(1);
            quota3["dfe_websitenumberofbookings"] = 3;
            quota3["dfe_websitequota"] = 10;

            var quota4 = new Entity("dfe_callbackbookingquota");
            quota4["dfe_starttime"] = DateTime.UtcNow.AddMinutes(20);
            quota4["dfe_websitenumberofbookings"] = 4;
            quota4["dfe_websitequota"] = 10;

            var quota5 = new Entity("dfe_callbackbookingquota");
            quota5["dfe_starttime"] = DateTime.UtcNow.AddDays(15);
            quota5["dfe_websitenumberofbookings"] = 5;
            quota5["dfe_websitequota"] = 10;

            var quota6 = new Entity("dfe_callbackbookingquota");
            quota5["dfe_starttime"] = DateTime.UtcNow.AddDays(3);
            quota5["dfe_websitenumberofbookings"] = 10;
            quota5["dfe_websitequota"] = 10;

            return new[] { quota1, quota2, quota3, quota4, quota5, quota6 }.AsQueryable();
        }

        private static IQueryable<Entity> MockPrivacyPolicies()
        {
            var policy1 = new Entity("dfe_privacypolicy");
            policy1["dfe_details"] = "Latest Active Web";
            policy1["dfe_policytype"] = new OptionSetValue { Value = (int)PrivacyPolicy.Type.Web };
            policy1["createdon"] = DateTime.UtcNow.AddDays(-10);
            policy1["dfe_active"] = true;

            var policy2 = new Entity("dfe_privacypolicy");
            policy2["dfe_details"] = "Not Web";
            policy2["dfe_policytype"] = new OptionSetValue { Value = 123 };
            policy2["createdon"] = DateTime.UtcNow.AddDays(-5);
            policy2["dfe_active"] = true;

            var policy3 = new Entity("dfe_privacypolicy");
            policy3["dfe_policytype"] = new OptionSetValue { Value = (int)PrivacyPolicy.Type.Web };
            policy3["dfe_details"] = "Not Active";
            policy3["createdon"] = DateTime.UtcNow.AddDays(-3);
            policy3["dfe_active"] = false;

            var policy4 = new Entity("dfe_privacypolicy");
            policy4["dfe_details"] = "Not Latest 1";
            policy4["dfe_policytype"] = new OptionSetValue { Value = (int)PrivacyPolicy.Type.Web };
            policy4["createdon"] = DateTime.UtcNow.AddDays(-15);
            policy4["dfe_active"] = true;

            var policy5 = new Entity("dfe_privacypolicy");
            policy5["dfe_details"] = "Not Latest 2";
            policy5["dfe_policytype"] = new OptionSetValue { Value = (int)PrivacyPolicy.Type.Web };
            policy5["createdon"] = DateTime.UtcNow.AddDays(-20);
            policy5["dfe_active"] = true;

            var policy6 = new Entity("dfe_privacypolicy");
            policy6["dfe_details"] = "Not Latest 3";
            policy6["dfe_policytype"] = new OptionSetValue { Value = (int)PrivacyPolicy.Type.Web };
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

        private static IEnumerable<Microsoft.PowerPlatform.Cds.Client.CdsServiceClient.PickListItem> MockInitialTeacherTrainingYears()
        {
            var year1 = new Microsoft.PowerPlatform.Cds.Client.CdsServiceClient.PickListItem { PickListItemId = 1, DisplayLabel = "2010" };
            var year2 = new Microsoft.PowerPlatform.Cds.Client.CdsServiceClient.PickListItem { PickListItemId = 2, DisplayLabel = "2011" };
            var year3 = new Microsoft.PowerPlatform.Cds.Client.CdsServiceClient.PickListItem { PickListItemId = 3, DisplayLabel = "2012" };

            return new List<Microsoft.PowerPlatform.Cds.Client.CdsServiceClient.PickListItem> { year1, year2, year3 };
        }
    }
}