using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class EPSG_4547 : CGCS_2000
    {
        public EPSG_4547()
        {
            Name = "CGCS2000 / 3-degree Gauss-Kruger CM 108E";
            Type = CoordinateType.ProjectedCoordinateSystem;
            EPSG = "4547";
            CentralLongitude = 114f;
            LatitudeOrigin = 0f;
            LongitudeOrigin = 106.5f;
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
