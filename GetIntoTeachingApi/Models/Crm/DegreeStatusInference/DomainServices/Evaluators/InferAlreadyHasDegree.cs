using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators
{
    /// <summary>
    /// Provides logic for inferring whether a candidate already has a degree based on the graduation year provisioned.
    /// </summary>
    public sealed class InferAlreadyHasDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// Check to assess whether the evaluation can be performed based on the year of graduation parameters provided.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the evaluation can be performed.
        /// </returns>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            evaluationRequest.CurrentCalendarYearProvider.DateTimeToday >
            evaluationRequest.YearOfGraduation.GetProposedGraduationEndDate();

        /// <summary>
        /// Performs the 'has degree' evaluation based on the year of graduation parameters provided.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
        /// </param>
        /// <returns>
        /// The <see cref="DegreeStatus"/> associated with the evaluation, i.e. HasDegree = 222750000.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Exception thrown when the evaluation cannot be performed.
        /// </exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             CanEvaluate(evaluationRequest) ? DegreeStatus.HasDegree :
                throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                    "Graduation year must be ahead of the current academic year.");
    }
}