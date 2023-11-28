using System;
using ServiceCenter.Core.Data;

namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 规则表
    ///</summary>
    public class RuleEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
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
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorDesc { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool StatusCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } 
    }

    /// <summary>
    /// 规则表参数
    ///</summary>
    public class RuleRequestParam :PageRequestParam
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id {get;set;}
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName {get;set;}
        /// <summary>
        /// 规则对应请求
        /// </summary>
        public string RuleUrl {get;set;}
        /// <summary>
        /// 规则类别
        /// </summary>
        public int RuleType {get;set;}
        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description {get;set;}
        /// <summary>
        /// 权重
        /// </summary>
        public int? Weight {get;set;}
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? StatusCode {get;set;}
    }
}