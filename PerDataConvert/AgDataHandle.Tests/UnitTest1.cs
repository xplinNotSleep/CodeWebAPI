using AgDataHandle.Maths;
using AgDataHandle.Maths.SpatialAlternation;
using AgDataHandle.SdbTo3dtiles;
using DotSpatial.Projections;
namespace AgDataHandle.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataConvert.ToConvert(@"E:\钉钉保存\管综 - 1124(1).SDB", @"E:\钉钉保存\管综 - 1124(1)", "{\"ModelUnitChange\":0.001}");
        }

        [TestMethod]
        public void TestMethod10()
        {
            DataConvert.ToConvert(@"E:\钉钉保存\施工图 - 1124-1.SDB", @"E:\钉钉保存\施工图 - 1124-1", "{\"ModelUnitChange\":0.001}");
        }

        [TestMethod]
        public void TestMethod2()
        {
            //   GZ_2000 gZ_2000 = new GZ_2000();
            //var ass=   gZ_2000.UnProjectToGisPos(new Vector2d( 3446329.501000002, 669611.1389999996));
            var coord = new CGCS_2000();
            coord.CentralLongitude = 121.464;
            //var radius = coord.UnProject(new Vector2d(3446329.501000002, 669611.138999999));
            var radius = coord.UnProject(new Vector2d(3443587.322, 670604.5753));
            var wgs84 = new Vector3d(radius[0] * 180 / Math.PI, radius[1] * 180 / Math.PI, radius[2]);

        }

        [TestMethod]
        public void TestMethod3()
        {
            Vector3d v = new Vector3d(29740.2393 , -12314.1896,0);
            Transform4Para(ref v, 639814.522384301, 3458085.9660171, 0.0111450885369286, 1.00026357281604, 0);
            var coord = new CGCS_2000();
            coord.CentralLongitude = 120;
            //var radius = coord.UnProject(new Vector2d(3446329.501000002, 669611.138999999));
            var radius = coord.UnProject(new Vector2d(v.Y, v.X));
            var wgs84 = new Vector3d(radius[0] * 180 / Math.PI, radius[1] * 180 / Math.PI, radius[2]);
        }

        void Transform4Para(ref Vector3d v, double dx, double dy, double A, double K, double Dh)
        {
            var X1 = dx;
            var Y1 = dy;

            var cosAngle = Math.Cos(A);
            var sinAngle = Math.Sin(A);

            X1 += K * (cosAngle * v.X - sinAngle * v.Y);
            Y1 += K * (sinAngle * v.X + cosAngle * v.Y);

            v.X = X1;
            v.Y = Y1;
            // 固定改正差
            v.Z += Dh;
        }

        [TestMethod]
        public void TestJWD()
        {
            var projection = ProjectionInfo.FromEsriString("PROJCS[\"SHAG2000\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",-3457147.81],PARAMETER[\"Central_Meridian\",121.4644444],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]");
            var radius = new double[] { 29720.28971, -12135.352141 };
            Reproject.ReprojectPoints(radius, null, projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);
        }

        //[TestMethod]
        //public void TestLBF()
        //{
        //    // 实例化计算参数
        //    MyPoint p = new MyPoint();

        //    p.SetLBH(113.256, 31.565, 5.216);

        //    // 经纬度转空间坐标
        //    p.BLH2XYZ();

        //    // 实例化七参数
        //    Datum7Paras datum7Paras = new Datum7Paras(
        //        489.2994563566, 141.1525159753, 15.74421120568,
        //        -0.164423, 4.141573, -4.808299,
        //        -6.56482989958);

        //    p.SevenParamTrans(datum7Paras);

        //    // 空间坐标转回经纬度
        //    p.XYZ2BLH(EllipsoidType.WGS84);

        //    // 高斯投影 经纬度转平面坐标
        //    // 实例化投影参数类
        //    ProjectionSetting projectionSetting = new ProjectionSetting(120, 500000);
        //    p.GaussProjection(EllipsoidType.WGS84, projectionSetting);

        //    Trans4Paras transformPara = new(6456.15957352521, -134618.390707439, 0.011104964500129, 1.00002537583871, 5.788);

        //    p.Transform4Para(transformPara);
        //}
    }
}