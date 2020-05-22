using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using CsvHelper;
using GeoCoordinatePortable;

namespace GetIntoTeachingApi.Services
{
    public class PostcodeService : IPostcodeService
    {
        private readonly IDictionary<string, PostcodeEntry> _postcodes;
        private const double MetersToMiles = 0.000621371;

        public PostcodeService(string fixture = "./Fixtures/ukpostcodes.csv")
        {
            _postcodes = new Dictionary<string, PostcodeEntry>();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                fixture = "./Fixtures/ukpostcodes.dev.csv";

            LoadPostcodesFixture(fixture);
        }

        public bool IsValid(string postcode)
        {
            if (string.IsNullOrEmpty(postcode))
                return false;

            return _postcodes.ContainsKey(Sanitize(postcode));
        }

        public double DistanceBetween(string originPostcode, string destinationPostcode)
        {
            var origin = _postcodes[Sanitize(originPostcode)];
            var destination = _postcodes[Sanitize(destinationPostcode)];

            return origin.Coordinate.GetDistanceTo(destination.Coordinate) * MetersToMiles;
        }

        private void LoadPostcodesFixture(string path)
        {
            _postcodes.Clear();

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var postcode = csv.GetField<string>("postcode");
                var entry = new PostcodeEntry()
                {
                    Latitude = csv.GetField<double?>("latitude"),
                    Longitude = csv.GetField<double?>("longitude")
                };

                if (entry.IsNonGeographic())
                    continue;

                _postcodes.Add(Sanitize(postcode), entry);
            }
        }

        private string Sanitize(string postcode)
        {
            return Regex.Replace(postcode, @"\s+", "").ToLower();
        }
    }

    internal class PostcodeEntry
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public GeoCoordinate Coordinate
        {
            get { return IsNonGeographic() ? null : new GeoCoordinate((double) Latitude, (double) Longitude); }
        }

        public bool IsNonGeographic()
        {
            return Latitude == null || Longitude == null;
        }
    }
}
