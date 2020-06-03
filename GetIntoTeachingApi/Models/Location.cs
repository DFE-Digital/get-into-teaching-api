using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    public class Location
    {
        [Key]
        public string Postcode { get; set; }
        [Column(TypeName = "geography")]
        public Point Coordinate { get; set; }

        public static string SanitizePostcode(string postcode)
        {
            return Regex.Replace(postcode, @"\s+", "").ToLower();
        }
    }
}
