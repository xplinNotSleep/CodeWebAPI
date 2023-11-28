using Pure.Data;
namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 方案规则表
    ///</summary>
    public class SchemeRuleMapper : ClassMapper<SchemeRuleEntity>
    {
        public SchemeRuleMapper()
        {
            Table("a_scheme_rule");
            Description("方案规则表");
            Map(m => m.Id).Column("id").Key(KeyType.Assigned).Description("ID").Size(50);
            Map(m => m.SchemeName).Column("scheme_name").Description("方案名称").Size(50);
            Map(m => m.SchemeId).Column("scheme_id").Description("方案名称").Size(50);
            Map(m => m.RuleName).Column("rule_name").Description("规则名称").Size(50);
            Map(m => m.RuleId).Column("rule_id").Description("规则名称").Size(50);
            Map(m => m.LayerName).Column("layer_name").Description("图层名称").Size(50);
            Map(m => m.WhereClause).Column("where_clause").Description("过滤条件").Size(255);
            Map(m => m.BackFieldNames).Column("back_field_names").Description("返回值字段").Size(255);
            Map(m => m.UniqueFieldName).Column("unique_field_name").Description("质检唯一字段").Size(50);
            Map(m => m.UniqueFieldCn).Column("unique_field_cn").Description("唯一字段中文").Size(50);
            Map(m => m.LayerName2).Column("layer_name2").Description("图层名称").Size(50);
            Map(m => m.WhereClause2).Column("where_clause2").Description("过滤条件").Size(255);
            Map(m => m.BackFieldNames2).Column("back_field_names2").Description("返回值字段").Size(255);
            Map(m => m.UniqueFieldName2).Column("unique_field_name2").Description("质检唯一字段").Size(50);
            Map(m => m.UniqueFieldCn2).Column("unique_field_cn2").Description("唯一字段中文").Size(50);
            Map(m => m.ErrorDesc).Column("error_desc").Description("错误描述").Size(255);
            Map(m => m.MinValue).Column("min_value").Description("最小值");
            Map(m => m.MaxValue).Column("max_value").Description("最大值");
            Map(m => m.ParamValue).Column("param_value").Description("参考值").Size(255);
            Map(m => m.ParamValues).Column("param_values").Description("参考范围值").Size(255);
            Map(m => m.StatusCode).Column("status_code").Description("是否启用");
            Map(m => m.CreateTime).Column("create_time").Description("创建时间");
            Map(m => m.UpdateTime).Column("update_time").Description("更新时间");
            AutoMap();
        }
    }
}