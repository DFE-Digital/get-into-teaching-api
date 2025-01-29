namespace GetIntoTeachingApi.Models.Crm.DomainServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDegreeStatusDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graduationYear"></param>
        /// <returns></returns>
        int? GetInferredDegreeStatusFromGraduationYear(GraduationYear graduationYear);
    }
}
