using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    ///// <summary>
    ///// 不同坐标系的转换
    ///// </summary>
    //public class CoordinateFactory
    //{
    //    public static ICoordinate CreateCoordinate(Spheroid s)
    //    {
    //        ICoordinate coordinate;
    //        switch (s)
    //        {
    //            case Spheroid.Beijing54:
    //                coordinate = new Beijing54();
    //                break;
    //            case Spheroid.WGS84:
    //                coordinate = new WGS84();
    //                break;
    //            case Spheroid.Xian80:
    //                coordinate = new Xian80();
    //                break;
    //            default:
    //                coordinate = null;
    //                break;
    //        }
    //        return coordinate;
    //    }

    //    public static ICoordinate CreateCoordinate()
    //    {
    //        return new WGS84();
    //    }
    //}
    //public class Beijing54 : GaussPrjBase
    //{

    //    public Beijing54()
    //    {
    //        _a = 6378245.0;
    //        _f = 1.0 / 298.3;
    //    }
    //}
    ///// <summary>
    ///// WGS84参考椭球 参数(全球GPS监测网采用此参考椭球)
    ///// </summary>
    //public class WGS84 : GaussPrjBase
    //{
    //    public WGS84()
    //    {
    //        _a = 6378137.0;
    //        _f = 1.0 / 298.257223563;

    //    }
    //}
    ///// <summary>
    ///// IUGG1975参考椭球 参数(北京54坐标系采用此参考椭球)
    ///// </summary>
    //public class Xian80 : GaussPrjBase
    //{
    //    public Xian80()
    //    {
    //        _a = 6378140.0;
    //        _f = 1.0 / 298.257;
    //    }
    //}
    //public abstract class GaussPrjBase : ICoordinate
    //{
    //    protected double _a;
    //    protected double _f;
    //    private int _zoneWide = 3;
    //    protected readonly double PI = 3.14159265353846;

    //    #region ICoordinate Members
    //    public int ZoneWide
    //    {
    //        get { return _zoneWide; }
    //        set { _zoneWide = value; }
    //    }

    //    public void DD2DMS(double DecimalDegree, out int Degree, out int Minute, out double Second)
    //    {
    //        Degree = (int)DecimalDegree;
    //        Minute = (int)((DecimalDegree - Degree) * 60.0);
    //        Second = Math.Round((DecimalDegree * 60 - Degree * 60 - Minute) * 60 * 100) / 100.0;

    //    }

    //    public double DistanceOfTwoPoints(double lng1, double lat1, double lng2, double lat2, Spheroid gs)
    //    {
    //        double radLat1 = Rad(lat1);
    //        double radLat2 = Rad(lat2);
    //        double a = radLat1 - radLat2;
    //        double b = Rad(lng1) - Rad(lng2);
    //        double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
    //        Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
    //        s = s * (gs == Spheroid.WGS84 ? 6378137.0 : (gs == Spheroid.Xian80 ? 6378140.0 : 6378245.0));
    //        s = Math.Round(s * 10000) / 10000;
    //        return s;
    //    }

    //    private double Rad(double d)
    //    {
    //        return d * Math.PI / 180.0;
    //    }
    //    public void DMS2DD(int Degree, int Minute, double Second, out double DecimalDegree)
    //    {
    //        DecimalDegree = Degree + Minute / 60.0 + Second / 60.0 / 60.0;
    //    }


    //    public void GaussPrjCalculate(double longitude, double latitude, out double X, out double Y)
    //    {
    //        int ProjNo = 0;
    //        double longitude1, latitude1, longitude0, latitude0, X0, Y0, xval, yval;
    //        double e2, ee, NN, T, C, A, M, iPI;
    //        iPI = 0.0174532925199433; //3.1415926535898/180.0; 
    //        //ZoneWide = 6; //6度带宽 
    //        ProjNo = (int)(longitude / ZoneWide);
    //        longitude0 = ZoneWide == 3 ? ProjNo * 3 : ProjNo * 6 - 3;
    //        longitude0 = longitude0 * iPI;
    //        latitude0 = 0;
    //        longitude1 = longitude * iPI; //经度转换为弧度
    //        latitude1 = latitude * iPI; //纬度转换为弧度
    //        e2 = 2 * _f - _f * _f;
    //        ee = e2 * (1.0 - e2);
    //        NN = _a / Math.Sqrt(1.0 - e2 * Math.Sin(latitude1) * Math.Sin(latitude1));
    //        T = Math.Tan(latitude1) * Math.Tan(latitude1);
    //        C = ee * Math.Cos(latitude1) * Math.Cos(latitude1);
    //        A = (longitude1 - longitude0) * Math.Cos(latitude1);
    //        M = _a * ((1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256) * latitude1 - (3 * e2 / 8 + 3 * e2 * e2 / 32 + 45 * e2 * e2 * e2 / 1024) * Math.Sin(2 * latitude1) + (15 * e2 * e2 / 256 + 45 * e2 * e2 * e2 / 1024) * Math.Sin(4 * latitude1) - (35 * e2 * e2 * e2 / 3072) * Math.Sin(6 * latitude1));
    //        xval = NN * (A + (1 - T + C) * A * A * A / 6 + (5 - 18 * T + T * T + 72 * C - 58 * ee) * A * A * A * A * A / 120);
    //        yval = M + NN * Math.Tan(latitude1) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24 + (61 - 58 * T + T * T + 600 * C - 330 * ee) * A * A * A * A * A * A / 720);
    //        X0 = 1000000L * (ProjNo + 1) + 500000L;
    //        Y0 = 0;
    //        X = Math.Round((xval + X0) * 10000000000) / 10000000000.0;
    //        Y = Math.Round((yval + Y0) * 10000000000) / 10000000000.0;
    //    }

    //    public void GaussPrjInvCalculate(double X, double Y, out double longitude, out double latitude)
    //    {
    //        int ProjNo;
    //        double longitude1, latitude1, longitude0, latitude0, X0, Y0, xval, yval;
    //        double e1, e2, ee, NN, T, C, M, D, R, u, fai, iPI;
    //        iPI = 0.0174532925199433; //3.1415926535898/180.0; 
    //        ProjNo = (int)(X / 1000000L); //查找带号
    //        //longitude0 = ProjNo * 3;
    //        longitude0 = ZoneWide == 3 ? ProjNo * 3 : ProjNo * 6 - 3;

    //        longitude0 = longitude0 * iPI; //中央经线
    //        X0 = ProjNo * 1000000L + 500000L;
    //        Y0 = 0;
    //        xval = X - X0;
    //        yval = Y - Y0; //带内大地坐标
    //        e2 = 2 * _f - _f * _f;
    //        e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
    //        ee = e2 / (1 - e2);
    //        M = yval;
    //        u = M / (_a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
    //        fai = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * u)
    //        + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
    //        C = ee * Math.Cos(fai) * Math.Cos(fai);
    //        T = Math.Tan(fai) * Math.Tan(fai);
    //        NN = _a / Math.Sqrt(1.0 - e2 * Math.Sin(fai) * Math.Sin(fai));
    //        R = _a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)));
    //        D = xval / NN;
    //        //计算经度(Longitude) 纬度(Latitude)
    //        longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D * D * D * D * D / 120) / Math.Cos(fai);
    //        latitude1 = fai - (NN * Math.Tan(fai) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24 + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);
    //        //转换为度 DD
    //        longitude = Math.Round((longitude1 / iPI) * 10000000000) / 10000000000.0;
    //        latitude = Math.Round((latitude1 / iPI) * 10000000000) / 10000000000.0;
    //    }
    //    #endregion        
    //}
    //public enum Spheroid
    //{
    //    Xian80,
    //    Beijing54,
    //    WGS84,
    //}
    //public interface ICoordinate
    //{
    //    /// <summary>
    //    /// 带宽
    //    /// </summary>
    //    int ZoneWide { get; set; }

    //    /// <summary>
    //    /// 十进制双精度角度转换成度分秒角度格式
    //    /// </summary>
    //    /// <param name="Decimal Degree">度，十进制型双精度</param>
    //    /// <param name="Degree">度，整型</param>
    //    /// <param name="Minute">分，整型</param>
    //    /// <param name="Second">秒，双精度型</param>
    //    void DD2DMS(double DecimalDegree, out int Degree, out int Minute, out double Second);

    //    /// <summary>
    //    /// 求两点之间的距离(根据经纬度)
    //    /// </summary>
    //    /// <param name="lng1">经度1</param>
    //    /// <param name="lat1">纬度1</param>
    //    /// <param name="lng2">经度2</param>
    //    /// <param name="lat2">纬度2</param>
    //    /// <param name="gs">高斯投影中所选用的参考椭球</param>
    //    /// <returns>两点间距离(单位:meters)</returns>
    //    double DistanceOfTwoPoints(double lng1, double lat1, double lng2, double lat2, Spheroid gs);





    //    /// <summary>
    //    /// 求两点之间的距离(大地坐标)
    //    /// </summary>
    //    /// <param name="x1"></param>
    //    /// <param name="y1"></param>
    //    /// <param name="x2"></param>
    //    /// <param name="y2"></param>
    //    /// <returns>单位为meters</returns>
    //    //double DistanceOfTwoPoints(double x1, double y1, double x2, double y2);

    //    /// <summary>
    //    /// 度分秒角度格式转换成十进制度双精度角度格式
    //    /// </summary>
    //    /// <param name="Degree">度，整型</param>
    //    /// <param name="Minute">分，整型</param>
    //    /// <param name="Second">秒，双精度型</param>
    //    /// <param name="Decimal Degree">度，十进制型双精度</param>   
    //    void DMS2DD(int Degree, int Minute, double Second, out double DecimalDegree);

    //    /// <summary>
    //    /// 高期投影正算
    //    /// 由经纬度（单位：Decimal Degree）正算到大地坐标（单位：Metre，含带号）
    //    /// </summary>
    //    /// <param name="longitude">经度</param>
    //    /// <param name="latitude">纬度</param>
    //    void GaussPrjCalculate(double longitude, double latitude, out double X, out double Y);

    //    /// <summary>
    //    /// 高斯投影反算
    //    /// 大地坐标（单位：Metre，含带号）反算到经纬度坐标（单位，Decimal Degree）
    //    /// </summary>
    //    /// <param name="X">大地坐标X值</param>
    //    /// <param name="Y">大地坐标Y值</param>
    //    void GaussPrjInvCalculate(double X, double Y, out double longitude, out double latitude);
    //}
}
