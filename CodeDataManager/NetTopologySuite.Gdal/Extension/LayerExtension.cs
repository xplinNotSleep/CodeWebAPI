using OSGeo.OGR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class LayerExtension
    {
        public static string GetNameEx(this Layer layer)
        {
            return EncodingUtils.Gb2312ToUtf8(layer.GetName());
        }

        public static void ReNameEx(this Layer layer, string new_name)
        {
            layer.Rename(EncodingUtils.Utf8ToGb2312(new_name));
        }
    }
}
