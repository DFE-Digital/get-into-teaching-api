using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices
{
    /// <summary>
    /// 
    /// </summary>
    public struct GraduationYear : IEquatable<GraduationYear>, IEquatable<DateTime>, IEquatable<int>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When <see cref="year"/> is not within the range from <value>1</value> to <value>9999</value>.
        /// </exception>
        public GraduationYear(int year)
        {
            // same limits as DateTime 
            // be careful when changing this values, because it might break
            // conversion from and to DateTime 
            const int min = 1;
            const int max = 9999;

            if (year < min || year > max)
            {
                //var message = string.Format("Year must be between {0} and {1}.", min, max); // Use local settings
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            _value = year;
        }

        private readonly int _value;

        public bool Equals(GraduationYear other)
        {
            return _value == other._value;
        }

        public bool Equals(DateTime other)
        {
            return _value == other.Year;
        }

        public bool Equals(int other)
        {
            return _value == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj is GraduationYear) return Equals((GraduationYear)obj);
            if (obj is int) return Equals((int)obj);
            if (obj is DateTime) return Equals((DateTime)obj);
            return false;
        }

        public static GraduationYear MinValue
        {
            get
            {
                return new GraduationYear(DateTime.MinValue.Year);
            }
        }

        public static GraduationYear MaxValue
        {
            get
            {
                return new GraduationYear(DateTime.MaxValue.Year);
            }
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(GraduationYear left, GraduationYear right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GraduationYear left, GraduationYear right)
        {
            return !left.Equals(right);
        }

        //public override string ToString()
        //{
        //    return _value.ToString();
        //}

        //public string ToString(IFormatProvider formatProvider)
        //{
        //    return _value.ToString(formatProvider);
        //}

        //public string ToString(string format)
        //{
        //    return _value.ToString(format);
        //}

        //public string ToString(string format, IFormatProvider formatProvider)
        //{
        //    return _value.ToString(format, formatProvider);
        //}

        //public DateTime ToDateTime()
        //{
        //    return new DateTime(_value, 1, 1).Year;
        //}

        /// <summary>
        /// 
        /// </summary>
        public readonly int GetYear()
        {
            return _value;
        }

        public static implicit operator DateTime(GraduationYear year)
        {
            return new DateTime(year._value, 1, 1);
        }

        public static explicit operator GraduationYear(DateTime dateTime)
        {
            return new GraduationYear(dateTime.Year);
        }

        public static explicit operator int(GraduationYear year)
        {
            return year._value;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="year"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentOutOfRangeException">
        /////     When <see cref="year"/> is not within the range from <value>1</value> to <value>9999</value>.
        ///// </exception>
        //public static explicit operator GraduationYear(int year)
        //{
        //    return new GraduationYear(year);
        //}
    }
}
