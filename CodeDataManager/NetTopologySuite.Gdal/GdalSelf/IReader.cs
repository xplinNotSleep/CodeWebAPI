using OSGeo.OGR;
using System.Collections.Generic;

namespace NetTopologySuite.GdalEx
{
    public interface IReader
    {
        DataSource GetDataSource();
        List<string> GetLayerNames();
        Layer GetLayerByName(string layerName);
        List<Feature> GetFeatures(string layerName);
        List<Feature> GetFeatures(Layer layer);
        List<FieldAttribute> GetFields(string layerName);
        List<FieldAttribute> GetFields(Layer layer, string backFieldNames = null);
        IList<IDictionary<string, object>> GetAttributes(string layerName);
        IList<IDictionary<string, object>> GetAttributes(List<Feature> features, List<FieldAttribute> fields);
        IDictionary<string, object> GetAttrbute(Feature feature, List<FieldAttribute> fields);
        List<FeatureDefn> GetFeatureDefns(string layerName);
        List<FeatureDefn> GetFeatureDefns(Layer layer);
        List<Geometry> GetGeometries(string layerName);
        List<Geometry> GetGeometries(Layer layer);
    }
}
