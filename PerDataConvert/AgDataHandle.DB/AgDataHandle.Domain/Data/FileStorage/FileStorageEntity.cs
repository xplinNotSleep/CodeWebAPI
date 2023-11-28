using System;
using ServiceCenter.Core.Data;

namespace AgDataHandle.Domain
{
    /// <summary>
    /// 文件管理表
    ///</summary>
    public class FileStorageEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        public string Id {get;set;}
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName {get;set;}
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath {get;set;}
        /// <summary>
        /// 文件大小
        /// </summary>
        public ulong FileSize {get;set;}
        /// <summary>
        /// 扩展名
        /// </summary>
        public string FileExtension {get;set;}
        /// <summary>
        /// 文件数类型
        /// </summary>
        public string FileTree { get; set; }
        /// <summary>
        /// 文件排重码
        /// </summary>
        public string Secret {get;set;}
        /// <summary>
        /// 状态
        /// </summary>
        public int StatusCode {get;set;}
        /// <summary>
        /// 上传人
        /// </summary>
        public string Createuser {get;set;}
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createtime {get;set;}
    }

    /// <summary>
    /// 文件管理表参数
    ///</summary>
    public class FileStorageRequestParam :PageRequestParam
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName {get;set;}
        /// <summary>
        /// 文件数类型
        /// </summary>
        //public string FileTree { get; set; }
    }

    public class FileStorageUploadRequestParam
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件数类型
        /// </summary>
        //public string FileTree { get; set; }
    }
}