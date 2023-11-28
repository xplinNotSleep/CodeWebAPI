namespace AgDataHandle.Domain
{
    /// <summary>
    /// 文件上传结果数据
    /// </summary>
    public class UploadResultData
    {
        /// <summary>
        /// 成功状态
        /// </summary>
        public int Status { get; set; } = 1;
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; } = "成功";
        /// <summary>
        /// 文件唯一标识码
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string filename { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string fileextension { get; set; }

        /// <summary>
        /// 长度（字节）
        /// </summary>
        public ulong filesize { get; set; }
        /// <summary>
        /// 上传的文件所在的路径
        /// </summary>
        public string uploadFilePath { get; set; }
    }
}
