using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class WGS84 : CoordinateSystem
    {
        public WGS84()
        {
            AliasName = "WGS84";
            Name = "WGS84";
            EPSG = "4326";
            Type = CoordinateType.GeographicCoordinateSystem;
        }

        public static Vector3d ToCartesian3(Vector3d vector3)
        {
            var cartesian3 = Coordinate.DegreesToCartesian3(vector3);
            return cartesian3;
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            throw new NotImplementedException();
        }

        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            throw new NotImplementedException();
        }
    }
}
