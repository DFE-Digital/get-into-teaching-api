using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference
{
    /// <summary>
    /// Defines the contract for all members wishing to infer degree status from the graduation year provisioned.
    /// </summary>
    public interface IInferDegreeStatus
    {
        /// <summary>
        /// The raw graduation year provisioned.
        /// </summary>
        int? GraduationYear { get; set; }

        /// <summary>
        /// The inferred graduation date based on the graduation year provisioned.
        /// </summary>
        DateTime? InferredGraduationDate { get; set; }

        /// <summary>
        /// Provides logic to conditionally infer the degree status based on
        /// the graduation year, if the graduation year is provisioned.
        /// </summary>
        /// <param name="degreeStatusDomainService">
        /// Implementation of <see cref="IDegreeStatusDomainService"/> which
        /// provides the functionality for inferring degree status based on the proposed graduation year.
        /// </param>
        /// <param name="currentYearProvider">
        /// Implementation of <see cref="ICurrentYearProvider"/> which provides the current date/time
        /// as well as helper methods to convert the current year to an integer representation.
        /// </param>
        /// <returns>
        /// The nullable integer representation of the inferred degree status.
        /// </returns>
        int? InferDegreeStatus(
            IDegreeStatusDomainService degreeStatusDomainService,
            ICurrentYearProvider currentYearProvider);
    }
}
