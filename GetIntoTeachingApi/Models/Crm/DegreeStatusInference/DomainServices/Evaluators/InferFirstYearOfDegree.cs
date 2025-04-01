using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators
{
    /// <summary>
    /// Provides logic for inferring whether a candidate is in the first year of their degree based on the graduation year provisioned.
    /// </summary>
    public sealed class InferFirstYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// Defines the number of years remaining for the candidate to be considered to be in their first year.
        /// </summary>
        private const int RemainingDegreeDuration = 2;

        /// <summary>
        /// Check to assess whether the evaluation can be performed based on the year of graduation parameters provided.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the evaluation can be performed.
        /// </returns>
        public bool CanEvaluate(DegreeStatusInferenceRequest evaluationRequest)
        {
            DateTimeOffset graduationYearStartDate =
                evaluationRequest.YearOfGraduation.GetProposedGraduationStartDate();
            DateTimeOffset currentDate =
                evaluationRequest.CurrentCalendarYearProvider.DateTimeToday.AddYears(RemainingDegreeDuration);

            return
                (currentDate <= evaluationRequest.YearOfGraduation.GetProposedGraduationEndDate() &&
                currentDate >= graduationYearStartDate) ||
                currentDate < graduationYearStartDate;
        }

        /// <summary>
        /// Performs the 'first year' evaluation based on the year of graduation parameters provided.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
        /// </param>
        /// <returns>
        /// The <see cref="DegreeStatus"/> associated with the evaluation, i.e. FirstYear = 222750003.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Exception thrown when the evaluation cannot be performed.
        /// </exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
             CanEvaluate(evaluationRequest) ? DegreeStatus.FirstYear :
                throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                    "Graduation year provided must be 2 or more years from the current academic year.");
    }
}
