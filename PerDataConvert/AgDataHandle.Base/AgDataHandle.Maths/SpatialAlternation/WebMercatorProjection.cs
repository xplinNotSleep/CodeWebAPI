using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    /// <summary>
    /// cesium的WebMercatorProjection的C#实现
    /// </summary>
    public class WebMercatorProjection
    {
        private Ellipsoid m_ellipsoid;
        private double m_semimajorAxis;
        private double m_oneOverSemimajorAxis;
        private readonly double PI_OVER_TWO = Math.PI / 2.0;
        public double MaximumLatitude
        {
            get { return MercatorAngleToGeodeticLatitude(Math.PI); }
        }
        public WebMercatorProjection(Ellipsoid ellipsoid)
        {
            this.m_ellipsoid = ellipsoid == null ? Ellipsoid.Wgs84 : ellipsoid;
            this.m_semimajorAxis = this.m_ellipsoid.MaximumRadius;
            this.m_oneOverSemimajorAxis = 1.0 / this.m_semimajorAxis;
        }
        /// <summary>
        /// 投影
        /// </summary>
        /// <param name="cartographic">弧度制经纬度</param>
        /// <param name="result">投影的墨卡托结果</param>
        /// <returns></returns>
        public Vector3d Project(Vector3d cartographic, Vector3d result)
        {
            if (result == null)
                result = new Vector3d(0, 0, 0);
            double semimajorAxis = m_semimajorAxis;
            double x = cartographic.X * semimajorAxis;
            double y = geodeticLatitudeToMercatorAngle(cartographic.Y) * semimajorAxis;
            double z = cartographic.Z;
            result.X = x;
            result.Y = y;
            result.Z = z;
            return result;
        }

        /// <summary>
        /// 反投影
        /// </summary>
        /// <param name="cartesian">墨卡托坐标</param>
        /// <param name="result">弧度制经纬度</param>
        /// <returns></returns>
        public Vector3d UnProject(Vector3d cartesian, Vector3d result)
        {
            if (result == null)
                result = new Vector3d(0, 0, 0);
            double oneOverEarthSemimajorAxis = m_oneOverSemimajorAxis;
            double longitude = cartesian.X * oneOverEarthSemimajorAxis;
            double latitude = MercatorAngleToGeodeticLatitude(cartesian.Y * oneOverEarthSemimajorAxis);
            double height = cartesian.Z;
            result.X = longitude;
            result.Y = latitude;
            result.Z = height;
            return result;
        }

        /**
        * Converts a Mercator angle, in the range -PI to PI, to a geodetic latitude
        * in the range -PI/2 to PI/2.
        * @param {Number} mercatorAngle The angle to convert.
        * @returns {Number} The geodetic latitude in radians.
        */
        private double MercatorAngleToGeodeticLatitude(double mercatorAngle) 
        {
            return PI_OVER_TWO - 2.0 * Math.Atan(Math.Exp(-mercatorAngle));
        }

        /**
        * Converts a geodetic latitude in radians, in the range -PI/2 to PI/2, to a Mercator
        * angle in the range -PI to PI.
        *
        * @param {Number} latitude The geodetic latitude in radians.
        * @returns {Number} The Mercator angle.
        */
        private double geodeticLatitudeToMercatorAngle(double latitude)
        {
            // Clamp the latitude coordinate to the valid Mercator bounds.
            if (latitude > MaximumLatitude)
            {
                latitude = MaximumLatitude;
            }
            else if (latitude < -MaximumLatitude)
            {
                latitude = -MaximumLatitude;
            }
            var sinLatitude = Math.Sin(latitude);
            return 0.5 * Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude));
        }

    }
}
