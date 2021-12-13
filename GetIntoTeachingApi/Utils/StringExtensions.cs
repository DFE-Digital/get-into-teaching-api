using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Utils
{
    public static class StringExtensions
    {
        private static readonly string[] TrueValues = new string[] { "true", "t", "1", "on" };

        public static string AsFormattedPostcode(this string str)
        {
            str = str?.Replace(" ", string.Empty);

            if (str == null || !Location.PostcodeRegex.IsMatch(str))
            {
                return null;
            }

            return str.ToUpper(CultureInfo.CurrentCulture).Insert(str.Length - Location.InwardPostcodeLength, " ");
        }

        public static string StripExitCode(this string str)
        {
            if (str == null)
            {
                return null;
            }

            return Regex.Replace(str, "^00", string.Empty);
        }

        public static string AsFormattedTelephone(this string str, bool international)
        {
            if (international && str != null)
            {
                // Remove non-digit characters.
                str = Regex.Replace(str, "[^0-9]", string.Empty);

                // Prefix the 00 exit code.
                str = $"00{str}";

                // Replace UK dial-in code with a 0.
                str = Regex.Replace(str, "^00440?", "0");
            }

            return str;
        }

        public static string NullIfEmptyOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }

        public static string ToPascalCase(this string str)
        {
            str = str.ToLower(CultureInfo.CurrentCulture).Replace("_", " ");
            var info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(str).Replace(" ", string.Empty);
        }

        public static bool ToBool(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            return TrueValues.Contains(str.Trim().ToLower(CultureInfo.CurrentCulture));
        }
    }
}
