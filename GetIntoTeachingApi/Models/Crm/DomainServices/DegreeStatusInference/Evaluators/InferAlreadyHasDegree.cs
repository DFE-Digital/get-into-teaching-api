using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// ss
    /// </summary>
    public sealed class InferAlreadyHasDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// sss
        /// </summary>
        /// <param name="evaluationRequest">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// 
        /// </exception>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            evaluationRequest.YearOfGraduation.GetYear()
                .Equals(evaluationRequest.CurrentCalendarYearProvider.ToYearInt()) ||
                    evaluationRequest.YearOfGraduation.GetYear() < evaluationRequest.CurrentCalendarYearProvider.ToYearInt();

        /// <summary>
        /// sss
        /// </summary>
        /// <param name="evaluationRequest">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// 
        /// </exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             CanEvaluate(evaluationRequest) ? DegreeStatus.HasDegree :
                throw new ArgumentOutOfRangeException(
                    nameof(evaluationRequest), "Year must be the current year.");
    }
}