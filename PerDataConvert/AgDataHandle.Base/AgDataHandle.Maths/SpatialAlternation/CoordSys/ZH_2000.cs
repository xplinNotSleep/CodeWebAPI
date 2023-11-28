namespace AgDataHandle.Maths.SpatialAlternation
{
    public class ZH_2000 : CGCS_2000
    {
        public ZH_2000()
        {
            AliasName = "ZH_2000";
            Name = "ZH_2000";
            Type = CoordinateType.ProjectedCoordinateSystem;
            EPSG = "";
            CentralLongitude = 113.55f;

            //下面两个值不使用
            LatitudeOrigin = double.NaN;
            LongitudeOrigin = double.NaN;
        }
    }
}
