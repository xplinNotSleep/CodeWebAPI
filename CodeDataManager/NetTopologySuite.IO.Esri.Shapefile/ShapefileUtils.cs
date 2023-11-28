using NetTopologySuite.Features;
using NetTopologySuite.Features.Fields;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri
{
    public class ShapefileUtils : ILayerUtils
    {
        string layerPath { get; set; }
        public ShapefileUtils() { }
        public ShapefileUtils(string layerPath)
        {
            this.layerPath = layerPath;
        }

        public ShapeType GetShapeType(string layName)
        {
            layName = GetLayerName(layName);
            return Shapefile.GetShapeType(layName);
        }

        public string GetProjection(string layName)
        {
            layName = GetLayerName(layName);
            var projFile = Path.ChangeExtension(layName, ".prj");
            if (File.Exists(projFile))
            {
                return File.ReadAllText(projFile);
            }
            return null;
        }

        public Feature[] ReadAllFeatures(string layName, string backFieldNames = null, string whereClause = null)
        {
            layName = GetLayerName(layName);
            return Shapefile.ReadAllFeatures(layName, backFieldNames,whereClause);
        }

        public Geometry[] ReadAllGeometries(string layName, string whereClause = null)
        {
            layName = GetLayerName(layName);
            return Shapefile.ReadAllGeometries(layName);
        }

        public void WriteAllFeatures(IEnumerable<IFeature> features, string layName, string projection = null, Encoding encoding = null)
        {
            layName = GetLayerName(layName);
            Shapefile.WriteAllFeatures(features, layName, projection, encoding);
        }

        public DbfFieldCollection GetDbfFields(string layName)
        {
            using(var dbfReader = new DbfReader(layName))
            {
                return dbfReader.Fields;
            }
        }

        public IList<IDictionary<string, object>> GetAttritubes(string layName, string backFieldNames = null, string whereClause = null)
        {
            return Shapefile.GetAttritubes(layName, backFieldNames, whereClause);
        }

        private string GetLayerName(string layName)
        {
            if (!File.Exists(layName) && this.layerPath != null)
            {
                if (layName.ToLower().EndsWith(".shp"))
                {
                    layName = Path.Combine(this.layerPath, layName);
                }
                else
                {
                    layName = Path.Combine(this.layerPath, layName + ".shp");
                }
            }
            return layName;
        }
    }
}
