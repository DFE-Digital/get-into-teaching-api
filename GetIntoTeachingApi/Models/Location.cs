using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    public class Location
    {
        [Key]
        public string Postcode { get; set; }
        public Point Coordinate { get; set; }

        public static string SanitizePostcode(string postcode)
        {
            return Regex.Replace(postcode, @"\s+", "").ToLower();
        }
    }
}
