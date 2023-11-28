using NetTopologySuite.Features.Fields;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.Features
{
    public interface ILayerUtils
    {
        /// <summary>
        /// 获取图层类型
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <returns></returns>
        ShapeType GetShapeType(string layName);
        /// <summary>
        /// 获取投影
        /// </summary>
        /// <param name="layName"></param>
        /// <returns></returns>
        string GetProjection(string layName);

        /// <summary>
        /// 读取图层所有的feature
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <param name="backFieldNames">返回字段，null默认全部</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        Feature[] ReadAllFeatures(string layName,string backFieldNames=null, string whereClause = null);
        /// <summary>
        /// 读取图层所有的几何
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        Geometry[] ReadAllGeometries(string layName, string whereClause = null);
        /// <summary>
        /// 获取属性列
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <returns></returns>
        DbfFieldCollection GetDbfFields(string layName);
        /// <summary>
        /// 获取图层的所有属性，字典key对应返回字段，如果为空则返回全部
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <param name="backFieldNames">返回字段</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        IList<IDictionary<string,object>> GetAttritubes(string layName, string backFieldNames = null, string whereClause = null);
        /// <summary>
        /// 把feature写入当前图层
        /// </summary>
        /// <param name="features">图层要素</param>
        /// <param name="layName">图层名称</param>
        /// <param name="projection">投影坐标</param>
        /// <param name="encoding">字符编码</param>
        void WriteAllFeatures(IEnumerable<IFeature> features, string layName, string projection = null, Encoding encoding = null);
        /// <summary>
        /// 删除要素
        /// </summary>
        /// <param name="feature"></param>
        //void DeleteFeature(IFeature feature);
    }
}
