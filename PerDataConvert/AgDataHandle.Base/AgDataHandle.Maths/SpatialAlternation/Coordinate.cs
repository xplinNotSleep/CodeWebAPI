using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    /// <summary>
    /// 坐标转换
    /// </summary>
    public class Coordinate
    {
        private static readonly Vector3 m_wgs84OneOverRadii = new Vector3(1.0f / 6378137.0f, 1.0f / 6378137.0f, 1.0f / 6356752.3142451793f);

        private static readonly Vector3 m_wgs84OneOverRadiiSquared = new Vector3(
          1.0f / (6378137.0f * 6378137.0f),
          1.0f / (6378137.0f * 6378137.0f),
          1.0f / (6356752.3142451793f * 6356752.3142451793f)
        );
        private static readonly float m_wgs84CenterToleranceSquared = 0.1f;

        /// <summary>
        /// 十进制度转wgs84笛卡尔（球心为原点）,高度未验证
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3d DegreesToCartesian3(Vector3d vector3)
        {
            double longitude = vector3.X / 180 * Math.PI;
            double latitude = vector3.Y / 180 * Math.PI;
            Vector3d Cartesian3 = GisUtilDouble.CesiumFromRadians(longitude, latitude, vector3.Z, GisUtilDouble.Wgs84RadiiSquared);
            return Cartesian3;
        }
        /// <summary>
        /// 十进制度转墨卡托
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3d DegreesToMercator(Vector3d vector3)
        {
            Vector3d cartesian3 = DegreesToCartesian3(vector3);
            Vector3d cartographic = CartesianToCartographic(cartesian3);
            return CartographicToMercator(cartographic);
        }
        /// <summary>
        /// 笛卡尔转弧度制经纬度
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public static Vector3d CartesianToCartographic(Vector3d cartesian)
        {
            Ellipsoid ellipsoid = new Ellipsoid(6378137.0, 6378137.0, 6356752.3142451793);
            return ellipsoid.CartesianToCartographic(cartesian);
        }

        /// <summary>
        /// 弧度制经纬度转墨卡托
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public static Vector3d CartographicToMercator(Vector3d cartesian)
        {
            var webMercatorProjection = new WebMercatorProjection(null);
            return webMercatorProjection.Project(cartesian, cartesian);
        }
        /// <summary>
        /// 经纬度弧度转十进制度
        /// </summary>
        /// <param name="cartographic"></param>
        /// <returns></returns>
        public static Vector3d CartographicToDegree(Vector3d cartographic)
        {
            double lng = LatLonHelper.Rad2degree(cartographic.X);
            double lat = LatLonHelper.Rad2degree(cartographic.Y);
            double alt = cartographic.Z;
            return new Vector3d(lng, lat, alt);
        }

        /// <summary>
        /// 墨卡托转弧度制经纬度
        /// </summary>
        /// <param name="mercator"></param>
        /// <returns></returns>
        public static Vector3d MercatorToCartographic(Vector3d mercator)
        {
            var webMercatorProjection = new WebMercatorProjection(null);
            return webMercatorProjection.UnProject(mercator, mercator);
        }
        /// <summary>
        /// 墨卡托转十进制度
        /// </summary>
        /// <param name="mercator"></param>
        /// <returns></returns>
        public static Vector3d MercatorToDegrees(Vector3d mercator)
        {
            Vector3d cartographic = MercatorToCartographic(mercator);
            return CartographicToDegree(cartographic);
        }

        /// <summary>
        /// 经纬度转高斯
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector3d DegreesToGaussKruger(Vector3d degree, double Project_CenterL = 113.2833333333, int Project_ZoneWide = 3, int Project_ZoneNo = 38, double Project_False_Easting = 500000, double Project_False_Northing = 0)
        {
            var tempBL = new Vector3d(degree.X, degree.Y, degree.Z);

            /*var Project_CenterL = 113.2833333333; //中央经线114
           var Project_ZoneWide = 3;   //3度带
           var Project_ZoneNo = 38; // 带号
           var Project_False_Easting = 500000;//偏移值（包含带号）
           var Project_False_Northing = 0;*/

            int m_a = 6378137;
            double m_b = 6356752.31414d;
            double m_f = ((double)1 / (double)298.257222101);

            int m_ZoneWide = Project_ZoneWide != 0 ? Project_ZoneWide : 3;
            int m_ZoneNo = Project_ZoneNo != 0 ? Project_ZoneNo : ((int)(tempBL.Y / m_ZoneWide) + 1);
            double m_CenterL = Project_CenterL != 0 ? Project_CenterL : (m_ZoneNo * m_ZoneWide - m_ZoneWide / 2);

            double e = Math.Sqrt(2 * m_f - Math.Pow(m_f, 2));
            double e2 = e / Math.Sqrt(1 - e * e);

            tempBL.X = LatLonHelper.Degree2rad(tempBL.X);
            tempBL.Y = LatLonHelper.Degree2rad(tempBL.Y);

            double n2 = Math.Pow(e2, 2) * Math.Pow(Math.Cos(tempBL.X), 2);
            double t = Math.Tan(tempBL.X);

            double W = Math.Sqrt(1 - Math.Pow(e * Math.Sin(tempBL.X), 2));
            double M = m_a * (1 - Math.Pow(e, 2)) / Math.Pow(W, 3);                                    //子午圈曲率半径
            double N = m_a / W;


            double dL = tempBL.Y - LatLonHelper.Degree2rad(m_CenterL);                  //经度差(弧度值)
            double l2 = Math.Pow(dL, 2);                                                               //经差平方值           

            //计算子午弧长
            double Rx = m_a * (1 - Math.Pow(e, 2)) * (1.00505250559297 * tempBL.X - 0.00253155620900066 * Math.Sin(2 * tempBL.X) + 2.65690155540381E-06 * Math.Sin(4 * tempBL.X) - 3.47007559905787E-09 * Math.Sin(6 * tempBL.X) + 4.91654216666515E-12 * Math.Sin(8 * tempBL.X) - 7.26313725279022E-15 * Math.Sin(10 * tempBL.X) + 1.07400991193683E-17 * Math.Sin(12 * tempBL.X));

            Rx = Rx = m_a * (1 - Math.Pow(e, 2)) * (1.00505250559297 * tempBL.X - 0.00253155620900066 * Math.Sin(2 * tempBL.X) + 2.65690155540381E-06 * Math.Sin(4 * tempBL.X));

            double X = Rx + N * t * Math.Pow(Math.Cos(tempBL.X), 2) * l2 * (0.5 + (5 - t * t + 9 * n2 + 4 * Math.Pow(n2, 2)) * Math.Pow(Math.Cos(tempBL.X), 2) * l2 / 24 + (61 - 58 * t * t + Math.Pow(t, 4)) / 720 * Math.Pow(Math.Cos(tempBL.X), 4) * Math.Pow(l2, 2));

            X = Rx + N * Math.Sin(tempBL.X) * Math.Cos(tempBL.X) * l2 * (0.5 + (5 - t * t + 9 * n2 + 4 * Math.Pow(n2, 2)) * Math.Pow(Math.Cos(tempBL.X), 2) * l2 / 24 + (61 - 58 * t * t + Math.Pow(t, 4)) / 720 * Math.Pow(Math.Cos(tempBL.X), 4) * Math.Pow(l2, 2));

            double Y = N * Math.Cos(tempBL.X) * dL * (1 + (1 - t * t + n2) * Math.Pow(Math.Cos(tempBL.X), 2) * l2 / 6.0 + (5 - 18 * t * t + Math.Pow(t, 4) + 14 * n2 - 58 * n2 * t * t) * Math.Pow(Math.Cos(tempBL.X), 4) * Math.Pow(l2, 2) / 120.0);

            // Y = Y + 500000 + m_ZoneNo * 1000000;
            Y = Y + Project_False_Easting;

            //return { Y, X };
            return new Vector3d(Y, X, tempBL.Z);
        }

        #region 坐标系转换
        /// <summary>
        /// 平面坐标转球面经纬度
        /// </summary>
        /// <param name="position">平面位置</param>
        /// <param name="coordinateSystemType">坐标系</param>
        /// <returns>球面经纬度</returns>
        public static Vector3d UnProject(Vector2d position, CoordinateSystemType coordinateSystemType)
        {
            Vector3d result = null;
            result = FindCoordinateSystem(coordinateSystemType).UnProject(position);
            return result;
        }

        /// <summary>
        /// 查找坐标转换系统
        /// </summary>
        /// <param name="coordinateSystemType"></param>
        /// <returns></returns>
        public static CoordinateSystem FindCoordinateSystem(CoordinateSystemType coordinateSystemType)
        {
            CoordinateSystem coordinateSystem = null;
           
            switch (coordinateSystemType)
            {
                case CoordinateSystemType.EPSG_32650:
                    coordinateSystem = new EPSG_32650();
                    break;
                case CoordinateSystemType.EPSG_4545:
                    coordinateSystem = new EPSG_4545();
                    break;
                case CoordinateSystemType.GZ_2000:
                    coordinateSystem = new GZ_2000();
                    break;
                case CoordinateSystemType.CGCS_2000:
                    coordinateSystem = new CGCS_2000();
                    break;
                case CoordinateSystemType.SZ_2000:
                    coordinateSystem = new SZ_2000();
                    break;
                case CoordinateSystemType.ZH_2000:
                    coordinateSystem = new ZH_2000();
                    break;
                case CoordinateSystemType.WGS84:
                    coordinateSystem = new WGS84();
                    break;
                case CoordinateSystemType.Cartesian:
                    coordinateSystem = new Cartesian();
                    break;
                default:
                    break;
            }
            return coordinateSystem;
        }

        public static CoordinateSystem FindCoordinateSystem(string coordinateSystemName)
        {
            Type type = typeof(CoordinateSystemType);
            CoordinateSystemType coordinateSystemType=(CoordinateSystemType) Enum.Parse(type, coordinateSystemName);
            return FindCoordinateSystem(coordinateSystemType);
        }
        #endregion

        #region
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="coordinateSystemType"></param>
        /// <returns></returns>
        public static Vector3d CoordinateTransform(double x,double y,double z, CoordinateTransformType coordinateTransformType)
        {
            switch (coordinateTransformType)
            {
                case CoordinateTransformType.GaussKrugerToWGS84:
                    return GaussKrugerProjection.ToDegrees(new Vector3d(x,y,z));
                case CoordinateTransformType.WGS84ToGaussKruger:
                   return DegreesToGaussKruger(new Vector3d(x, y, z));
                case CoordinateTransformType.WebMercatorToWGS84:
                    var value = new WebMercatorProjection(null).UnProject(new Vector3d(x, y, z), null);
                    return new Vector3d(LatLonHelper.Rad2degree(value.X),LatLonHelper.Rad2degree(value.Y),value.Z);
                case CoordinateTransformType.WGS84ToWebMercator:
                   return  DegreesToMercator(new Vector3d(x, y, z));
                case CoordinateTransformType.GZ2000XYZToCGCS20000BL:
                   var point2000= new GZ_2000().ToCGCS_2000XYZ(new Vector3d(x, y, z));
                    return GaussKrugerProjection.ToDegrees(point2000);
                case CoordinateTransformType.CGCS20000BLToGZ2000XYZ:
                    var xyz = DegreesToGaussKruger(new Vector3d(x, y, z));
                    return new CGCS_2000().ToGZ2000XYZ(xyz);
                case CoordinateTransformType.CGCS2000ToCGCS2000_3_degree_Gauss_Kruger_CM_114E:
                    return DegreesToGaussKruger(new Vector3d(x, y, z));
                case CoordinateTransformType.CGCS2000SZToWGS84:
                    return GaussKrugerProjection.ToDegrees(new Vector3d(x, y, z),114.05);
                default:
                    break;
            }
            return null;
        }
        #endregion
    }
}
