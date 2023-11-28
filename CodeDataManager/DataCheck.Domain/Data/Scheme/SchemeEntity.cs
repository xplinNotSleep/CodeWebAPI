using System;
using ServiceCenter.Core.Data;

namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 方案表
    ///</summary>
    public class SchemeEntity
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
        /// 图层路径
        /// </summary>
        public string LayerPath { get; set; }
        /// <summary>
        /// sde、gdb、shp,默认sde数据库
        /// </summary>
        public string LayerType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime {get;set;}
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime {get;set;}
    }

    /// <summary>
    /// 方案表参数
    ///</summary>
    public class SchemeRequestParam :PageRequestParam
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
        /// 图层路径
        /// </summary>
        public string LayerPath { get; set; }
        /// <summary>
        /// sde、gdb、shp,默认sde数据库
        /// </summary>
        public string LayerType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime {get;set;}
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateTime {get;set;}
    }
}