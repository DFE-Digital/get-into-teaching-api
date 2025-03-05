using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles
{
    public static class ChainEvaluationHandlerStub
    {
        public static IEnumerable<IChainEvaluationHandler<
            DegreeStatusInferenceRequest, DegreeStatus>> ChainEvaluationHandlersStub<TRequest, TResponse>() =>
                new List<IChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>>()
                {
                    new ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>(new InferFirstYearOfDegree()),
                    new ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>(new InferSecondYearOfDegree()),
                    new ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>(new InferFinalYearOfDegree()),
                    new ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>(new InferAlreadyHasDegree())
                };
    }
}