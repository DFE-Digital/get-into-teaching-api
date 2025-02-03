using System;
using System.Globalization;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct GraduationYear : IEquatable<DateTime>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly int _proposedGraduationYear;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When <see cref="year"/> is not within the range from <value>1</value> to <value>9999</value>.
        /// </exception>
        public GraduationYear(int year, ICurrentYearProvider currentYearProvider)
        {
            const int yearsAhead = 40;

            int minDate = currentYearProvider.ToYearInt();
            int maxDate = currentYearProvider.ToYearsAheadInt(yearsAhead);

            _proposedGraduationYear =
                (year >= minDate || year <= maxDate) ? year :
                 throw new ArgumentOutOfRangeException(
                     nameof(year), $"Year must be between {Convert(minDate)} and {Convert(maxDate)}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => _proposedGraduationYear.GetHashCode();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(GraduationYear left, GraduationYear right) => left.Equals(right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(GraduationYear left, GraduationYear right) => !left.Equals(right);

        /// <summary>
        /// 
        /// </summary>
        public readonly int GetYear() => _proposedGraduationYear;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentYear"></param>
        /// <returns></returns>
        public int GetNumberOfYearsAwayFromGraduating(int currentYear) => _proposedGraduationYear - currentYear;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public readonly DateTime Convert(int year)
        {
            const string YearFormat = "yyyyMMdd";

            return DateTime.ParseExact(
                year.ToString(CultureInfo.InvariantCulture), YearFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DateTime other) => _proposedGraduationYear == other.Year;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is GraduationYear graduationYear) return Equals(graduationYear);
            if (obj is int graduationYearInt) return Equals(graduationYearInt);
            if (obj is DateTime graduationYearDateTime) return Equals(graduationYearDateTime);

            return false;
        }
    }
}
