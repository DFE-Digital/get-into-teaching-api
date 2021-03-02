using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

// Ideally this would be in the test project, however the Fody weavers
// don't appear to run against the models in the test project so they
// are included here.
namespace GetIntoTeachingApi.Mocks
{
    [Entity("relatedMock")]
    public class MockRelatedModel : BaseModel
    {
        public MockRelatedModel()
            : base()
        {
        }

        public MockRelatedModel(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
