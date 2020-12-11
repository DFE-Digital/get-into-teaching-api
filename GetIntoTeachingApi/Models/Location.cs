using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using GetIntoTeachingApi.Database;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    public class Location
    {
        public enum SourceType
        {
            Unknown,
            Google,
            CSV,
        }

        public static readonly Regex OutwardOrFullPostcodeRegex = new Regex(
            $@"\A({OutwardPostcodePattern})\Z|\A({FullPostcodePattern})\z", RegexOptions.IgnoreCase);
        public static readonly Regex PostcodeRegex = new Regex(
            $@"\A({FullPostcodePattern})\Z", RegexOptions.IgnoreCase);

        private const string OutwardPostcodePattern = @"[A-Z][A-HJ-Y]?\d[A-Z\d]?";
        private const string InwardPostcodePattern = @"\d[A-Z]{2}|GIR ?0A{2}";
        private const string FullPostcodePattern = OutwardPostcodePattern + " ?" + InwardPostcodePattern;

        [Key]
        public string Postcode { get; set; }
        [Column(TypeName = "geography")]
        public Point Coordinate { get; set; }
        public SourceType Source { get; set; }

        public static string SanitizePostcode(string postcode)
        {
            var builder = new StringBuilder();

            // More efficient than string.Replace
            foreach (char c in postcode)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                builder.Append(char.ToLower(c));
            }

            return builder.ToString();
        }

        public Location()
        {
        }

        public Location(string postcode)
            : this()
        {
            Postcode = SanitizePostcode(postcode);
        }

        public Location(string postcode, double latitude, double longitude)
            : this(postcode)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            Coordinate = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        }

        public Location(string postcode, Point coordinate)
            : this(postcode)
        {
            Coordinate = coordinate;
        }

        public Location(string postcode, Point coordinate, SourceType source)
         : this(postcode, coordinate)
        {
            Source = source;
        }

        public Location(string postcode, double latitude, double longitude, SourceType source)
           : this(postcode, latitude, longitude)
        {
            Source = source;
        }
    }
}
