using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Mocks;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class MappingInfoTests
    {
        private readonly MappingInfo _mappingInfo;

        public MappingInfoTests()
        {
            _mappingInfo = new MappingInfo(typeof(MockModel));
        }

        [Fact]
        public void Constructor_MapsClass()
        {
            _mappingInfo.Class.Should().Be("GetIntoTeachingApi.Mocks.MockModel");
            _mappingInfo.LogicalName.Should().Be("mock");
        }

        [Fact]
        public void Constructor_MapsProperties()
        {
            var fields = new Dictionary<string, IDictionary<string, string>>()
            {
                { "Field1", new Dictionary<string, string>() { { "Name", "dfe_field1" }, { "Type", "Microsoft.Xrm.Sdk.EntityReference" }, { "Reference", "dfe_list" } } },
                { "Field2", new Dictionary<string, string>() { { "Name", "dfe_field2" }, { "Type", "Microsoft.Xrm.Sdk.OptionSetValue" } } },
                { "Field3", new Dictionary<string, string>() { { "Name", "dfe_field3" } } },
            };

            var relationships = new Dictionary<string, IDictionary<string, string>>()
            {
                { "RelatedMock", new Dictionary<string, string>() { { "Name", "dfe_mock_dfe_relatedmock_mock" }, { "Type", "GetIntoTeachingApi.Mocks.MockRelatedModel" } } },
                { "RelatedMocks", new Dictionary<string, string>() { { "Name", "dfe_mock_dfe_relatedmock_mocks" }, { "Type", "GetIntoTeachingApi.Mocks.MockRelatedModel" } } },
            };

            _mappingInfo.Fields.Should().BeEquivalentTo(fields);
            _mappingInfo.Relationships.Should().BeEquivalentTo(relationships);
        }
    }
}
