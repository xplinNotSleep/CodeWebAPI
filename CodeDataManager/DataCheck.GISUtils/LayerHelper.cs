using AGSpatialDataCheck.GISUtils.CheckParams;
using NetTopologySuite.Features;
using NetTopologySuite.Features.Fields;
using NetTopologySuite.GdalEx;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Postgis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AGSpatialDataCheck.GISUtils
{
    public class LayerHelper
    {
        static readonly ConcurrentDictionary<string, ILayerUtils> dictLayerUtils = new ConcurrentDictionary<string, ILayerUtils>();
        public static ShapeType GetShapeType(CheckLayer layer)
        {
            var layerUtils = GetLayerUtils(layer);
            ShapeType layerShape = layerUtils.GetShapeType(layer.LayerName);
            return layerShape;
        }

        public static string GetProjection(CheckLayer layer)
        {
            var layerUtils = GetLayerUtils(layer);
            return layerUtils.GetProjection(layer.LayerName);
        }

        public static DbfFieldCollection GetDbfFields(CheckLayer layer)
        {
            var layerUtils = GetLayerUtils(layer);
            return layerUtils.GetDbfFields(layer.LayerName);
        }

        public static IList<IDictionary<string, object>> GetAttritubes(CheckLayer layer)
        {

            var layerUtils = GetLayerUtils(layer);
            return layerUtils.GetAttritubes(layer.LayerName, layer.BackFieldNames, layer.WhereClause);

        }

        public static Feature[] GetFeatures(CheckLayer layer)
        {
            var layerUtils = GetLayerUtils(layer);
            return layerUtils.ReadAllFeatures(layer.LayerName, layer.WhereClause);
        }

        public static Geometry[] GetGeometries(CheckLayer layer)
        {
            var layerUtils = GetLayerUtils(layer);
            return layerUtils.ReadAllGeometries(layer.LayerName, layer.WhereClause);
        }

        public static void WriteAllFeatures(CheckLayer layer, IEnumerable<IFeature> features, string projection = null, Encoding encoding = null)
        {
            var layerUtils = GetLayerUtils(layer);
            layerUtils.WriteAllFeatures(features, layer.LayerName, projection, encoding);
        }

        static ILayerUtils GetLayerUtils(CheckLayer layer)
        {
            ILayerUtils layerUtils;
            if (dictLayerUtils.ContainsKey(layer.LayerPath))
            {
                layerUtils = dictLayerUtils[layer.LayerPath];
            }
            else
            {
                if (layer.LayerType == "sde")
                {
                    layerUtils = new SdeLayerUtils(layer);
                }
                else if (layer.LayerType == "gdb")
                {
                    layerUtils = new GdalLayerUtils(GdalFileType.Gdb, layer.LayerPath);
                }
                else
                {
                    layerUtils = new ShapefileUtils(layer.LayerPath);
                }
                dictLayerUtils[layer.LayerPath] = layerUtils;
            }
            return layerUtils;
        }

        public static void HandleGeometry<T>(Feature feature, Geometry geometry, Action<T, Feature> action) where T : Geometry
        {
            if (geometry is GeometryCollection)
            {
                var geometries = geometry as GeometryCollection;
                foreach (var geo in geometries)
                {
                    HandleGeometry(feature, geo, action);
                }
            }
            else
            {
                action(geometry as T, feature);
            }

        }
    }
}
