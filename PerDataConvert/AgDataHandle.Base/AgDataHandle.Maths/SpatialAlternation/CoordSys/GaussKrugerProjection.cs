using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    /// <summary>
    /// 高斯投影，通过墨卡托反投影，计算经纬度
    /// </summary>
    public class GaussKrugerProjection : CoordinateSystem
    {
        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            Vector3d mer = Coordinate.DegreesToMercator(new Vector3d(CentralLongitude, LatitudeOrigin, 0));
            mer.X -= False_Easting;
            mer.Y += False_Northing;
            mer.X += vector2.X;
            mer.Y += vector2.Y;
            var v = Coordinate.MercatorToDegrees(mer);
            return new GisPosition(v.X, v.Y, v.Z, 0);
        }

        public static Vector3d ToDegrees(Vector3d vector3d, double Project_CenterL = 113.2833333333, int Project_ZoneWide = 3, int Project_ZoneNo = 38, double Project_False_Easting = 500000, double Project_False_Northing = 0)
        {
            double m_X = vector3d.Y;
            double m_Y = vector3d.X;

            /*var Project_CenterL = 113.2833333333; //中央经线114
            var Project_ZoneWide = 3;   //3度带
            var Project_ZoneNo = 38; // 带号
            int Project_False_Easting = 500000;//偏移值（包含带号）
            int Project_False_Northing = 0;*/

            int m_a = 6378137;
            double m_b = 6356752.31414d;
            double m_f = ((double)1 / (double)298.257222101);

            double e = Math.Sqrt(2 * m_f - Math.Pow(m_f, 2)); //第一偏心率
            double e2 = e / Math.Sqrt(1 - e * e);  //第二偏心率
                                                   //弧长参数
            double m_r1 = 1 + 0.75 * Math.Pow(e, 2) + 45.0 / 64 * Math.Pow(e, 4) + 175.0 / 256 * Math.Pow(e, 6) + 11025.0 / 16384 * Math.Pow(e, 8) + 43659.0 / 65536 * Math.Pow(e, 10) + 693693.0 / 1048576 * Math.Pow(e, 12);

            double m_r2 = 3.0 / 8 * Math.Pow(e, 2) + 15.0 / 32 * Math.Pow(e, 4) + 525.0 / 1024 * Math.Pow(e, 6) + 2205.0 / 4096 * Math.Pow(e, 8) + 72765.0 / 131072 * Math.Pow(e, 10) + 297297.0 / 524288 * Math.Pow(e, 12);

            double m_r3 = 15.0 / 256 * Math.Pow(e, 4) + 105.0 / 1024 * Math.Pow(e, 6) + 2205.0 / 16384 * Math.Pow(e, 8) + 10395.0 / 65536 * Math.Pow(e, 10) + 1486485.0 / 8388608 * Math.Pow(e, 12);

            double m_r4 = 35.0 / 3072 * Math.Pow(e, 6) + 105.0 / 4096 * Math.Pow(e, 8) + 10395.0 / 262144 * Math.Pow(e, 10) + 55055.0 / 1048576 * Math.Pow(e, 12);

            double m_r5 = 315.0 / 131072 * Math.Pow(e, 8) + 3465.0 / 524288 * Math.Pow(e, 10) + 99099.0 / 8388608 * Math.Pow(e, 12);

            double m_r6 = 693.0 / 1310720 * Math.Pow(e, 10) + 9009.0 / 5242880 * Math.Pow(e, 12);

            double m_r7 = 1001.0 / 8388608 * Math.Pow(e, 12);

            double A1 = m_a * (1 - Math.Pow(e, 2)) * m_r1 * Math.PI / 180;                      //乘弧长参数1
            double A2 = m_a * (1 - Math.Pow(e, 2)) * m_r2;                                      //乘弧长参数2
            double A3 = m_a * (1 - Math.Pow(e, 2)) * m_r3;                                      //乘弧长参数3
            double A4 = m_a * (1 - Math.Pow(e, 2)) * m_r4;                                      //乘弧长参数4                    

            double B0 = (double)m_X / (double)A1;
            double preB0 = 0d;
            /**************************************
             * 底点纬度迭代算法
             * 求B
             * **************************************/
            do
            {
                preB0 =B0;
                B0 = B0 * Math.PI / 180;
                B0 = (m_X - (-A2 * Math.Sin(2 * B0) + A3 * Math.Sin(4 * B0) - A4 * Math.Sin(6 * B0))) / A1;
                if (Math.Abs(B0 - preB0) < 0.0000001) break;

            } while (true);


            B0 = B0 * Math.PI / 180;

            double n2 = Math.Pow(e2, 2) * Math.Pow(Math.Cos(B0), 2);
            double t = Math.Tan(B0);

            double W = Math.Sqrt(1 - Math.Pow(e * Math.Sin(B0), 2));                    //第一辅助参数
            double M = m_a * (1 - Math.Pow(e, 2)) / Math.Pow(W, 3);                     //子午圈曲率半径
            double N = m_a / W;                                                         //卯酉圈曲率半径  

            m_Y = m_Y % 1000000 - Project_False_Easting % 1000000;                              //减去带号--减去偏移值
            double ly = m_Y / N;

            int m_ZoneWide = Project_ZoneWide!=0 ? Project_ZoneWide : 3;
            int m_ZoneNo = Project_ZoneNo!=0 ? Project_ZoneNo : ((int)(m_Y / m_ZoneWide) + 1);
            double m_CenterL = Project_CenterL!=0 ? Project_CenterL : (m_ZoneNo * m_ZoneWide - m_ZoneWide / 2);

            double centerL0 = LatLonHelper.Degree2rad(m_CenterL);                //度转换弧度

            double tempL = centerL0 + (ly - (1 + 2 * t * t + n2) * Math.Pow(ly, 3) / 6 + (5 + 28 * t * t + 24 * Math.Pow(t, 4) + 6 * n2 + 8 * n2 * Math.Pow(t, 2)) * Math.Pow(ly, 5) / 120) / Math.Cos(B0);
            //以百分制形式返回经度
            double L = Math.Round(LatLonHelper.Rad2degree(tempL) * 100000000000) / 100000000000;

            double tempB = B0 - t / (2 * M) * m_Y * ly * (1 - (5 + 3 * t * t + n2 - 9 * n2 * Math.Pow(t, 2)) * Math.Pow(ly, 2) / 12 + (61 + 90 * t * t + 45 * Math.Pow(t, 4)) * Math.Pow(ly, 4) / 360);
            //以百分制形式返回纬度
            double B = Math.Round(LatLonHelper.Rad2degree(tempB) * 100000000000) / 100000000000;
            return new Vector3d(B,L,vector3d.Z);
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            var L0 = this.CentralLongitude;
            var a = this.Semimajor_Axis;//6378137.0; //长半轴 m
            var b = this.Semiminmor_Axis;//6356752.31414; //短半轴 m
            var e = Math.Sqrt(a * a - b * b) / a;// 0.0818191910428; //第一偏心率 Math.sqrt(5)

            var B = gisPosition.Latitude;
            var L = gisPosition.Longitude;


            var eC = 0.0820944379496957;
            // 开始计算
            var radB = B * 3.141592653589793 / 180;
            var radL = L * 3.141592653589793 / 180;
            var deltaL = (L - L0) * 3.141592653589793 / 180;
            var N = a * a / b / Math.Sqrt(1 + eC * eC * Math.Cos(radB) * Math.Cos(radB));
            var C1 = 1.0 + 3.0 / 4 * e * e + 45.0 / 64 * Math.Pow(e, 4) + 175.0 / 256 * Math.Pow(e, 6)
                + 11025.0 / 16384 * Math.Pow(e, 8);
            var C2 = 3.0 / 4 * e * e + 15.0 / 16 * Math.Pow(e, 4) + 525.0 / 512 * Math.Pow(e, 6)
                + 2205.0 / 2048 * Math.Pow(e, 8);
            var C3 = 15.0 / 64 * Math.Pow(e, 4) + 105.0 / 256 * Math.Pow(e, 6) + 2205.0 / 4096 * Math.Pow(e, 8);
            var C4 = 35.0 / 512 * Math.Pow(e, 6) + 315.0 / 2048 * Math.Pow(e, 8);
            var C5 = 315.0 / 131072 * Math.Pow(e, 8);
            var t = Math.Tan(radB);
            var eta = eC * Math.Cos(radB);
            var X = a * (1 - e * e) * (C1 * radB - C2 * Math.Sin(2 * radB) / 2 + C3 * Math.Sin(4 * radB) / 4
                - C4 * Math.Sin(6 * radB) / 6 + C5 * Math.Sin(8 * radB));

            var x = X + N * Math.Sin(radB) * Math.Cos(radB) * Math.Pow(deltaL, 2)
                * (1 + Math.Pow(deltaL * Math.Cos(radB), 2) * (5 - t * t + 9 * eta * eta + 4 * Math.Pow(eta, 4)) / 12
                    + Math.Pow(deltaL * Math.Cos(radB), 4) * (61 - 58 * t * t + Math.Pow(t, 4)) / 360)
                / 2;
            var y = N * deltaL * Math.Cos(radB)
                * (1 + Math.Pow(deltaL * Math.Cos(radB), 2) * (1 - t * t + eta * eta) / 6
                    + Math.Pow(deltaL * Math.Cos(radB), 4)
                    * (5 - 18 * t * t + Math.Pow(t, 4) - 14 * eta * eta - 58 * eta * eta * t * t) / 120)
                + False_Easting;
            return new Vector3d(x, y, (double)gisPosition.TransHeight);
        }
    }
}
