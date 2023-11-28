using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class CGCS_2000 : CoordinateSystem
    {
        public CGCS_2000()
        {
            Name = "CGCS_2000";
            EPSG = "CGCS_2000";
            Type = CoordinateType.ProjectedCoordinateSystem;

            CentralLongitude = 113.18f;

            //下面两个值不使用
            LatitudeOrigin = double.NaN;
            LongitudeOrigin = double.NaN;

            False_Easting = 500000f;
            False_Northing = 0f;
            Linear_Unit = 0.0174532925199433;

            Semimajor_Axis = 6378137.0;
            Semiminmor_Axis= 6356752.31414;
            Inverse_Flattening = 298.257222101;

            Datum = "China_2000";
            Spheroid = "CGCS2000,6378137,298.257222101";
        }
        /// <summary>
        /// 传入CGCS2000坐标的xy，返回经纬度
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public override GisPosition UnProjectToGisPos(Vector2d vector2)
        {
            var x = vector2.X;
            var y = vector2.Y;

            double lat, lon;
            y -= 500000;
            double[] result = new double[2];
            double iPI = 0.0174532925199433;//pi/180
            double a = 6378137.0; //长半轴 m
            double b = 6356752.31414; //短半轴 m
            double f = 1 / 298.257222101;//扁率 a-b/a
            double e = 0.0818191910428; //第一偏心率 Math.sqrt(5)
            double ee = Math.Sqrt(a * a - b * b) / b; //第二偏心率
            double bf = 0; //底点纬度
            double a0 = 1 + (3 * e * e / 4) + (45 * e * e * e * e / 64) + (175 * e * e * e * e * e * e / 256) + (11025 * e * e * e * e * e * e * e * e / 16384) + (43659 * e * e * e * e * e * e * e * e * e * e / 65536);
            double b0 = x / (a * (1 - e * e) * a0);
            double c1 = 3 * e * e / 8 + 3 * e * e * e * e / 16 + 213 * e * e * e * e * e * e / 2048 + 255 * e * e * e * e * e * e * e * e / 4096;
            double c2 = 21 * e * e * e * e / 256 + 21 * e * e * e * e * e * e / 256 + 533 * e * e * e * e * e * e * e * e / 8192;
            double c3 = 151 * e * e * e * e * e * e * e * e / 6144 + 151 * e * e * e * e * e * e * e * e / 4096;
            double c4 = 1097 * e * e * e * e * e * e * e * e / 131072;
            bf = b0 + c1 * Math.Sin(2 * b0) + c2 * Math.Sin(4 * b0) + c3 * Math.Sin(6 * b0) + c4 * Math.Sin(8 * b0); // bf =b0+c1*sin2b0 + c2*sin4b0 + c3*sin6b0 +c4*sin8b0 +...
            double tf = Math.Tan(bf);
            double n2 = ee * ee * Math.Cos(bf) * Math.Cos(bf); //第二偏心率平方成bf余弦平方
            double c = a * a / b;
            double v = Math.Sqrt(1 + ee * ee * Math.Cos(bf) * Math.Cos(bf));
            double mf = c / (v * v * v); //子午圈半径
            double nf = c / v;//卯酉圈半径

            //纬度计算
            lat = bf - (tf / (2 * mf) * y) * (y / nf) * (1 - 1 / 12 * (5 + 3 * tf * tf + n2 - 9 * n2 * tf * tf) * (y * y / (nf * nf)) + 1 / 360 * (61 + 90 * tf * tf + 45 * tf * tf * tf * tf) * (y * y * y * y / (nf * nf * nf * nf)));
            //经度偏差
            lon = 1 / (nf * Math.Cos(bf)) * y - (1 / (6 * nf * nf * nf * Math.Cos(bf))) * (1 + 2 * tf * tf + n2) * y * y * y + (1 / (120 * nf * nf * nf * nf * nf * Math.Cos(bf))) * (5 + 28 * tf * tf + 24 * tf * tf * tf * tf) * y * y * y * y * y;
            result[0] = retain6(lat / iPI);
            result[1] = retain6(CentralLongitude + lon / iPI);
            //System.out.println(result[1]+","+result[0]);
            return new GisPosition(result[1], result[0], 0, 0);
        }

        private double retain6(double num)
        {
            return Math.Round(num, 6);

        }

        public Vector3d ToGZ2000XYZ(Vector3d vector3d) 
        {
            Vector3d tempVector3 = new Vector3d(vector3d.Y-2329620, vector3d.X - 460020, vector3d.Z);
            var m_Scale = 0.999997756396785d;
            var m_RotationAngle = 0.0044099861111111d * Math.PI / 180;
            var m_DX = -199995.893159972d;
            var m_DY = -16.6491077492992d;

            var A = m_Scale * Math.Cos(m_RotationAngle);
            var B = m_Scale * Math.Sin(m_RotationAngle);

            var X = A * tempVector3.X - B * tempVector3.Y + m_DX;
            var Y = A * tempVector3.Y + B * tempVector3.X + m_DY;
            return  new Vector3d(Y,X+200000,tempVector3.Z);
        }

        public override Vector3d ProjectFromGisPos(GisPosition gisPosition)
        {
            var L0 = this.CentralLongitude;
            var a = this.Semimajor_Axis;//6378137.0; //长半轴 m
            var b = this.Semiminmor_Axis;//6356752.31414; //短半轴 m
            var e = Math.Sqrt(a * a - b * b) / a;// 0.0818191910428; //第一偏心率 Math.sqrt(5)

            var B=gisPosition.Latitude;
            var L=gisPosition.Longitude;
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
            return new Vector3d(y, x, gisPosition.TransHeight == null ? 0 : (double)gisPosition.TransHeight);
        }
    }
}
