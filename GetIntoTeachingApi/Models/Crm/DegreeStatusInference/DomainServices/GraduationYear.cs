﻿using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices
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
            const int MaximumYearsAhead = 40;
            int maxDate = currentYearProvider.ToYearsAheadInt(MaximumYearsAhead);

            _proposedGraduationYear =
                year <= maxDate ? year :
                    throw new ArgumentOutOfRangeException(
                        nameof(year), $"Year must be less than {maxDate}");
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
                return graduationYear.GetYear() == _proposedGraduationYear;
            if (obj is int graduationYearInt)
                return graduationYearInt == _proposedGraduationYear;
            if (obj is DateTime graduationYearDateTime)
                return graduationYearDateTime.Year == _proposedGraduationYear;

            return false;
        }
    }
}
