using NetTopologySuite.GdalEx.Extension;
using OSGeo.OGR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetTopologySuite.GdalEx
{
    public enum GdalFileType
    {
        Shp = 0,
        Gdb = 1,
        Sde = 2,
        UnKnown = 9
    }

    public class GdalReader : IReader
    {
        static ConcurrentDictionary<string, IReader> Readers { get; set; } = new ConcurrentDictionary<string, IReader>();
        bool IsUTF8 { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="path">
        /// sde的路径写法：PG:dbname=testgdal host=172.18.2.168 port=5432 user=sde password=sde
        /// </param>
        /// <returns></returns>
        public static IReader GetReader(GdalFileType fileType, string path)
        {
            if (Readers.ContainsKey(path))
            {
                return Readers[path];
            }
            IReader reader = null;
            switch (fileType)
            {
                case GdalFileType.Shp:
                    reader = new GdalReader("ESRI Shapefile", path, false);
                    break;
                case GdalFileType.Gdb:
                    reader = new GdalReader("OpenFileGDB", path, false);
                    break;
                case GdalFileType.Sde:
                    reader = new GdalReader("PostgreSQL", path, true);
                    break;
                default:
                    reader = null;
                    break;
            }
            Readers[path] = reader;
            return reader;
        }

        DataSource dataSource = null;
        public GdalReader(string typeName, string path, bool isUTF8 = false)
        {
            var driver = Ogr.GetDriverByName(typeName);
            dataSource = driver.Open(path, 1);
            IsUTF8 = isUTF8;
        }

        public DataSource GetDataSource() { return dataSource; }

        public List<string> GetLayerNames()
        {
            var layers = new List<string>();
            for (int i = 0; i < dataSource.GetLayerCount(); i++)
            {
                var layer = dataSource.GetLayerByIndex(i);
                layers.Add(IsUTF8 ? layer.GetNameEx() : layer.GetName());
            }
            return layers;
        }

        public Layer GetLayerByName(string layerName)
        {
            return IsUTF8 ? dataSource.GetLayerByNameEx(layerName) : dataSource.GetLayerByName(layerName);
        }

        public List<Feature> GetFeatures(string layerName)
        {
            var layer = GetLayerByName(layerName);
            return GetFeatures(layer);
        }

        public List<Feature> GetFeatures(Layer layer)
        {
            List<Feature> features = new List<Feature>();

            long feaCount = layer.GetFeatureCount(1);
            for (int i = 0; i < feaCount; i++)
            {
                var featrue = layer.GetFeature(i);
                if (featrue == null) continue;
                features.Add(featrue);
            }
            return features;
        }

        public List<FieldAttribute> GetFields(string layerName)
        {
            var layer = GetLayerByName(layerName);
            return GetFields(layer);
        }

        public List<FieldAttribute> GetFields(Layer layer,string backFieldNames=null)
        {
            List<FieldAttribute> attrs = new List<FieldAttribute>();
            var featrueDefn = layer.GetLayerDefn();
            List<string> listBackFieldNames = null;
            if(!string.IsNullOrEmpty( backFieldNames ))
            {
                listBackFieldNames=backFieldNames.ToLower().Split(new char[] { ',' }).ToList();
            }
            for (int j = 0; j < featrueDefn.GetFieldCount(); j++)
            {
                var field = featrueDefn.GetFieldDefn(j);
                var name= (IsUTF8 ? field.GetNameEx() : field.GetName()).ToLower();
                if (listBackFieldNames != null && !listBackFieldNames.Contains(name))
                    continue;
                FieldAttribute fieldAttr = new FieldAttribute();
                fieldAttr.Name = name;
                fieldAttr.Type = field.GetFieldType();
                fieldAttr.Width = field.GetWidth();
                fieldAttr.Precision = field.GetPrecision();
                attrs.Add(fieldAttr);
            }
            return attrs;
        }
        public IList<IDictionary<string, object>> GetAttributes(string layerName)
        {
            var layer = GetLayerByName(layerName);
            var features = GetFeatures(layer);
            var fields = GetFields(layer);
            return GetAttributes(features, fields);
        }

        public IList<IDictionary<string, object>> GetAttributes(List<Feature> features,List<FieldAttribute> fields)
        {
            IList<IDictionary<string, object>> attrs = new List<IDictionary<string, object>>();
            foreach (var feature in features)
            {
                attrs.Add(GetAttrbute(feature,fields));
            }
            return attrs;
        }

        public IDictionary<string, object> GetAttrbute(Feature feature, List<FieldAttribute> fields)
        {
            IDictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                object val = null;
                switch (field.Type)
                {
                    case FieldType.OFTString:
                        val = IsUTF8 ? feature.GetFieldAsStringEx(field.Name) : feature.GetFieldAsString(field.Name);
                        break;
                    case FieldType.OFTInteger:
                        val = IsUTF8 ? feature.GetFieldAsIntegerEx(field.Name) : feature.GetFieldAsInteger(field.Name);
                        break;
                    case FieldType.OFTInteger64:
                        val = IsUTF8 ? feature.GetFieldAsInteger64Ex(field.Name) : feature.GetFieldAsInteger64(field.Name);
                        break;
                    case FieldType.OFTDate:
                    case FieldType.OFTDateTime:
                        val = IsUTF8 ? feature.GetFieldAsISO8601DateTimeEx(field.Name, null) : feature.GetFieldAsISO8601DateTime(field.Name, null);
                        break;
                    case FieldType.OFTReal:
                        val = IsUTF8 ? feature.GetFieldAsDoubleEx(field.Name) : feature.GetFieldAsDouble(field.Name);
                        break;
                    default:
                        val = IsUTF8 ? feature.GetFieldAsStringEx(field.Name) : feature.GetFieldAsString(field.Name);
                        break;
                }
                keyValuePairs.Add(field.Name, val);
            }
            return keyValuePairs;
        }

        public List<FeatureDefn> GetFeatureDefns(string layerName)
        {
            var layer = GetLayerByName(layerName);
            return GetFeatureDefns(layer);
        }

        public List<FeatureDefn> GetFeatureDefns(Layer layer)
        {
            List<FeatureDefn> featureDefns = new List<FeatureDefn>();
            for (int i = 0; i < layer.GetFeatureCount(1); i++)
            {
                featureDefns.Add(layer.GetFeature(i).GetDefnRef());
            }
            return featureDefns;
        }

        public List<Geometry> GetGeometries(string layerName)
        {
            var layer = GetLayerByName(layerName);
            return GetGeometries(layer);
        }

        public List<Geometry> GetGeometries(Layer layer)
        {
            List<Geometry> geos = new List<Geometry>();
            for (long i = 0; i < layer.GetFeatureCount(1); i++)
            {
                var oFeature = layer.GetFeature(i);
                var oGeo = oFeature?.GetGeometryRef();
                if (oGeo != null)
                {
                    geos.Add(oGeo);
                }
            }
            return geos;
        }
    }
}
