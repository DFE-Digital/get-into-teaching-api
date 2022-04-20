using System;
using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class InterviewTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Interview);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("DateAndTime").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "date_and_time");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("CancelledAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "cancelled_at");
        }

        [Fact]
        public void ToCrmModel_MapsToACrmApplicationInterviewModel()
        {
            var interview = new Interview()
            {
                Id = 123,
                DateAndTime = new DateTime(2021, 1, 2),
                CreatedAt = new DateTime(2021, 1, 3),
                UpdatedAt = new DateTime(2021, 1, 4),
                CancelledAt = new DateTime(2021, 1, 5),
            };

            var crmInterview = interview.ToCrmModel();

            crmInterview.FindApplyId.Should().Be(interview.Id.ToString());
            crmInterview.ScheduledAt.Should().Be(interview.DateAndTime);
            crmInterview.CreatedAt.Should().Be(interview.CreatedAt);
            crmInterview.UpdatedAt.Should().Be(interview.UpdatedAt);
            crmInterview.CancelledAt.Should().Be(interview.CancelledAt);
        }
    }
}
