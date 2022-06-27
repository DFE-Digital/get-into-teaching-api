using System;
using System.Collections.Generic;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

// Ideally this would be in the test project, however the Fody weavers
// don't appear to run against the models in the test project so they
// are included here.
namespace GetIntoTeachingApi.Mocks
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
        [EntityField("dfe_field4", null, null, new string[] { "TEST" })]
        public string Field4 { get; set; }
        [EntityRelationship("dfe_mock_dfe_relatedmock_mock", typeof(MockRelatedModel))]
        public MockRelatedModel RelatedMock { get; set; }
        [EntityRelationship("dfe_mock_dfe_relatedmock_mocks", typeof(MockRelatedModel))]
        public IEnumerable<MockRelatedModel> RelatedMocks { get; set; }
        public string CompoundField => $"Field 4: {Field4}";
        public string FieldDefinedWithValue { get; set; } = "initial value";

        public MockModel()
            : base()
        {
        }

        public MockModel(Entity entity, ICrmService crm, IValidator<MockModel> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
