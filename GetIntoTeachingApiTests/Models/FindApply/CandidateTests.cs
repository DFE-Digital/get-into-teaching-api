using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class CandidateTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Candidate);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("Attributes").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "attributes");
        }

        [Fact]
        public void ToCrmModel_MapsToACrmCandidateModel()
        {
            var form = new ApplicationForm()
            {
                Id = 123,
                ApplicationStatus = "offer_deferred",
                ApplicationPhase = "apply_1",
            };

            var candidate = new Candidate()
            {
                Id = "123",
                Attributes = new CandidateAttributes()
                {
                    Email = "email@address.com",
                    CreatedAt = new DateTime(2021, 1, 1),
                    UpdatedAt = new DateTime(2021, 1, 2),
                    ApplicationForms = new List<ApplicationForm>() { form },
                },
            };

            var crmCandidate = candidate.ToCrmModel();

            crmCandidate.FindApplyId.Should().Be(candidate.Id);
            crmCandidate.FindApplyCreatedAt.Should().Be(candidate.Attributes.CreatedAt);
            crmCandidate.FindApplyUpdatedAt.Should().Be(candidate.Attributes.UpdatedAt);
            crmCandidate.FindApplyStatusId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.OfferDeferred);
            crmCandidate.FindApplyPhaseId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1);
            crmCandidate.ApplicationForms.First().FindApplyId.Should().Be(form.Id.ToString());
        }

        [Fact]
        public void ToCrmModel_WhenApplicationFormsIsNull_MapsToACrmCandidateModel()
        {
            var candidate = new Candidate()
            {
                Id = "123",
                Attributes = new CandidateAttributes()
                {
                    ApplicationForms = null,
                },
            };

            var crmCandidate = candidate.ToCrmModel();

            crmCandidate.FindApplyId.Should().Be(candidate.Id);
            crmCandidate.FindApplyStatusId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn);
            crmCandidate.FindApplyPhaseId.Should().BeNull();
        }
    }
}
