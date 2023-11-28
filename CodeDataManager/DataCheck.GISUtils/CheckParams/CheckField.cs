namespace AGSpatialDataCheck.GISUtils.CheckParams
{
    public class CheckField
    {
        /// <summary>
        /// 表一
        /// </summary>
        public CheckLayer Layer { get; set; }

        /// <summary>
        /// 字段1名称
        /// </summary>
        public string UniqueFieldName { get; set; }

        /// <summary>
        /// 字段1名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段1中文名
        /// </summary>
        public string FieldCN { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName2 { get; set; }

        /// <summary>
        /// 质检字段中文名2
        /// </summary>
        public string FieldCN2 { get; set; }

        /// <summary>
        /// 质检的多个字段
        /// </summary>
        //public string FieldNames { get; set; }

        /// <summary>
        /// 参数范围值
        /// </summary>
        public string Values { get; set; }

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
}
