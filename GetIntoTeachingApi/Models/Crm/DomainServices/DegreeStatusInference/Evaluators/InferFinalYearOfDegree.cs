using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public class InferFinalYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
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
            evaluationRequest.Equals(evaluationRequest.
                CurrentCalendarYearProvider.DateTimeToday.AddYears(RemainingDegreeDuration).Year);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             (evaluationRequest.CurrentCalendarYearProvider.ToYearInt().Equals(evaluationRequest
                 .CurrentCalendarYearProvider.ToYearsAheadInt(RemainingDegreeDuration))) ? DegreeStatus.FinalYear :
                    throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                        $"Year must be {RemainingDegreeDuration} years from {DateTime.Today.Year}.");
    }
}
