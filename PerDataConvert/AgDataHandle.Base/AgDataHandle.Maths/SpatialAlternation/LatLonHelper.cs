using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class LatLonHelper
    {
        //地球半径，单位米（WGS84参考椭球体的长半轴长度）
        private const double EARTH_RADIUS = 6378137.0;
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Degree2rad(lat1);
            double radLng1 = Degree2rad(lng1);
            double radLat2 = Degree2rad(lat2);
            double radLng2 = Degree2rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            //半正矢量公式
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        //弧度和角度的换算关系如下：
        //1度=π/180弧度
        //1弧度=180/π度
        //度=》弧度
        public static double Degree2rad(double val)
        {
            return val * Math.PI / 180.0;
        }
        public static double Rad2degree(double val)
        {
            return val * 180 / Math.PI;
        }

        //纬度=》米
        public static double LatitudeToMeters(double diff)
        {
            return diff / 0.000000157891;
        }

        //经度=》米
        public static double LongitudeToMeters(double diff, double lati)
        {
            return diff / 0.000000156785 * Math.Abs(Math.Cos(lati));
        }

        public static double MetersToLatitude(double m)
        {
            return m * 0.000000157891;
        }

        public static double MetersToLongitude(double m, double lati)
        {
            return m * 0.000000156785 / Math.Cos(lati);
        }
    }
}
