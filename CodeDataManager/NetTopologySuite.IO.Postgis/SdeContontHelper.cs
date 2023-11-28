using NetTopologySuite.Features;
using NetTopologySuite.Features.Helpers;
using Pure.Data;
using Pure.Data.Migration.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NetTopologySuite.IO.Postgis
{
    internal class SdeContontHelper
    {
        IDatabase Database { get; set; }
        ITransformationProvider TransformationProvider { get; set; }
        public SdeContontHelper(SdeConnectstringParam param)
        {
            Database = new Database($"Host={param.Host};Port={param.Port};Database={param.Databese};Username={param.Username};Password={param.Password}", DatabaseType.PostgreSQL);
            TransformationProvider = Database.CreateTransformationProvider();
            TransformationProvider.SetSchema(param.Username);
        }

        internal DataTable GetLayer(string layerName, string backFieldNames = null, string whereClause = null)
        {
            if (!TransformationProvider.TableExists(layerName))
            {
                throw new Exception("表名有误,请验证");
            }
            if (!TransformationProvider.ColumnExists(layerName, "shape"))
            {
                throw new Exception("当前表非空间表");
            }
            string columnsStr = "";
            if(string.IsNullOrEmpty(backFieldNames))
            {
                var columns = TransformationProvider.GetColumnInfoByTableName(layerName);
                foreach (var column in columns.Where(p => p.ColumnName != "shape"))
                {
                    columnsStr += column.ColumnName + ",";
                }
                columnsStr=columnsStr.TrimEnd(',');
            }
            else
            {
                columnsStr = backFieldNames;
            }
            var sql = $"select {columnsStr},st_asbinary(shape) as shape from {layerName}";
            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += " where " + whereClause;
            }
            return Database.ExecuteDataTable(sql);
        }

        internal IList<IDictionary<string, object>> GetAttritubes(string layerName, string backFieldNames = null, string whereClause = null)
        {
            if (!TransformationProvider.TableExists(layerName))
            {
                throw new Exception("表名有误,请验证");
            }
            string columnsStr = "";
            if (string.IsNullOrEmpty(backFieldNames))
            {
                var columns = TransformationProvider.GetColumnInfoByTableName(layerName);
                foreach (var column in columns.Where(p => p.ColumnName != "shape"))
                {
                    columnsStr += column.ColumnName + ",";
                }
                columnsStr = columnsStr.TrimEnd(',');
            }
            else
            {
                columnsStr = backFieldNames;
            }
            var sql = $"select {columnsStr} from {layerName}";
            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += " where " + whereClause;
            }
            return Database.ExecuteList<dynamic>(sql).Select(p => p as IDictionary<string, object>).ToList();
        }

        internal IEnumerable<ColumnInfo> GetColumns(string layerName)
        {
            return TransformationProvider.GetColumnInfoByTableName(layerName);
        }

        internal void CreateGeometryTable(string layerName, IEnumerable<ColumnInfo> columns, bool IsOverwise = true)
        {
            if (TransformationProvider.TableExists(layerName))
            {
                if (IsOverwise)
                {
                    TransformationProvider.RemoveTable(layerName);
                }
                else
                {
                    return;
                }
            }
            var shapeColumn = columns.FirstOrDefault(p => p.ColumnName.ToLower() == "shape");
            if (shapeColumn != null)
            {
                shapeColumn.RawType = "st_geometry";
            }
            TransformationProvider.AddTable(layerName, columns);
        }

        internal DataTable GetGeometry(string layerName, string whereClause = null)
        {
            if (!TransformationProvider.TableExists(layerName))
            {
                throw new Exception("表名有误,请验证");
            }
            if (!TransformationProvider.ColumnExists(layerName, "shape"))
            {
                throw new Exception("当前表非空间表");
            }
            var sql = $"select st_asbinary(shape) as shape from {layerName}";
            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += " where " + whereClause;
            }
            return Database.ExecuteDataTable(sql);
        }

        internal ShapeType GetShapeType(string layerName)
        {
            if (!TransformationProvider.TableExists(layerName))
            {
                throw new Exception("表名有误,请验证");
            }
            if (!TransformationProvider.ColumnExists(layerName, "shape"))
            {
                throw new Exception("当前表非空间表");
            }
            var sql = $"select st_asbinary(shape) as shape from {layerName} limit 1";
            var bytes = (byte[])Database.ExecuteScalar(sql);
            WKBReader wKBReader = new WKBReader();
            var geometry = wKBReader.Read(bytes);
            return geometry.GetShapeType();
        }

        internal string GetProjection(string layName)
        {
            var sql = $"select b.srtext from sde_layers a left join public.sde_spatial_references b on a.srid =b.srid where a.table_name ='{layName}'";
            var ret = Database.ExecuteScalar(sql);
            if (ret != null)
            {
                return ret.ToString();
            }
            return null;
        }

        internal void InsertLayer(string layerName, IEnumerable<IFeature> features)
        {
            if (!TransformationProvider.ColumnExists(layerName, "shape"))
            {
                throw new Exception("当前表非空间表");
            }
            var sqlMaxObjectid = $"select max(objectid) from {layerName}";
            var maxObjectId = Database.ExecuteScalar(sqlMaxObjectid);
            int objectId = 0;
            if (maxObjectId != null)
            {
                objectId = (int)maxObjectId;
            }
            foreach (var feature in features)
            {
                Dictionary<string, object> value = new Dictionary<string, object>();
                string columnsStr = "";
                string columnsParams = "";
                if (feature.Attributes != null)
                {
                    foreach (var attr in feature.Attributes.GetNames())
                    {
                        if (feature.Attributes[attr] is DBNull) continue;
                        if (attr == "objectid")
                        {
                            value.Add(attr, ++objectId);
                        }
                        else
                        {
                            value.Add(attr, feature.Attributes[attr]);
                        }
                        columnsStr += attr + ",";
                        columnsParams += "@" + attr + ",";
                    }
                }
                columnsStr += "shape";
                columnsParams += "st_geomfromwkb(@shape)";
                var sql = $"insert into {layerName} ({columnsStr}) values ({columnsParams})";
                value.Add("shape", feature.Geometry.AsBinary());
                Database.Execute(sql, value);
            }
        }
    }
}
