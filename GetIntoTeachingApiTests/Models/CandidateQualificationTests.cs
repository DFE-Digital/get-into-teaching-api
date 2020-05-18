using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateQualificationTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity["dfe_category"] = new OptionSetValue { Value = 1 };
            entity["dfe_type"] = new OptionSetValue { Value = 2 };
            entity["dfe_degreestatus"] = new OptionSetValue { Value = 3 };

            var qualification = new CandidateQualification(entity);

            qualification.Id.Should().Be(entity.Id);
            qualification.CategoryId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_category").Value);
            qualification.TypeId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_type").Value);
            qualification.DegreeStatusId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value);
        }

        [Fact]
        public void PopulateEntity_ReverseMapsCorrectly()
        {
            var qualification = new CandidateQualification()
            {
                Id = Guid.NewGuid(),
                CategoryId = 1,
                TypeId = 2,
                DegreeStatusId = 3,
            };

            var entity = new Entity("dfe_candidatequalification", (Guid) qualification.Id);
            qualification.PopulateEntity(entity);

            entity.GetAttributeValue<OptionSetValue>("dfe_category").Value.Should().Be(qualification.CategoryId);
            entity.GetAttributeValue<OptionSetValue>("dfe_type").Value.Should().Be(qualification.TypeId);
            entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value.Should().Be(qualification.DegreeStatusId);
        }
    }
}
