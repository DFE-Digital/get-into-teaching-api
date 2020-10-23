using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using GetIntoTeachingApi.Database;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    public class Location
    {
        public static readonly Regex PostcodeRegex = new Regex(
            @"^([A-Z][A-HJ-Y]?\d[A-Z\d]? ?\d[A-Z]{2}|GIR ?0A{2})$", RegexOptions.IgnoreCase);

        [Key]
        public string Postcode { get; set; }
        [Column(TypeName = "geography")]
        public Point Coordinate { get; set; }

        public static string SanitizePostcode(string postcode)
        {
            return Regex.Replace(postcode, @"\s+", string.Empty).ToLower();
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
    }
}
