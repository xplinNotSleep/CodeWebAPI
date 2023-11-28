using System;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class Cartesian : CoordinateSystem
    {
        public Cartesian() {
            Name = "Cartesian";
            Type = CoordinateType.Cartesian;

        }

        public static Vector3d ToWGS84(Vector3d vector3)
        {
            var degree = Coordinate.CartesianToCartographic(vector3);
            return Coordinate.CartographicToDegree(degree);
        }
        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            throw new NotImplementedException();
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            throw new NotImplementedException();
        }
    }
}
