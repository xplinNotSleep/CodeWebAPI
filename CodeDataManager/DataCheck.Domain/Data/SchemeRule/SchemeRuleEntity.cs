using System;
using ServiceCenter.Core.Data;

namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 方案规则表
    ///</summary>
    public class SchemeRuleEntity
    {

        /// <summary>
        /// ID
        /// </summary>
        public string Id {get;set;}
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SchemeName {get;set;}
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SchemeId {get;set;}
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName {get;set;}
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleId {get;set;}
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName {get;set;}
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string WhereClause {get;set;}
        /// <summary>
        /// 返回值字段
        /// </summary>
        public string BackFieldNames {get;set;}
        /// <summary>
        /// 质检唯一字段
        /// </summary>
        public string UniqueFieldName {get;set;}
        /// <summary>
        /// 唯一字段中文
        /// </summary>
        public string UniqueFieldCn {get;set;}
        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData { get; set; }
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName2 {get;set;}
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string WhereClause2 {get;set;}
        /// <summary>
        /// 返回值字段
        /// </summary>
        public string BackFieldNames2 {get;set;}
        /// <summary>
        /// 质检唯一字段
        /// </summary>
        public string UniqueFieldName2 {get;set;}
        /// <summary>
        /// 唯一字段中文
        /// </summary>
        public string UniqueFieldCn2 {get;set;}
        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData2 { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorDesc {get;set;}
        /// <summary>
        /// 字段1名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段1中文名
        /// </summary>
        public string FieldCN { get; set; }
        /// <summary>
        /// 字段2名称
        /// </summary>
        public string FieldName2 { get; set; }
        /// <summary>
        /// 字段2中文名
        /// </summary>
        public string FieldCN2 { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue {get;set;}
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue {get;set;}
        /// <summary>
        /// 参考值
        /// </summary>
        public string ParamValue {get;set;}
        /// <summary>
        /// 参考范围值
        /// </summary>
        public string ParamValues { get;set;}
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool StatusCode { get; set; } = true;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime {get;set;}
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime {get;set;}
    }

    /// <summary>
    /// 方案规则表参数
    ///</summary>
    public class SchemeRuleRequestParam :PageRequestParam
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id {get;set;}
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SchemeName {get;set;}
        /// <summary>
        /// 方案名称
        /// </summary>
        public string SchemeId {get;set;}
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName {get;set;}
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleId {get;set;}
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName {get;set;}
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string WhereClause {get;set;}
        /// <summary>
        /// 返回值字段
        /// </summary>
        public string BackFieldNames {get;set;}
        /// <summary>
        /// 质检唯一字段
        /// </summary>
        public string UniqueFieldName {get;set;}
        /// <summary>
        /// 唯一字段中文
        /// </summary>
        public string UniqueFieldCn {get;set;}
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName2 {get;set;}
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string WhereClause2 {get;set;}
        /// <summary>
        /// 返回值字段
        /// </summary>
        public string BackFieldNames2 {get;set;}
        /// <summary>
        /// 质检唯一字段
        /// </summary>
        public string UniqueFieldName2 {get;set;}
        /// <summary>
        /// 唯一字段中文
        /// </summary>
        public string UniqueFieldCn2 {get;set;}
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorDesc {get;set;}
        /// <summary>
        /// 最小值
        /// </summary>
        public double? MinValue {get;set;}
        /// <summary>
        /// 最大值
        /// </summary>
        public double? MaxValue {get;set;}
        /// <summary>
        /// 参考值
        /// </summary>
        public string ParamValue { get;set;}
        /// <summary>
        /// 参考范围值
        /// </summary>
        public string ParamValues { get;set;}
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? StatusCode {get;set;}
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime {get;set;}
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime {get;set;}
    }
}