using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InferFinalYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
     /// 
     /// </summary>
        private const int RemainingDegreeDuration = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            evaluationRequest.YearOfGraduation.GetYear()
                .Equals(evaluationRequest.CurrentCalendarYearProvider.ToYearsAheadInt(RemainingDegreeDuration));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            CanEvaluate(evaluationRequest) ? DegreeStatus.FinalYear :
                throw new ArgumentOutOfRangeException(
                    nameof(evaluationRequest),
                    $"Year must be {RemainingDegreeDuration} years from {DateTime.Today.Year}.");
    }
}
