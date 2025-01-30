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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfYearsAhead"></param>
        /// <returns></returns>
        int ToYearsAheadInt(int numberOfYearsAhead);
    }
}
