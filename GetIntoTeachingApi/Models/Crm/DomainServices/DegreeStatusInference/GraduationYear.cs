using System;
using System.Globalization;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// Defines an immutable class for defining graduation year, as well as providing methods to convert and compare it.
    /// </summary>
    public readonly struct GraduationYear : IEquatable<DateTime>
    {
        private readonly int _proposedGraduationYear;

        /// <summary>
        /// Creates an immutable instance of <see cref="GraduationYear"/> with the specified year.
        /// </summary>
        /// <param name="year">
        /// The graduation year specified as an integer.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Error thrown when <see cref="year"/> is not within the range from <value>1</value> to <value>9999</value>.
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
        /// Gets the hash-code for the current instance, allowing a comparison to
        /// be made between this object and another object of the same type.
        /// </summary>
        /// <returns>
        /// The hash-code for the current instance.
        /// </returns>
        public override int GetHashCode() => _proposedGraduationYear.GetHashCode();

        /// <summary>
        /// Operator overload to facilitate the equality check of two <see cref="GraduationYear"/> instances.
        /// </summary>
        /// <param name="left">
        /// Left hand side of the comparison.
        /// </param>
        /// <param name="right">
        /// Right hand side of the comparison.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the two instances are equal.
        /// </returns>
        public static bool operator ==(GraduationYear left, GraduationYear right) => left.Equals(right);

        /// <summary>
        /// Operator overload to facilitate the non-equality check of two <see cref="GraduationYear"/> instances.
        /// </summary>
        /// <param name="left">
        /// Left hand side of the comparison.
        /// </param>
        /// <param name="right">
        /// Right hand side of the comparison.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the two instances are equal.
        /// </returns>
        public static bool operator !=(GraduationYear left, GraduationYear right) => !left.Equals(right);

        /// <summary>
        /// Gets the year encapsulated in the current instance.
        /// </summary>
        public readonly int GetYear() => _proposedGraduationYear;

        /// <summary>
        /// Gets the number of years away from graduating based on the difference between current year and the proposed graduation year.
        /// </summary>
        /// <param name="currentYear">
        /// An integer value representing the current year.
        /// </param>
        /// <returns>
        /// An integer value representing the number of years away from graduating.
        /// </returns>
        public int GetNumberOfYearsAwayFromGraduating(int currentYear) => _proposedGraduationYear - currentYear;

        /// <summary>
        /// Allows the conversion of the year to a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="year">
        /// An integer value representing a specified year.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> object representing the year provided.
        /// </returns>
        public readonly DateTime Convert(int year)
        {
            const string YearFormat = "yyyyMMdd";

            return DateTime.ParseExact(
                year.ToString(CultureInfo.InvariantCulture), YearFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Equality check between the proposed graduation year <see cref="DateTime"/> instance and another <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="other">
        /// The <see cref="DateTime"/> instance to compare against.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the two instances are equal.
        /// </returns>
        public bool Equals(DateTime other) => _proposedGraduationYear == other.Year;

        /// <summary>
        /// Provides equality check between the current instance and another object.
        /// </summary>
        /// <param name="obj">
        /// A boxed instance on which to compare the proposed graduation year.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the two instances are equal.
        /// </returns>
        public override bool Equals(object obj)
         {
            if (obj is null)
            {
                return false;
            }

            if (obj is GraduationYear graduationYear)
                return Equals(graduationYear.GetYear() == _proposedGraduationYear);
            if (obj is int graduationYearInt)
                return Equals(graduationYearInt == _proposedGraduationYear);
            if (obj is DateTime graduationYearDateTime)
                return Equals(graduationYearDateTime.Year == _proposedGraduationYear);

            return false;
        }
    }
}
