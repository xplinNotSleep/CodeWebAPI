namespace AGSpatialDataCheck.Domain.Dto
{
    public class TaskDto:SchemeRuleEntity
    {
        /// <summary>
        /// 图层路径
        /// </summary>
        public string LayerPath { get; set; }
        /// <summary>
        /// sde、gdb、shp,默认sde数据库
        /// </summary>
        public string LayerType { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 规则对应请求
        /// </summary>
        public string RuleUrl { get; set; }
        /// <summary>
        /// 规则类别
        /// </summary>
        public int RuleType { get; set; }
        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }
    }
}
