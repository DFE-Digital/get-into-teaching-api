using System;
using System.Globalization;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CurrentYearProvider : ICurrentYearProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTimeToday => DateTime.Today;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ToYearInt() => Convert.ToInt32(DateTimeToday.Year, CultureInfo.CurrentCulture);
    }
}
