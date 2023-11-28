using System;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class EPSG_32650 : CoordinateSystem
    {
        public EPSG_32650()
        {
            Name = "WGS 84 / UTM zone 50N";
            Type = CoordinateType.ProjectedCoordinateSystem;
            EPSG = "32650";
            CentralLongitude = 114.0f;
            LatitudeOrigin = 0f;
            LongitudeOrigin = 117.0f;
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            throw new NotImplementedException();
        }

        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            Vector3d mer = Coordinate.DegreesToMercator(new Vector3d(CentralLongitude, LatitudeOrigin, 0));
            mer.X += vector2.X;
            mer.Y += vector2.Y;
            var v = Coordinate.MercatorToDegrees(mer);
            return new GisPosition(v.X, v.Y, v.Z, 0);
        }
    }

}
