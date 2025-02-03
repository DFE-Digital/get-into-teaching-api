using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDegreeStatusDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="degreeStatusInferenceRequest">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        int? GetInferredDegreeStatusFromGraduationYear(DegreeStatusInferenceRequest degreeStatusInferenceRequest);
    }
}