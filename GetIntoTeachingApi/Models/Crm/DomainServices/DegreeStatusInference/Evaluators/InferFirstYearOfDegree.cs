using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InferFirstYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// 
        /// </summary>
        private const int RemainingDegreeDuration = 3;

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
        /// <exception cref="System.NotImplementedException">
        /// 
        /// </exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             (evaluationRequest.Equals(evaluationRequest.
                 CurrentCalendarYearProvider.ToYearsAheadInt(RemainingDegreeDuration))) ||
                (evaluationRequest.YearOfGraduation.GetNumberOfYearsAwayFromGraduating(
                    evaluationRequest.CurrentCalendarYearProvider.ToYearInt()) > RemainingDegreeDuration) ? DegreeStatus.FirstYear :
                        throw new ArgumentOutOfRangeException(nameof(evaluationRequest), "Year must be the current year.");
    }
}
