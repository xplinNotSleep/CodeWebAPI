using Pure.Data;
namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 方案表
    ///</summary>
    public class SchemeMapper : ClassMapper<SchemeEntity>
    {
        public SchemeMapper()
        {
            Table("a_scheme");
            Description("方案表");
            Map(m => m.Id).Column("id").Key(KeyType.Assigned).Description("ID").Size(50);
            Map(m => m.SchemeName).Column("scheme_name").Description("方案名称").Size(255);
            Map(m => m.LayerPath).Column("layer_path").Description("图层路径").Size(255);
            Map(m => m.LayerType).Column("layer_type").Description("图层类型").Size(20);
            Map(m => m.CreateTime).Column("create_time").Description("创建时间");
            Map(m => m.UpdateTime).Column("update_time").Description("修改时间");
            AutoMap();
        }
    }
}