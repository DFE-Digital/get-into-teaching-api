using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApiTests.Mocks
{
    [Entity("mock")]
    public class MockModel : BaseModel
    {
        [EntityField("dfe_field1", typeof(EntityReference), "dfe_list")]
        public Guid? Field1 { get; set; }
        [EntityField("dfe_field2", typeof(OptionSetValue))]
        public int? Field2 { get; set; }
        [EntityField("dfe_field3")]
        public string Field3 { get; set; }
        [EntityRelationship("dfe_mock_dfe_relatedmock_mock", typeof(MockRelatedModel))]
        public MockRelatedModel RelatedMock { get; set; }
        [EntityRelationship("dfe_mock_dfe_relatedmock_mocks", typeof(MockRelatedModel))]
        public IEnumerable<MockRelatedModel> RelatedMocks { get; set; }

        public MockModel() : base() { }

        public MockModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }

    [Entity("relatedMock")]
    public class MockRelatedModel : BaseModel
    {
        public MockRelatedModel() : base() { }

        public MockRelatedModel(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
