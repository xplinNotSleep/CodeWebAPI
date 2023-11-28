using NetTopologySuite.IO.Postgis;

namespace AGSpatialDataCheck.GISUtils.CheckParams
{
    public class CheckLayer : SdeConnectstringParam
    {
      //  private string _LayerPath;
        /// <summary>
        /// 图层所在路径
        /// </summary>
       // public string LayerPath{ get { if (LayerType == "sde") return $"dbname={Databese} host={Host} port={Port} user={Username} password={Password}";  return _LayerPath; } set { _LayerPath = value; } }
        public string LayerPath
        {
            get; set;
        }
        /// <summary>
        /// sde、gdb、shp,默认sde数据库
        /// </summary>
        public string LayerType { get; set; }
        /// <summary>
        /// 图层名称
        /// </summary>LineStartField
        public string LayerName { get; set; }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string WhereClause { get; set; }
        /// <summary>
        /// 返回字段名称，返回越少字段越节省内存
        /// </summary>
        public string BackFieldNames { get; set; }
        /// <summary>
        /// 质检字段名
        /// </summary>
        public string UniqueFieldName { get; set; }

        /// <summary>
        /// 质检字段中文名
        /// </summary>
        //public string UniqueFieldCn { get; set; }

        ///// <summary>
        ///// 质检数据点号字段(一般是点数据)
        ///// </summary>
        //public string PointNumField { get; set; }

        ///// <summary>
        ///// 质检数据起点号字段(一般是线数据)
        ///// </summary>
        //public string LineStartField { get; set; }

        ///// <summary>
        ///// 质检数据终点号字段(一般是线数据)
        ///// </summary>
        //public string LineEndField { get; set; }

        /// <summary>
        /// 缓冲距离，用来扩大图形,例如线或者点
        /// </summary>
        public double BufferDistance { get; set; }

        ///// <summary>
        ///// 簇容差
        ///// </summary>
        //public double ClusterTolerance { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData { get; set; }
    }

    /// <summary>
    /// 两个图层
    /// </summary>
    public class MultiSdeLayer
    {
        /// <summary>
        /// 第一个图层
        /// </summary>

        public CheckLayer layer1 { get; set; }
        /// <summary>
        /// 第二个图层
        /// </summary>
        public CheckLayer layer2 { get; set; }
    }
}
