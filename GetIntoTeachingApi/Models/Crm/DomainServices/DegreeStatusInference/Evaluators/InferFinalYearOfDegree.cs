using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public class InferFinalYearOfDegree : IEvaluator<GraduationYear, DegreeStatus>
    {/// <summary>
     /// 
     /// </summary>
        private const int RemainingDegreeDuration = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanEvaluate(GraduationYear evaluationRequest) =>
            evaluationRequest.Equals(DateTime.Today.AddYears(RemainingDegreeDuration).Year);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DegreeStatus Evaluate(GraduationYear evaluationRequest) =>
             (evaluationRequest.Equals(DateTime.Today.AddYears(RemainingDegreeDuration).Year)) ? DegreeStatus.FinalYear :
                throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                    $"Year must be {RemainingDegreeDuration} years from {DateTime.Today.Year}.");
    }
}
