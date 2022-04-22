using System;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ReferenceTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Reference);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("RequestedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "requested_at");
            type.GetProperty("FeedbackStatus").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "feedback_status");
            type.GetProperty("RefereeType").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "referee_type");
        }

        [Fact]
        public void ToCrmModel_MapsToACrmApplicationReferenceModel()
        {
            var reference = new Reference()
            {
                Id = 123,
                FeedbackStatus = "cancelled",
                RefereeType = "employer",
                RequestedAt = new DateTime(2021, 1, 2),
            };

            var crmReference = reference.ToCrmModel();

            crmReference.FindApplyId.Should().Be(reference.Id.ToString());
            crmReference.FeedbackStatusId.Should().Be((int)ApplicationReference.FeedbackStatus.Cancelled);
            crmReference.Type.Should().Be(reference.RefereeType);
            crmReference.RequestedAt.Should().Be(reference.RequestedAt);
        }
    }
}
