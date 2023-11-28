using Pure.Data;
namespace AgDataHandle.Domain
{
    /// <summary>
    /// 文件管理表
    ///</summary>
    public class FileStorageMapper : ClassMapper<FileStorageEntity>
    {
        public FileStorageMapper()
        {
            Table("A_File_Storage");
            Description("文件管理表");
            Map(m => m.Id).Column("id").Key(KeyType.Assigned).Description("主键");
            Map(m => m.FileName).Column("filename").Description("文件名");
            Map(m => m.FilePath).Column("filepath").Description("文件路径");
            Map(m => m.FileSize).Column("filesize").Description("文件大小");
            Map(m => m.FileExtension).Column("fileextension").Description("扩展名");
            Map(m => m.FileTree).Column("filetree").Description("文件树");
            Map(m => m.Secret).Column("secret").Description("文件排重码");
            Map(m => m.StatusCode).Column("statuscode").Description("状态");
            Map(m => m.Createuser).Column("createuser").Description("上传人");
            Map(m => m.Createtime).Column("createtime").Description("创建时间");
            AutoMap();
        }
    }
}