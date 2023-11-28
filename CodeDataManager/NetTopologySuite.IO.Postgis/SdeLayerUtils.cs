using NetTopologySuite.Features;
using NetTopologySuite.Features.Fields;
using NetTopologySuite.Geometries;
using Pure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NetTopologySuite.IO.Postgis
{
    public class SdeLayerUtils : ILayerUtils
    {
        public SdeLayerUtils(SdeConnectstringParam sdeConnectstringParam)
        {
            UpdateSdeConnectstring(sdeConnectstringParam);
        }

        SdeContontHelper sdeContontHelper { get; set; }
        /// <summary>
        /// 更新sde链接参数
        /// </summary>
        /// <param name="sdeConnectstringParam"></param>
        public void UpdateSdeConnectstring(SdeConnectstringParam sdeConnectstringParam)
        {
            sdeContontHelper = new SdeContontHelper(sdeConnectstringParam);
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="layName"></param>
        /// <returns></returns>
        public ShapeType GetShapeType(string layName)
        {
            return sdeContontHelper.GetShapeType(layName);
        }

        /// <summary>
        /// 获取投影
        /// </summary>
        /// <param name="layName"></param>
        /// <returns></returns>
        public string GetProjection(string layName)
        {
            return sdeContontHelper.GetProjection(layName);
        }

        /// <summary>
        /// 读取sde数据库下所有的feature
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public Feature[] ReadAllFeatures(string layName, string backFieldNames = null, string whereClause = null)
        {
            var dt = sdeContontHelper.GetLayer(layName, backFieldNames, whereClause);
            if (dt == null || dt.Rows.Count == 0) return null;
            List<Feature> features = new List<Feature>();
            foreach (DataRow row in dt.Rows)
            {
                byte[] data = (byte[])row["shape"];
                MemoryStream ms = new MemoryStream(data);
                WKBReader reader = new WKBReader();
                var geo = reader.Read(ms);
                Dictionary<string, object> attributes = new Dictionary<string, object>();
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName == "shape") continue;
                    attributes.Add(column.ColumnName, row[column.ColumnName]);
                }
                Feature feature = new Feature(geo, new AttributesTable(attributes));
                features.Add(feature);
            }
            return features.ToArray();
        }

        /// <summary>
        /// 读取sde数据库下所有的几何
        /// </summary>
        /// <param name="layName">图层名称</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public Geometry[] ReadAllGeometries(string layName, string whereClause = null)
        {
            var dt = sdeContontHelper.GetGeometry(layName, whereClause);
            if (dt == null || dt.Rows.Count == 0) return null;
            List<Geometry> geometries = new List<Geometry>();
            foreach (DataRow row in dt.Rows)
            {
                byte[] data = (byte[])row["shape"];
                MemoryStream ms = new MemoryStream(data);
                WKBReader reader = new WKBReader();
                var geo = reader.Read(ms);
                geometries.Add(geo);
            }
            return geometries.ToArray();
        }

        public IList<IDictionary<string, object>> GetAttritubes(string layName, string backFieldNames = null, string whereClause = null)
        {
            return sdeContontHelper.GetAttritubes(layName, backFieldNames, whereClause);
        }

        public DbfFieldCollection GetDbfFields(string layName)
        {
            var columns = sdeContontHelper.GetColumns(layName);
            DbfFieldCollection dbfFields = new DbfFieldCollection(columns.Count());
            foreach (var column in columns)
            {
                switch (column.Type)
                {
                    case DbType.String:
                    case DbType.AnsiString:
                        dbfFields.Add(new DbfCharacterField(column.ColumnName, column.ColumnLength));
                        break;
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.UInt16:
                    case DbType.UInt32:
                        dbfFields.Add(new DbfNumericInt32Field(column.ColumnName, column.ColumnLength));
                        break;
                    case DbType.Int64:
                    case DbType.UInt64:
                        dbfFields.Add(new DbfNumericInt64Field(column.ColumnName, column.ColumnLength));
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                        dbfFields.Add(new DbfDateField(column.ColumnName));
                        break;
                    case DbType.Single:
                    case DbType.Double:
                    case DbType.Decimal:
                        dbfFields.Add(new DbfNumericDoubleField(column.ColumnName, column.ColumnLength, column.ColumnPrecision));
                        break;
                    default:
                        dbfFields.Add(new DbfCharacterField(column.ColumnName, column.ColumnLength));
                        break;
                }
            }
            return dbfFields;
        }

        /// <summary>
        /// 获取图层字段
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public IEnumerable<ColumnInfo> GetColumns(string layerName)
        {
            return sdeContontHelper.GetColumns(layerName);
        }

        /// <summary>
        /// 创建几何表
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="columns"></param>
        /// <param name="IsOverwise"></param>
        public void CreateGeometryTable(string layerName, IEnumerable<ColumnInfo> columns, bool IsOverwise = true)
        {
            sdeContontHelper.CreateGeometryTable(layerName, columns, IsOverwise);
        }

        /// <summary>
        /// 把要素写入图层，暂时不处理投影和编码
        /// </summary>
        /// <param name="features"></param>
        /// <param name="layName"></param>
        /// <param name="projection"></param>
        /// <param name="encoding"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void WriteAllFeatures(IEnumerable<IFeature> features, string layName, string projection = null, Encoding encoding = null)
        {
            if (features == null)
                throw new ArgumentNullException(nameof(features));
            sdeContontHelper.InsertLayer(layName, features);
        }

        /// <summary>
        /// sde库内复制图层
        /// </summary>
        /// <param name="orgin_layerName"></param>
        /// <param name="src_layerName"></param>
        public void CopyLayer(string orgin_layerName, string src_layerName)
        {
            var columns = GetColumns(orgin_layerName);
            CreateGeometryTable(src_layerName, columns);
            var features = ReadAllFeatures(orgin_layerName);
            sdeContontHelper.InsertLayer(src_layerName, features);
        }

        /// <summary>
        /// 把features复制到sde库内
        /// </summary>
        /// <param name="features"></param>
        /// <param name="columns"></param>
        /// <param name="src_layName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void CopyLayer(IEnumerable<IFeature> features, IEnumerable<ColumnInfo> columns, string src_layName)
        {
            if (features == null)
                throw new ArgumentNullException(nameof(features));
            CreateGeometryTable(src_layName, columns);
            sdeContontHelper.InsertLayer(src_layName, features);
        }
    }
}
