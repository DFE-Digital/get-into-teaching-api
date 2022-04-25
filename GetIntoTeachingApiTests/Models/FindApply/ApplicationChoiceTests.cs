using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ApplicationChoiceTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(ApplicationChoice);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("Status").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "status");
            type.GetProperty("Provider").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "provider");
            type.GetProperty("Course").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "course");
            type.GetProperty("Interviews").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "interviews");
        }

        [Fact]
        public void ToCrmModel_MapsToACrmApplicationChoiceModel()
        {
            var interview = new Interview()
            {
                Id = 456,
                DateAndTime = new DateTime(2021, 1, 2),
                CreatedAt = new DateTime(2021, 1, 3),
                UpdatedAt = new DateTime(2021, 1, 4),
                CancelledAt = new DateTime(2021, 1, 5),
            };

            var choice = new ApplicationChoice()
            {
                Id = 123,
                CreatedAt = new DateTime(2021, 1, 3),
                UpdatedAt = new DateTime(2021, 1, 4),
                Status = "cancelled",
                Provider = new Provider() { Name = "Provider Name" },
                Course = new Course() { Id = Guid.NewGuid(), Name = "Course Name" },
                Interviews = new List<Interview>() { interview },
            };

            var crmChoice = choice.ToCrmModel();

            crmChoice.FindApplyId.Should().Be(choice.Id.ToString());
            crmChoice.CreatedAt.Should().Be(choice.CreatedAt);
            crmChoice.UpdatedAt.Should().Be(choice.UpdatedAt);
            crmChoice.StatusId.Should().Be((int)GetIntoTeachingApi.Models.Crm.ApplicationChoice.Status.Cancelled);
            crmChoice.Provider.Should().Be(choice.Provider.Name);
            crmChoice.CourseId.Should().Be(choice.Course.Id.ToString());
            crmChoice.CourseName.Should().Be(choice.Course.Name);
            crmChoice.Interviews.First().FindApplyId.Should().Be(interview.Id.ToString());
        }

        [Fact]
        public void ToCrmModel_WhenRelationshipsAreNull_MapsToACrmApplicationChoiceModel()
        {
            var choice = new ApplicationChoice()
            {
                Status = "cancelled",
                Provider = new Provider() { Name = "Provider Name" },
                Course = new Course() { Id = Guid.NewGuid(), Name = "Course Name" },
            };

            var crmChoice = choice.ToCrmModel();

            crmChoice.Interviews.Should().BeNull();
        }
    }
}
