using AGSpatialDataCheck.GISUtils.CheckParams;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class PointCheckUtils:BaseMessageUtils
    {
        /// <summary>
        /// 点重叠检查
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public BaseMessage IsOverlap(CheckLayer layer)
        {
            try
            {
                var shapeType = LayerHelper.GetShapeType(layer);
                if (!shapeType.IsPoint())
                {
                    return Fail(new Exception(layer.LayerName + "不为点要素类"));
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
            catch (Exception e)
            {
                return Fail(e);
            }
        }


    }
}
