using AGSpatialDataCheck.GISUtils.CheckParams;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using ServiceCenter.Core;
using System;
using System.Collections.Generic;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class PointToLineCheckUtils:BaseMessageUtils
    {

        /// <summary>
        /// 线要素是否与点要素有关联(孤立管线检测)
        /// </summary>
        /// <param name="multiLayer"></param>
        /// <returns></returns>
        public BaseMessage IsSingleLine(MultiSdeLayer ptLineLayer)
        {
            try
            {
                //获取检查的点线要素类几何类型是否正确
                var ptShapeType = LayerHelper.GetShapeType(ptLineLayer.layer1);
                var lineShapeType = LayerHelper.GetShapeType(ptLineLayer.layer2);
                bool IsPoint = ptShapeType.IsPoint();
                bool IsPolyLine = lineShapeType.IsPolyLine();
                if (!IsPoint)
                {
                    return Fail(new Exception(ptLineLayer.layer1.LayerName + "不为点要素类"));
                }
                if (!IsPolyLine)
                {
                    return Fail(new Exception(ptLineLayer.layer2.LayerName + "不为线要素类"));
                }

                
                //List<string[]> ret = new List<string[]>();
                //var retSpace = new string[1000];//储存空间上不跟管点相接的点号记录
                //var retProperty = new string[1000];//储存管线属性表中起止点编号在管点中无法找到相应点号的记录
                List<string> retLine = new List<string>();
                int featureCount = 0;
                var lineFeatures = LayerHelper.GetFeatures(ptLineLayer.layer2);
                var ptFeatures = LayerHelper.GetFeatures(ptLineLayer.layer1);
                var pExtraData = ptLineLayer.layer1.ExtraData.ToDictionary();
                var PointNumField = pExtraData["PointNumField"].ToString();
                var lineExtraData = ptLineLayer.layer2.ExtraData.ToDictionary();
                var LineStartField = lineExtraData["LineStartField"].ToString();
                var LineEndField = lineExtraData["LineEndField"].ToString();
                //遍历管线
                foreach (Feature lineFea in lineFeatures)
                {
                    bool IsSpaceRelated = false;//空间上关联
                    bool IsTableRelated = false;//属性表点号关联

                    featureCount++;
                    Geometry gmline = lineFea.Geometry;//获取线几何
                    //获取管线的起止点编号
                    string qdValue = lineFea.Attributes[LineStartField].ToString();
                    string zdValue = lineFea.Attributes[LineEndField].ToString();

                    foreach (Feature ptFea in ptFeatures)
                    {
                        Geometry gmpt = ptFea.Geometry;//管点几何
                        //如果找到在空间上关联管线
                        if(gmpt.Intersects(gmline))
                        {
                            IsSpaceRelated = true;
                        }
                        #region 旧的检测点是否在线上的方法
                        //Coordinate ptCd = gmpt.Coordinate;//管点坐标
                        //for (int a = 0; a < gmline.Coordinates.Length; a++)
                        //{
                        //    //if (a == gmline.Coordinates.Length - 1)
                        //    //    break;
                        //    Coordinate cd1 = gmline.Coordinates[a];
                        //    Coordinate cd2 = gmline.Coordinates[a + 1];
                        //    double dis1 = CheckHelper.Calculate(ptCd.X, ptCd.Y, 0, cd1.X, cd1.Y, 0);
                        //    double dis2 = CheckHelper.Calculate(ptCd.X, ptCd.Y, 0, cd2.X, cd2.Y, 0);
                        //    double disLine = CheckHelper.Calculate(cd1.X, cd1.Y, 0, cd2.X, cd2.Y, 0);
                        //    if (dis1 < 0.000000000001 || dis2 < 0.000000000001)
                        //    {
                        //        havePoint = true;
                        //    }
                        //}
                        #endregion
                        //查找属性表中是否跟管点数据中的点号关联
                        string ptValue = ptFea.Attributes[PointNumField].ToString();
                        if(ptValue==qdValue || ptValue==zdValue )
                        {
                            IsTableRelated = true;
                        }

                    }

                    if(!IsSpaceRelated || !IsTableRelated)
                    {
                        retLine.Add(lineFea.Attributes[ptLineLayer.layer2.UniqueFieldName].ToString());
                    }
                }
                return Success(retLine);
            }
            catch (Exception e)
            {
                //Toolset.TinyLogger.WriteLog(e.Message, e);
                return Fail(e);
            }
        }

        /// <summary>
        /// 点要素是否与线要素有关联(孤立管点检测)
        /// </summary>
        /// <param name="multiLayer"></param>
        /// <returns></returns>
        public BaseMessage IsSinglePoint(MultiSdeLayer ptLineLayer)
        {
            try
            {
                //获取检查的点线要素类几何类型是否正确
                var ptShapeType = LayerHelper.GetShapeType(ptLineLayer.layer1);
                var lineShapeType = LayerHelper.GetShapeType(ptLineLayer.layer2);
                if (!ptShapeType.IsPoint())
                {
                    return Fail(new Exception(ptLineLayer.layer1.LayerName + "不为点要素类"));
                }
                if (!lineShapeType.IsPolyLine())
                {
                    return Fail(new Exception(ptLineLayer.layer2.LayerName + "不为线要素类"));
                }

                
                List<string> retPoint = new List<string>();
                int featureCount = 0;
                var lineFeatures = LayerHelper.GetFeatures(ptLineLayer.layer2);
                var ptFeatures = LayerHelper.GetFeatures(ptLineLayer.layer1);
                var pExtraData = ptLineLayer.layer1.ExtraData.ToDictionary();
                var PointNumField = pExtraData["PointNumField"].ToString();
                var lineExtraData = ptLineLayer.layer2.ExtraData.ToDictionary();
                var LineStartField = lineExtraData["LineStartField"].ToString();
                var LineEndField = lineExtraData["LineEndField"].ToString();
                //遍历管点
                foreach (Feature pointFea in ptFeatures)
                {
                    bool IsSpaceRelated = false;//空间上关联
                    bool IsTableRelated = false;//属性表点号关联

                    featureCount++;
                    Geometry gmPoint = pointFea.Geometry;//获取点几何
                    //获取管点编号
                    string ptValue = pointFea.Attributes[PointNumField].ToString();
                    foreach (Feature lineFea in lineFeatures)
                    {
                        Geometry gmLine = lineFea.Geometry;//管线几何
                        //如果找到管线在空间上关联管点
                        if (gmPoint.Intersects(gmLine))
                        {
                            IsSpaceRelated = true;
                        }
                        //查找属性表中是否跟管点数据中的点号关联
                        string qdValue = lineFea.Attributes[LineStartField].ToString();
                        string zdValue = lineFea.Attributes[LineEndField].ToString();
                        
                        if (qdValue == ptValue || zdValue == ptValue)
                        {
                            IsTableRelated = true;
                        }

                    }

                    if (!IsSpaceRelated || !IsTableRelated)
                    {
                        retPoint.Add(pointFea.Attributes[ptLineLayer.layer1.UniqueFieldName].ToString());
                    }
                }
                
                return Success(retPoint);
            }
            catch (Exception e)
            {
                //Toolset.TinyLogger.WriteLog(e.Message, e);
                return Fail(e);
            }
        }

        /// <summary>
        /// 点要素是否被线要素覆盖
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsCovered(MultiSdeLayer multiLayer)
        {
            return null;
        }

        /// <summary>
        /// 点要素不能在线上检查
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsOnLine(MultiSdeLayer multiLayer)
        {
            return null;
        }
    }
}
