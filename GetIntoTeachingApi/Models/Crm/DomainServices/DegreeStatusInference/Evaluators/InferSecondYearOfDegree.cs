using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public class InferSecondYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// 
        /// </summary>
        private const int RemainingDegreeDuration = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            evaluationRequest.YearOfGraduation.Equals(evaluationRequest.
                CurrentCalendarYearProvider.DateTimeToday.AddYears(RemainingDegreeDuration).Year);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             (evaluationRequest.YearOfGraduation.Equals(evaluationRequest
                 .CurrentCalendarYearProvider.ToYearsAheadInt(RemainingDegreeDuration))) ? DegreeStatus.SecondYear :
                    throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                        $"Year must be {RemainingDegreeDuration} years from {DateTime.Today.Year}.");
    }
}
