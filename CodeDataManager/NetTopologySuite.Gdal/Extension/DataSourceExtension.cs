using OSGeo.OGR;
using OSGeo.OSR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class DataSourceExtension
    {
        public static string GetNameEx(this DataSource dataSource)
        {
            return EncodingUtils.Gb2312ToUtf8(dataSource.GetName());
        }

        public static Layer CreateLayerEx(this DataSource dataSource, string name, SpatialReference srs, wkbGeometryType geom_type, string[] options)
        {
            return dataSource.CreateLayer(EncodingUtils.Utf8ToGb2312(name), srs, geom_type, options);
        }

        public static Layer CopyLayerEx(this DataSource dataSource, Layer src_layer, string new_name, string[] options)
        {
            return dataSource.CopyLayer(src_layer, EncodingUtils.Utf8ToGb2312(new_name), options);
        }

        public static Layer GetLayerByNameEx(this DataSource dataSource, string layer_name)
        {
            return dataSource.GetLayerByName(EncodingUtils.Utf8ToGb2312(layer_name));
        }
    }
}
