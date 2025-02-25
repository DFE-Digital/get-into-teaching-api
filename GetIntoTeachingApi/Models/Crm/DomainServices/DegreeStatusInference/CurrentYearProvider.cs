using System;
using System.Globalization;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// Provides an implementation of <see cref="ICurrentYearProvider"/> which provides the current date/time
    /// as well as helper methods to convert the current year to an integer representation.
    /// </summary>
    public sealed class CurrentYearProvider : ICurrentYearProvider
    {
        /// <summary>
        /// Gets todays date and time in Coordinated Universal Time (UTC) format.
        /// </summary>
        public DateTimeOffset DateTimeToday => DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the current year as an integer.
        /// </summary>
        /// <returns>
        /// An integer representation of the current year.
        /// </returns>
        public int ToYearInt() => Convert.ToInt32(DateTimeToday.Year, CultureInfo.CurrentCulture);

        /// <summary>
        /// Gets the current year as an integer, but moved ahead by the specified number of years.
        /// </summary>
        /// <param name="numberOfYearsAhead">
        /// Specifies the number of years to move ahead.
        /// </param>
        /// <returns>
        /// An integer representation of the current year moved ahead by the specified number of years.
        /// </returns>
        public int ToYearsAheadInt(int numberOfYearsAhead) =>
            Convert.ToInt32(DateTimeToday.AddYears(
                numberOfYearsAhead).Year, CultureInfo.CurrentCulture);
    }
}
