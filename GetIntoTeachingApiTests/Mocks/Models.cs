using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApiTests.Mocks
{
    [Entity(LogicalName = "mock")]
    public class MockModel : BaseModel
    {
        [EntityField(Name = "dfe_field1", Type = typeof(EntityReference), Reference = "dfe_list")]
        public Guid? Field1 { get; set; }
        [EntityField(Name = "dfe_field2", Type = typeof(OptionSetValue))]
        public int? Field2 { get; set; }
        [EntityField(Name = "dfe_field3")]
        public string Field3 { get; set; }
        [EntityRelationship(Name = "dfe_mock_dfe_relatedmock_mock", Type = typeof(MockRelatedModel))]
        public MockRelatedModel RelatedMock { get; set; }
        [EntityRelationship(Name = "dfe_mock_dfe_relatedmock_mocks", Type = typeof(MockRelatedModel))]
        public IEnumerable<MockRelatedModel> RelatedMocks { get; set; }

        public MockModel() : base() { }

        public MockModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }

    [Entity(LogicalName = "relatedMock")]
    public class MockRelatedModel : BaseModel
    {
        public MockRelatedModel() : base() { }

        public MockRelatedModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
