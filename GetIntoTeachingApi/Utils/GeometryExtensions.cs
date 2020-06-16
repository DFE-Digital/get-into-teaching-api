using System.Collections.Generic;
using GetIntoTeachingApi.Database;
using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace GetIntoTeachingApi.Utils
{
    internal static class GeometryExtensions
    {
        private static readonly CoordinateSystemServices _coordinateSystemServices = new CoordinateSystemServices(
            new CoordinateSystemFactory(),
            new CoordinateTransformationFactory(),
            new Dictionary<int, string>
            {
                [DbConfiguration.UkSrid] = @"
                    PROJCS[""OSGB 1936 / British National Grid"",
                    GEOGCS[""OSGB 1936"",
                        DATUM[""OSGB_1936"",
                            SPHEROID[""Airy 1830"",6377563.396,299.3249646,
                                AUTHORITY[""EPSG"",""7001""]],
                            TOWGS84[446.448,-125.157,542.06,0.15,0.247,0.842,-20.489],
                            AUTHORITY[""EPSG"",""6277""]],
                        PRIMEM[""Greenwich"",0,
                            AUTHORITY[""EPSG"",""8901""]],
                        UNIT[""degree"",0.0174532925199433,
                            AUTHORITY[""EPSG"",""9122""]],
                        AUTHORITY[""EPSG"",""4277""]],
                    PROJECTION[""Transverse_Mercator""],
                    PARAMETER[""latitude_of_origin"",49],
                    PARAMETER[""central_meridian"",-2],
                    PARAMETER[""scale_factor"",0.9996012717],
                    PARAMETER[""false_easting"",400000],
                    PARAMETER[""false_northing"",-100000],
                    UNIT[""metre"",1,
                        AUTHORITY[""EPSG"",""9001""]],
                    AXIS[""Easting"",EAST],
                    AXIS[""Northing"",NORTH],
                    AUTHORITY[""EPSG"",""27700""]]"
            });

        public static Geometry ProjectTo(this Geometry geometry, int srid)
        {
            var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);
            var result = geometry.Copy();

            result.Apply(new MathTransformFilter((MathTransform)transformation.MathTransform));

            return result;
        }

        private class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _transform;

            public MathTransformFilter(MathTransform transform) => _transform = transform;

            public bool Done => false;
            public bool GeometryChanged => true;

            public void Filter(CoordinateSequence seq, int i)
            {
                var result = _transform.Transform(new[] { seq.GetOrdinate(i, Ordinate.X), seq.GetOrdinate(i, Ordinate.Y) });
                seq.SetOrdinate(i, Ordinate.X, result[0]);
                seq.SetOrdinate(i, Ordinate.Y, result[1]);
            }
        }
    }
}
