using OSGeo.OGR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class FeatureDefnExtension
    {
        public static string GetNameEx(this FeatureDefn featureDefn)
        {
            return EncodingUtils.Gb2312ToUtf8(featureDefn.GetName());
        }

        public static int GetFieldIndexEx(this FeatureDefn featureDefn, string field_name)
        {
            return featureDefn.GetFieldIndex(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static int GetGeomFieldIndexEx(this FeatureDefn featureDefn, string field_name)
        {
            return featureDefn.GetGeomFieldIndex(EncodingUtils.Utf8ToGb2312(field_name));
        }
    }
}
