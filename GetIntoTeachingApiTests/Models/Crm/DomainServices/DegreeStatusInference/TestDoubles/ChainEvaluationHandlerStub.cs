﻿using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using System.Collections.Generic;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;

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
