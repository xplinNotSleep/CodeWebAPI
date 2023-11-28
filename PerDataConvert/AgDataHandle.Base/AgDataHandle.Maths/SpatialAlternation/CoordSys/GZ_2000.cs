using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class GZ_2000 : CGCS_2000
    {
        public GZ_2000()
        {
            AliasName = "GZ_2000";
            Name = "DAYUANCUN-CGCS2000 / 3-degree Gauss-Kruger CM 113.17E";
            Type = CoordinateType.ProjectedCoordinateSystem;
            EPSG = "";
            CentralLongitude = 113.2833333333f;
            LatitudeOrigin = 0f;
            LongitudeOrigin = 106.5f;
        }

        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            Vector3d v = ToCGCS_2000XYZ(new Vector3d(vector2.Y, vector2.X, 0));
            base.CentralLongitude = CentralLongitude;
            var result = base.UnProjectToGisPos(new Vector2d(v.Y, v.X));
            return result;
        }

        public Vector3d ToCGCS_2000XYZ(Vector3d vector3d)
        {
            var tempP = new Vector3d(vector3d.Y - 200000, vector3d.X, vector3d.Z);

            var fourTP_scale = 0.999997756396785d;//缩放比例
            var fourTP_angle = 0.0044099861111111d;//旋转角度
            var fourTP_DX = -199995.893159972d;//北向
            var fourTP_DY = -16.6491077492992d;//东西向
            var rotationAngle = fourTP_angle * Math.PI / 180;

            var A = fourTP_scale * Math.Cos(rotationAngle);
            var B = fourTP_scale * Math.Sin(rotationAngle);


            var X = (A * tempP.X + B * tempP.Y - A * fourTP_DX - B * fourTP_DY) / (Math.Pow(A, 2) + Math.Pow(B, 2));
            var Y = (A * tempP.Y - B * tempP.X + B * fourTP_DX - A * fourTP_DY) / (Math.Pow(A, 2) + Math.Pow(B, 2));

            return new Vector3d(Y + 460020, X + 2329620, vector3d.Z);
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            base.CentralLongitude = CentralLongitude;
            var v = base.ProjectFromGisPos(gisPosition);
            var result = base.ToGZ2000XYZ(new Vector3d(v.X, v.Y, 0));
            return result;
        }
    }

}
