using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices
{
    /// <summary>
    /// Provides methods for converting the current
    /// <see cref="DateTimeOffset"/> to the integer based year format.
    /// </summary>
    public interface ICurrentYearProvider
    {
        /// <summary>
        /// Represents a point in time, typically expressed as a date
        /// and time of day, relative to Coordinated Universal Time (UTC).
        /// </summary>
        DateTimeOffset DateTimeToday { get; }

        /// <summary>
        /// Attempts to convert the year value from the <see cref="DateTimeOffset"/>
        /// instance provisioned, to it's integer representation, but moved ahead by the specified number of years.
        /// </summary>
        /// <param name="numberOfYearsAhead"></param>
        /// <returns>
        /// An integer representation of the <see cref="DateTimeOffset"/> year provided.
        /// </returns>
        DateTimeOffset ToYearsAhead(int numberOfYearsAhead);
    }
}