using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ApplicationFormTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(ApplicationForm);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("SubmittedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "submitted_at");
            type.GetProperty("ApplicationStatus").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_status");
            type.GetProperty("ApplicationPhase").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_phase");
            type.GetProperty("RecruitmentCycleYear").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "recruitment_cycle_year");
            type.GetProperty("ApplicationChoices").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_choices");
            type.GetProperty("References").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "references");
            type.GetProperty("Qualifications").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "qualifications");
            type.GetProperty("PersonalStatement").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "personal_statement");
        }

        [Fact]
        public void ToCrmModel_MapsToACrmApplicationFormModel()
        {
            var reference = new Reference()
            {
                Id = 123,
                FeedbackStatus = "cancelled",
                RefereeType = "employer",
                RequestedAt = new DateTime(2021, 1, 2),
            };

            var choice = new ApplicationChoice()
            {
                Id = 456,
                CreatedAt = new DateTime(2021, 1, 3),
                UpdatedAt = new DateTime(2021, 1, 4),
                Status = "cancelled",
                Provider = new Provider() { Name = "Provider Name" },
                Course = new Course() { Id = Guid.NewGuid(), Name = "Course Name" },
            };

            var form = new ApplicationForm()
            {
                Id = 789,
                CreatedAt = new DateTime(2021, 1, 5),
                UpdatedAt = new DateTime(2021, 1, 6),
                SubmittedAt = new DateTime(2021, 1, 7),
                ApplicationStatus = "never_signed_in",
                ApplicationPhase = "apply_1",
                RecruitmentCycleYear = 2022,
                ApplicationChoices = new ApplicationResponse<IEnumerable<ApplicationChoice>>() { Completed = true, Data = new List<ApplicationChoice> { choice } },
                References = new ApplicationResponse<IEnumerable<Reference>>() { Completed = false, Data = new List<Reference> { reference } },
                Qualifications = new ApplicationResponse<IEnumerable<object>>() { Completed = true },
                PersonalStatement = new ApplicationResponse<IEnumerable<object>>() { Completed = null },
            };

            var crmForm = form.ToCrmModel();

            crmForm.FindApplyId.Should().Be(form.Id.ToString());
            crmForm.CreatedAt.Should().Be(form.CreatedAt);
            crmForm.UpdatedAt.Should().Be(form.UpdatedAt);
            crmForm.StatusId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Status.NeverSignedIn);
            crmForm.PhaseId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.Phase.Apply1);
            crmForm.RecruitmentCycleYearId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationForm.RecruitmentCycleYear.Year2022);
            crmForm.Choices.First().FindApplyId.Should().Be(choice.Id.ToString());
            crmForm.References.First().FindApplyId.Should().Be(reference.Id.ToString());
            crmForm.ApplicationChoicesCompleted.Should().Be(form.ApplicationChoices.Completed);
            crmForm.ReferencesCompleted.Should().Be(form.References.Completed);
            crmForm.QualificationsCompleted.Should().Be(form.Qualifications.Completed);
            crmForm.PersonalStatementCompleted.Should().Be(form.PersonalStatement.Completed);
        }

        [Fact]
        public void ToCrmModel_WhenRelationshipsAreNull_MapsToACrmApplicationFormModel()
        {
            var form = new ApplicationForm() { ApplicationStatus = "never_signed_in", ApplicationPhase = "apply_1" };

            var crmForm = form.ToCrmModel();

            crmForm.Choices.Should().BeNull();
            crmForm.References.Should().BeNull();
        }
    }
}
