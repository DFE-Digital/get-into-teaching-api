using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InferAlreadyHasDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest) => evaluationRequest.Equals(DateTime.Today.Year);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             (evaluationRequest.YearOfGraduation.Equals(evaluationRequest
                 .CurrentCalendarYearProvider.ToYearInt()) ? DegreeStatus.HasDegree :
                    throw new ArgumentOutOfRangeException(
                        nameof(evaluationRequest), "Year must be the current year."));
    }
}