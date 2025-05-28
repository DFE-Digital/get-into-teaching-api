using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices
{
    /// <summary>
    /// Defines an immutable class for defining graduation year boundaries (start and end window).
    /// </summary>
    public readonly struct GraduationYear
    {
        private readonly DateTimeOffset _proposedGraduationYearEndDate;
        private readonly DateTimeOffset _proposedGraduationYearStartDate;

        /// <summary>
        /// Creates an immutable instance of <see cref="GraduationYear"/>
        /// with the specified graduation window start and end dates.
        /// </summary>
        /// <param name="year">
        /// The graduation year specified as an integer.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Error thrown when <see cref="year"/> is not within the range from <value>1</value> to <value>9999</value>.
        /// </exception>
        public GraduationYear(int year, ICurrentYearProvider currentYearProvider)
        {
            const int MaximumYearsAhead = 40;
            TimeSpan offset = new(1, 0, 0);

            DateTimeOffset maximumFutureDate = currentYearProvider.ToYearsAhead(MaximumYearsAhead);

            // Set the graduation end window to 31st August for the year provided.
            DateTimeOffset proposedGraduationYearEndDate =
                new(year, month: 08, day: 31, hour: 23, minute: 59, second: 59, offset);

            _proposedGraduationYearEndDate =
                proposedGraduationYearEndDate <= maximumFutureDate ? proposedGraduationYearEndDate :
                    throw new ArgumentOutOfRangeException(
                        nameof(year), $"Year must be less than {maximumFutureDate}");

            // Set the graduation start window to 1st September for the previous year provided.
            _proposedGraduationYearStartDate =
                new DateTimeOffset(year - 1, month: 09, day: 01, hour: 00, minute: 00, second: 00, offset);
        }

        /// <summary>
        /// Gets the proposed graduation year end date encapsulated in the current instance.
        /// </summary>
        public readonly DateTimeOffset GetProposedGraduationEndDate() => _proposedGraduationYearEndDate;

        /// <summary>
        /// Gets the proposed graduation year start date encapsulated in the current instance.
        /// </summary>
        public readonly DateTimeOffset GetProposedGraduationStartDate() => _proposedGraduationYearStartDate;
    }
}
