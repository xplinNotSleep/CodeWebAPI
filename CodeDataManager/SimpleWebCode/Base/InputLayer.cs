using System;

namespace AGSpatialDataCheck.Web.Base
{
    public class InputLayer
    {
        /// <summary>
        /// 图层所在路径
        /// </summary>
        public string LayerPath { get; set; } = "";

        /// <summary>
        /// shp、gdb、sde
        /// </summary>
        public string LayerType { get; set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        public string FeatureClassName { get; set; }

        /// <summary>
        /// 过滤条件,默认为"",表示检查所有记录
        /// </summary>
        public string WhereClause { get; set; } = "";

        /// <summary>
        /// 返回字段名称，返回越少字段越节省内存
        /// </summary>
        public string BackFieldNames { get; set; } = "";

        /// <summary>
        /// 缓冲距离，用来扩大图形,例如线或者点
        /// </summary>
        public double BufferDistance { get; set; }

        /// <summary>
        /// 簇容差
        /// </summary>
        public double ClusterTolerance { get; set; }

        /// <summary>
        /// 质检字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 质检字段中文名,用来显示
        /// </summary>
        public string FieldOtherName { get; set; }

        /// <summary>
        /// 规则错误描述
        /// </summary>
        public string ErrorDesc { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; }
    }

    /// <summary>
    /// 两个图层
    /// </summary>
    public class MultiLayer
    {
        /// <summary>
        /// 第一个图层
        /// </summary>
        public InputLayer layer { get; set; }
        /// <summary>
        /// 第二个图层
        /// </summary>
        public InputLayer layer2 { get; set; }
    }
}
