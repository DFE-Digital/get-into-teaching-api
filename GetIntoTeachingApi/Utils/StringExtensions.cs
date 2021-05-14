using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Utils
{
    public static class StringExtensions
    {
        public static string AsFormattedPostcode(this string str)
        {
            str = str?.Replace(" ", string.Empty);

            if (str == null || !Location.PostcodeRegex.IsMatch(str))
            {
                return null;
            }

            return str.ToUpper().Insert(str.Length - Location.InwardPostcodeLength, " ");
        }

        public static string NullIfEmptyOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
    }
}
