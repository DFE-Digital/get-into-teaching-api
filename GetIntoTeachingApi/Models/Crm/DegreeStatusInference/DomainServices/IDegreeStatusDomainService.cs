using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices
{
    /// <summary>
    /// Defines the contract for making requests for inferred degree status. The <see cref="IDegreeStatusDomainService"/>
    /// instance encapsulate the proposed graduation year on which the underlying inference logic is applied.
    /// </summary>
    public interface IDegreeStatusDomainService
    {
        /// <summary>
        /// Gets the inferred degree status from the proposed graduation year.
        /// </summary>
        /// <param name="degreeStatusInferenceRequest">
        /// This request parameter encapsulates the proposed graduation year and
        /// the <see cref="ICurrentYearProvider"/> implementation which provides year based conversion methods.
        /// </param>
        /// <returns>
        /// The nullable integer representation of the inferred degree status.
        /// </returns>
        int? GetInferredDegreeStatusFromGraduationYear(DegreeStatusInferenceRequest degreeStatusInferenceRequest);
    }
}