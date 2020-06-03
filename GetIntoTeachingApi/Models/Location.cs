using System.ComponentModel.DataAnnotations;
using GeoCoordinatePortable;

namespace GetIntoTeachingApi.Models
{
    public class Location
    {
        [Key]
        public string Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public GeoCoordinate Coordinate => IsNonGeographic() ? null : 
            new GeoCoordinate((double) Latitude, (double) Longitude);

        public bool IsNonGeographic() => Latitude == null || Longitude == null;
    }
}
