using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICurrentYearProvider
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime DateTimeToday { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int ToYearInt();
    }
}
