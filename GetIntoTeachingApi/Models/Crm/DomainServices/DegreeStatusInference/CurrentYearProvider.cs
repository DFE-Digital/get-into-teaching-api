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
        public DateTimeOffset DateTimeToday => DateTimeOffset.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public int ToYearInt() => Convert.ToInt32(DateTimeToday.Year, CultureInfo.CurrentCulture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfYearsAhead">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToYearsAheadInt(int numberOfYearsAhead) =>
            Convert.ToInt32(DateTimeToday.AddYears(
                numberOfYearsAhead).Year, CultureInfo.CurrentCulture);
    }
}
