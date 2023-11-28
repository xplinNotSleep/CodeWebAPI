using AGSpatialDataCheck.GISUtils.CheckParams;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using ServiceCenter.Core;
using System;
using System.Collections.Generic;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class LineCheckUtils: BaseMessageUtils
    {
        /// <summary>
        /// 线自相交检查,包括封闭图形
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public BaseMessage IsSelfIntersected(CheckLayer layer)
        {
            try
            {
                var shapeType = LayerHelper.GetShapeType(layer);
                if (!shapeType.IsPolyLine())
                {
                    return Fail(new Exception(layer.LayerName + "不为线要素类"));
                }
                var features = LayerHelper.GetFeatures(layer);
                var ret = new List<string>();//检查出的记录
                //判断线是否为孤立线
                Action<LineString, Feature> geoHandle = (line, feature) => {

                    if (!line.IsSimple && !line.IsRing)
                    {
                        ret.Add(feature.Attributes[layer.UniqueFieldName].ToString());
                    }
                };
                foreach (var feature in features)
                {
                    var geoType = feature.Geometry.OgcGeometryType;
                    if (geoType == OgcGeometryType.MultiLineString)
                    {
                        var multiLine = feature.Geometry as MultiLineString;
                        if (multiLine != null)
                        {
                            LayerHelper.HandleGeometry(feature, multiLine, geoHandle);
                        }
                    }
                    else
                    {
                        var line = feature.Geometry as LineString;
                        geoHandle(line, feature);
                    }
                }
                return Success(ret);
            }
            catch(Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 同一要素类中线要素相交检查
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public BaseMessage IsIntersected(CheckLayer layer)
        {
            try
            {
                var shapeType = LayerHelper.GetShapeType(layer);
                if (!shapeType.IsPolyLine())
                {
                    return Fail(new Exception(layer.LayerName + "不为线要素类"));
                }

                var ret = new List<string>();
                int featureCount = 0;
                var features = LayerHelper.GetFeatures(layer);
                //遍历要素的时候先不考虑是否为multi的情况
                foreach(Feature feature1 in features)
                {
                    featureCount++;
                    var gm1 = feature1.Geometry as Geometry;
                    string guid = feature1.Attributes[layer.UniqueFieldName].ToString();
                    foreach (Feature feature2 in features)
                    {
                        var gm2 = feature2.Geometry as Geometry;
                        if (gm2.Intersects(gm1))
                        {
                            if(!ret.Contains(guid))
                            {
                                ret.Add(guid);
                            }
                        }
                    }
                }

                return Success(ret);
            }
            catch(Exception e)
            {
                return Fail(e);
            }

            
        }

        /// <summary>
        /// 同一要素类中线要素重叠检查，包括重叠、包含与被包含等
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public BaseMessage IsOverlap(CheckLayer layer)
        {
            try
            {
                var shapeType = LayerHelper.GetShapeType(layer);
                if (!shapeType.IsPolyLine())
                {
                    return Fail(new Exception(layer.LayerName + "不为线要素类"));
                }

                var features = LayerHelper.GetFeatures(layer);
                var ret = new List<string>();
                int featureCount = 0;
                foreach (Feature feature1 in features)
                {
                    featureCount++;
                    var gm1 = feature1.Geometry as Geometry;
                    string guid1 = feature1.Attributes[layer.UniqueFieldName].ToString();
                    foreach (Feature feature2 in features)
                    {
                        string guid2 = feature2.Attributes[layer.UniqueFieldName].ToString();
                        if (guid1 == guid2) continue;
                        var gm2 = feature2.Geometry as Geometry;
                        
                        if (gm2.Overlaps(gm1))
                        {
                            if(!ret.Contains(guid1)) ret.Add(guid1);

                        }
                            
                    }
                }
                return Success(ret);
            }
            catch(Exception e)
            {
                return Fail(e);
            }

        }

        /// <summary>
        /// 同一要素类中线要素是否为孤立线
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>有悬挂端点的要素</returns>
        public BaseMessage IsHang(CheckLayer layer)
        {
            try
            {
                var shapeType = LayerHelper.GetShapeType(layer);
                if (!shapeType.IsPolyLine())
                {
                    return Fail(new Exception(layer.LayerName + "不为线要素类"));
                }

                var features = LayerHelper.GetFeatures(layer);
                var ret = new List<string>();
                int featureCount = 0;

                foreach (Feature feature in features)
                {
                    featureCount++;
                    var line = feature.Geometry as LineString;
                    if (line == null)
                    {
                        if (feature.Geometry as MultiLineString != null)
                        {
                            MultiLineString multiLineString = feature.Geometry as MultiLineString;
                            line = multiLineString.Geometries[0] as LineString;
                            if (line == null) continue;
                        }
                    }
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    Point pStart = line.StartPoint;
                    Point pEnd = line.EndPoint;
                    Geometry tGeoBufferStart = pStart.Buffer(layer.BufferDistance);
                    Geometry tGeoBufferEnd = pEnd.Buffer(layer.BufferDistance);
                    string guid = feature.Attributes[layer.UniqueFieldName].ToString();
                    foreach (Feature feature1 in features)
                    {
                        string guid1 = feature1.Attributes[layer.UniqueFieldName].ToString();
                        if (guid1 == guid) continue;
                        Envelope envelope1 = feature1.Geometry.EnvelopeInternal;
                        if (!envelope1.Intersects(tGeoBufferStart.EnvelopeInternal)
                            || !envelope1.Intersects(tGeoBufferEnd.EnvelopeInternal))
                        {
                            if (!ret.Contains(guid1)) ret.Add(guid1);
                        }
                    }
                }
                return Success(ret);
            }
            catch (Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查线要素是否具有多部件
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>有悬挂端点的要素</returns>
        public BaseMessage IsMultipart(CheckLayer layer)
        {
            return null;
        }



    }
}
