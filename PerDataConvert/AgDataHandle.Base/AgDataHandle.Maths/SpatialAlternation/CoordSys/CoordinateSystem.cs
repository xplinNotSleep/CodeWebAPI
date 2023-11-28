namespace AgDataHandle.Maths.SpatialAlternation
{
    /// <summary>
    /// 坐标系抽象类
    /// </summary>
    public abstract class CoordinateSystem
    {

        public string AliasName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 坐标类型
        /// </summary>
        public CoordinateType Type { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string EPSG { get; set; }
        /// <summary>
        /// 中央经线
        /// </summary>
        public double CentralLongitude { get; set; }
        /// <summary>
        /// 纬度原点
        /// </summary>
        public double LatitudeOrigin { get; set; }
        /// <summary>
        /// 经度原点
        /// </summary>
        public double LongitudeOrigin { get; set; }
        /// <summary>
        /// 东向偏移值
        /// </summary>
        public double False_Easting { get; set; } = 0;
        /// <summary>
        /// 北向偏移值
        /// </summary>
        public double False_Northing { get; set; } = 0;
        /// <summary>
        /// 比例因子
        /// </summary>
        public double Scale_Factor { get; set; } = 1;
        /// <summary>=1
        /// 长度单位
        /// </summary>
        public double Linear_Unit { get; set; }
        /// <summary>
        /// 长半轴
        /// </summary>
        public double Semimajor_Axis { get; set; }

        /// <summary>
        /// 短半轴
        /// </summary>
        public double Semiminmor_Axis { get; set; }

        /// <summary>
        /// 反扁率
        /// </summary>
        public double Inverse_Flattening { get; set; }

        /// <summary>
        /// 基准面
        /// </summary>
        public string Datum { get; set; }

        /// <summary>
        /// 椭球
        /// </summary>
        public string Spheroid { get; set; }

        /// <summary>
        /// 反投影,将投影坐标转换为
        /// </summary>
        /// <param name="vector2">平面坐标</param>
        /// <returns>球面经纬度坐标</returns>
        public Vector3d UnProject(Vector2d vector2)
        {
            var gisPos = UnProjectToGisPos(vector2);
            return new Vector3d(gisPos.Latitude, gisPos.Longitude, (double)gisPos.TransHeight);
        }

        public abstract GisPosition UnProjectToGisPos(Vector2d vector2);

        /// <summary>
        /// 将经纬度转为投影坐标系
        /// </summary>
        /// <param name="B"></param>
        /// <param name="L"></param>
        /// <returns></returns>
        public  Vector3d Project(double B, double L)
        {
            return ProjectFromGisPos(new GisPosition() { Latitude = (double)B, Longitude = (double)L });
        }
        public abstract Vector3d ProjectFromGisPos(GisPosition gisPosition);

        
    }

    /// <summary>
    /// 坐标系类型
    /// </summary>
    public enum CoordinateSystemType
    {
        WGS84,
        Cartesian,
        GZ_2000,
        SZ_2000,
        ZH_2000,
        CGCS_2000,
        EPSG_32650,
        EPSG_4545,

    }

    public enum CoordinateTransformType
    {
        GaussKrugerToWGS84,
        WGS84ToGaussKruger,
        WebMercatorToWGS84,
        WGS84ToWebMercator,
        GZ2000XYZToCGCS20000BL,
        CGCS20000BLToGZ2000XYZ,
        CGCS2000ToCGCS2000_3_degree_Gauss_Kruger_CM_114E,
        CGCS2000SZToWGS84,

    }

    public enum CoordinateType
    {
        ProjectedCoordinateSystem,
        GeographicCoordinateSystem,
        Cartesian
    }
}
