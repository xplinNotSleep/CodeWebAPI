using Pure.Data;
using System;

namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 规则表
    ///</summary>
    public class RuleMapper : ClassMapper<RuleEntity>
    {
        public RuleMapper()
        {
            Table("a_rule");
            Description("规则表");
            Map(m => m.Id).Column("id").Key(KeyType.Assigned).Description("主键").Size(50);
            Map(m => m.RuleName).Column("rule_name").Description("规则名称").Size(255);
            Map(m => m.RuleUrl).Column("rule_url").Description("规则对应请求").Size(255);
            Map(m => m.RuleType).Column("rule_type").Description("规则类别");
            Map(m => m.Description).Column("description").Description("规则描述").Size(255);
            Map(m => m.Weight).Column("weight").Description("权重");
            Map(m => m.ErrorDesc).Column("error_desc").Description("错误描述").Size(255);
            Map(m => m.StatusCode).Column("status_code").Description("是否启用");
            Map(m => m.CreateTime).Column("create_time").Description("创建时间");
            Map(m => m.UpdateTime).Column("update_time").Description("修改时间");
            AutoMap();
        }
    }
}