using OSGeo.OGR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class GeometryExtension
    {
        public static string GetGeometryNameEx(this Geometry geometry)
        {
            return EncodingUtils.Gb2312ToUtf8(geometry.GetGeometryName());
        }
    }
}
