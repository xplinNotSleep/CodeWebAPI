using NetTopologySuite.Features;
using NetTopologySuite.Features.Fields;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.GdalEx
{
    public class GdalLayerUtils:ILayerUtils
    {
        IReader reader { get; set; }
        public GdalLayerUtils(GdalFileType fileType, string path)
        {
            reader = GdalReader.GetReader(fileType, path);
        }

        public ShapeType GetShapeType(string layName)
        {
            var layer = reader.GetLayerByName(layName);
            var type = layer.GetGeomType();
            switch (type)
            {
                case OSGeo.OGR.wkbGeometryType.wkbPoint: 
                    return ShapeType.Point;
                case OSGeo.OGR.wkbGeometryType.wkbLineString: 
                    return ShapeType.PolyLine;
                case OSGeo.OGR.wkbGeometryType.wkbMultiLineString:
                    return ShapeType.PolyLine;
                case OSGeo.OGR.wkbGeometryType.wkbPolygon:
                    return ShapeType.Polygon;
                case OSGeo.OGR.wkbGeometryType.wkbMultiPoint: 
                    return ShapeType.MultiPoint;
                case OSGeo.OGR.wkbGeometryType.wkbPointZM:
                    return ShapeType.PointZM;
                case OSGeo.OGR.wkbGeometryType.wkbLineStringZM:
                    return ShapeType.PolyLineZM;
                case OSGeo.OGR.wkbGeometryType.wkbPolygonZM:
                    return ShapeType.PolygonZM;
                case OSGeo.OGR.wkbGeometryType.wkbMultiPointZM: 
                    return ShapeType.MultiPointZM;
                case OSGeo.OGR.wkbGeometryType.wkbPointM: 
                    return ShapeType.PointM;
                case OSGeo.OGR.wkbGeometryType.wkbLineStringM:
                    return ShapeType.PolyLineM;
                case OSGeo.OGR.wkbGeometryType.wkbPolygonM:
                    return ShapeType.PolygonM;
                case OSGeo.OGR.wkbGeometryType.wkbMultiPointM: 
                    return ShapeType.MultiPointM;
                case OSGeo.OGR.wkbGeometryType.wkbGeometryCollection: 
                    return ShapeType.MultiPatch;
            }
            return ShapeType.NullShape;
        }

        public string GetProjection(string layName)
        {
            var layer = reader.GetLayerByName(layName);
            var srs = layer.GetSpatialRef();
            return srs.__str__();
        }

        public Feature[] ReadAllFeatures(string layName, string backFieldNames=null, string whereClause = null)
        {
            var layer=reader.GetLayerByName(layName);
            var gdalFeatures = reader.GetFeatures(layer);
            var fields=reader.GetFields(layer,backFieldNames);
            List<Feature> features = new List<Feature>();
            foreach (var f in gdalFeatures)
            {
                var geo=f?.GetGeometryRef();
                if(geo==null) continue;
                byte[] bytes = new byte[geo.WkbSize()];
                geo.ExportToWkb(bytes);
                MemoryStream ms = new MemoryStream(bytes);
                WKBReader wkbreader = new WKBReader();
                var geometry = wkbreader.Read(ms);
                var attributes = reader.GetAttrbute(f, fields);
                Feature feature = new Feature(geometry, new AttributesTable(attributes));
                features.Add(feature);
            }
            return features.ToArray();
        }

        public Geometry[] ReadAllGeometries(string layName, string whereClause = null)
        {
            var geos = reader.GetGeometries(layName);
            List<Geometry> geometries = new List<Geometry>();
            foreach (var geo in geos)
            {
                byte[] bytes = new byte[geo.WkbSize()];
                geo.ExportToWkb(bytes);
                MemoryStream ms = new MemoryStream(bytes);
                WKBReader reader = new WKBReader();
                var geometry = reader.Read(ms);
                geometries.Add(geometry);
            }
            return geometries.ToArray();
        }

        /// <summary>
        /// 此方法先搁置，以后有需要的时候再补充
        /// </summary>
        /// <param name="features"></param>
        /// <param name="layName"></param>
        /// <param name="projection"></param>
        /// <param name="encoding"></param>
        public void WriteAllFeatures(IEnumerable<IFeature> features, string layName, string projection = null, Encoding encoding = null)
        {
            throw new System.Exception("此方法先搁置，以后有需要的时候再补充");
        }

        public IList<IDictionary<string, object>> GetAttritubes(string layName, string backFieldNames = null, string whereClause = null)
        {
            return reader.GetAttributes(layName);
        }

        public DbfFieldCollection GetDbfFields(string layName)
        {
            var fields=reader.GetFields(layName);
            DbfFieldCollection dbfFields = new DbfFieldCollection(fields.Count);
            foreach (var field in fields)
            {
                switch (field.Type)
                {
                    case OSGeo.OGR.FieldType.OFTString:
                        dbfFields.Add(new DbfCharacterField(field.Name, field.Width));
                        break;
                    case OSGeo.OGR.FieldType.OFTInteger:
                        dbfFields.Add(new DbfNumericInt32Field(field.Name, field.Width));
                        break;
                    case OSGeo.OGR.FieldType.OFTInteger64:
                        dbfFields.Add(new DbfNumericInt64Field(field.Name, field.Width));
                        break;
                    case OSGeo.OGR.FieldType.OFTDate:
                    case OSGeo.OGR.FieldType.OFTDateTime:
                        dbfFields.Add(new DbfDateField(field.Name));
                        break;
                    case OSGeo.OGR.FieldType.OFTReal:
                        dbfFields.Add(new DbfNumericDoubleField(field.Name,field.Width,field.Precision));
                        break;
                    default:
                        dbfFields.Add(new DbfCharacterField(field.Name, field.Width));
                        break;
                }
            }
            return dbfFields;
        }
    }
}
