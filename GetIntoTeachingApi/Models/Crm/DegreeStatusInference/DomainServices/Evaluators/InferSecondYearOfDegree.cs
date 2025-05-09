﻿using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators
{
    /// <summary>
    /// Provides logic for inferring whether a candidate is in the second year of their degree based on the graduation year provisioned.
    /// </summary>
    public sealed class InferSecondYearOfDegree : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus>
    {
        /// <summary>
        /// Defines the number of years remaining for the candidate to be considered to be in their second year.
        /// </summary>
        private const int RemainingDegreeDuration = 1;

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
            DateTimeOffset currentDate =
                evaluationRequest.CurrentCalendarYearProvider.DateTimeToday.AddYears(RemainingDegreeDuration);

            return
                currentDate <= evaluationRequest.YearOfGraduation.GetProposedGraduationEndDate() &&
                currentDate >= evaluationRequest.YearOfGraduation.GetProposedGraduationStartDate();
        }

        /// <summary>
        /// Performs the 'second year' evaluation based on the year of graduation parameters provided.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
        /// </param>
        /// <returns>
        /// The <see cref="DegreeStatus"/> associated with the evaluation, i.e. SecondYear = 222750002.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Exception thrown when the evaluation cannot be performed.
        /// </exception>
        public DegreeStatus Evaluate(DegreeStatusInferenceRequest evaluationRequest) =>
            CanEvaluate(evaluationRequest) ? DegreeStatus.SecondYear :
                throw new ArgumentOutOfRangeException(nameof(evaluationRequest),
                    "Graduation year provided must be 1 year from the current academic year.");
    }
}
