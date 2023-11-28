using AGSpatialDataCheck.Domain;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ServiceCenter.Core;
using ServiceCenter.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    /// <summary>
    /// FileStorage控制器
    /// </summary>
    [Route("api/FileStorage")]
    public class FileStorageApiController : ApiControllerBase
    {
        /// <summary>
        /// 控制器内服务接口
        /// </summary>
        public FileStorageService _FileStorageService { get; private set; }
        /// <summary>
        /// 上传文件夹路径
        /// </summary>
        private string UploadPath;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="OFileStorageService"></param>
        public FileStorageApiController(FileStorageService OFileStorageService)
        {
            _FileStorageService = OFileStorageService;
            UploadPath = Path.Combine(Directory.GetCurrentDirectory(), "upload");
        }

        #region query
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<BaseMessage<List<FileStorageEntity>>> GetList([FromQuery] FileStorageRequestParam data)
        {
            try
            {
                if (data.PageIndex < 1)
                    data.PageIndex = 1;
                if (data.Sort.IsNullOrEmpty())
                {
                    data.Sort = "Id";
                    data.Order = "DESC";
                }
                var param = new DbQueryParameter(data);
                var result = await _FileStorageService.GetPageDataAsync(param);
                return Success(result.Data, "成功", result.Total);
            }
            catch (Exception ex)
            {
                return Fail<List<FileStorageEntity>>(null, ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Get/{Id}")]
        public async Task<BaseMessage<FileStorageEntity>> Get(string Id)
        {
            try
            {
                FileStorageEntity data = await _FileStorageService.GetAsync(Id);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<FileStorageEntity>(null, ex.Message, ex);
            }
        }
        #endregion query

        #region add
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<BaseMessage<FileStorageEntity>> Create([FromBody] FileStorageEntity data)
        {
            try
            {
                var validResult = _FileStorageService.Validate(data);
                if (validResult.IsValid == false)
                {
                    return Fail<FileStorageEntity>(null, validResult.ToString());
                }
                data.Createtime = DateTime.Now;

                data = await _FileStorageService.InsertAsync(data);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<FileStorageEntity>(null, ex.Message, ex);
            }
        }


        #endregion add


        #region update
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<BaseMessage<FileStorageEntity>> Put([FromBody] JsonElement data)
        {
            try
            {
                FileStorageEntity v = await _FileStorageService.GetAsync(data.GetProperty("Id").GetString());
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _FileStorageService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<FileStorageEntity>(null, validResult.ToString());
                    }

                    await _FileStorageService.UpdateAsync(v);
                    return Success(v);
                }
                return Fail<FileStorageEntity>(null, "无法找到数据!");
            }
            catch (Exception ex)
            {
                return Fail<FileStorageEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 部分更新数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("patch/{Id}")]
        public async Task<BaseMessage<FileStorageEntity>> Patch(string Id, [FromBody] JsonElement data)
        {
            try
            {
                FileStorageEntity v = await _FileStorageService.GetAsync(Id);
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _FileStorageService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<FileStorageEntity>(null, validResult.ToString());
                    }

                    v = await _FileStorageService.UpdateAsync(v);
                }
                return Success(v);
            }
            catch (Exception ex)
            {
                return Fail<FileStorageEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 批量部分更新数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("batch_patch/{Ids}")]
        public async Task<BaseMessage<int>> BatchUpdate(string Ids, [FromBody] JsonPatchDocument<FileStorageEntity> data)
        {
            var nCount = 0;
            try
            {
                var arr = Ids.Split(',');
                List<Task> listTask = new List<Task>();
                foreach (var Id in arr)
                {
                    var v = await _FileStorageService.GetAsync(Id);
                    if (v != null)
                    {
                        data.ApplyTo(v);
                        var validResult = _FileStorageService.Validate(v);
                        if (validResult.IsValid == false)
                        {
                            Fail(0, validResult.ToString());
                        }

                        listTask.Add(_FileStorageService.UpdateAsync(v));
                        nCount++;
                    };
                }
                await Task.WhenAll(listTask);
                return Success(nCount);
            }
            catch (Exception ex)
            {
                return Fail(nCount, ex.Message, ex);
            }
        }
        #endregion update


        #region delete
        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{Id}")]
        public async Task<BaseMessage<int>> Delete(string Id)
        {
            try
            {
                var file = await _FileStorageService.GetAsync(Id);
                DeleteFileCascade(file);
                await _FileStorageService.DeleteAsync(Id);
                return Success(1, "成功");
            }
            catch (Exception ex)
            {
                return Fail(0, ex.Message, ex);
            }
        }

        bool DeleteFileCascade(FileStorageEntity file)
        {
            if (file != null)
            {
                if (file.Secret.IsNullOrEmpty() || !_FileStorageService.Exists(p => p.Secret == file.Secret && p.Id != file.Id))
                {
                    string fileName = file.FilePath.Replace("\\", "/");
                    //暂时不再删除绑定文件本体
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                        var dir =Path.GetDirectoryName(fileName);
                        Directory.Delete(dir);
                    }

                }
                return true;
            }
            else if (file.Secret.IsNullOrEmpty())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据集合批量删除数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpDelete("batch_delete/{Ids}")]
        public async Task<BaseMessage<int>> BatchDelete(string Ids)
        {
            int nCount = 0;
            try
            {
                var arr = Ids.Split(',');
                foreach (var Id in arr)
                {
                    var file = await _FileStorageService.GetAsync(Id);
                    DeleteFileCascade(file);
                    await _FileStorageService.DeleteAsync(Id);
                    nCount++;
                }
                return Success(nCount, "成功");
            }
            catch (Exception ex)
            {
                return Fail(nCount, ex.Message, ex);
            }
        }
        #endregion delete

        #region 上传下载
        /// <summary>
        /// 单文件上传
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<BaseMessage<UploadResultData>> Upload([FromForm] FileStorageRequestParam model)
        {
            try
            {
                UploadResultData uploadresult = new UploadResultData();
                if (model.FileTree.IsNullOrEmpty())
                {
                    uploadresult.Status = 0;
                    uploadresult.Message = "文件树不能为空";
                    return Fail(uploadresult);
                }
                IFormFile fileForm = Request.Form.Files[0];
                var filename = fileForm.FileName;
                var filenameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                var fileext = Path.GetExtension(filename);

                string fileName = model.FileName;
                if (model.FileName.IsNullOrEmpty())
                {
                    fileName = fileForm.FileName;
                }
                var extName = Path.GetExtension(fileForm.FileName).ToLower();
                if (!fileName.ToLower().EndsWith(extName))
                {
                    fileName += extName;
                }

                if (fileName.IsNullOrEmpty())
                {
                    uploadresult.Status = 0;
                    uploadresult.Message = "文件名不能为空";
                    return Fail(uploadresult);
                }
                Toolset.TinyLogger.WriteLog("开始上传文件：" + fileName);
                using (var stream = fileForm.OpenReadStream())
                {
                    Byte[] buffer = null;
                    #region 文件大小
                    ulong FileSize = 0;//文件大小 B
                    if (Request.ContentType == "application/octet-stream" && Request.ContentLength > 0)
                    {
                        FileSize = Request.ContentLength.HasValue ? (ulong)Request.ContentLength.Value : 0;// Request.InputStream.Length;
                        buffer = new Byte[FileSize];
                        Request.Body.Read(buffer, 0, buffer.Length);
                    }
                    else if (Request.ContentType.Contains("multipart/form-data"))
                    {
                        FileSize = (ulong)stream.Length;
                        buffer = new Byte[FileSize];
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    #endregion
                    Byte[] arr = new Byte[500];
                    if (buffer.Length > 500)
                    {
                        Buffer.BlockCopy(buffer, 0, arr, 0, 300);
                        Buffer.BlockCopy(buffer, buffer.Length - 200, arr, 300, 200);
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, arr, 0, buffer.Length);
                    }
                    string secret = PasswordHelper.MD5(System.Text.Encoding.Default.GetString(arr) + buffer.Length + extName);
                    var path = "";
                    if (_FileStorageService.Exists(p => p.Secret == secret))
                    {
                        var oldfile = _FileStorageService.FirstOrDefault(p => p.Secret == secret);
                        path = oldfile.FilePath;
                        uploadresult.id = oldfile.Id;
                        uploadresult.filename = oldfile.FileName;
                        uploadresult.fileextension = oldfile.FileExtension;
                        uploadresult.filesize = oldfile.FileSize;
                        uploadresult.uploadFilePath = oldfile.FilePath;
                        uploadresult.Message = "已存在数据";
                    }
                    else
                    {
                        #region 为文件分配路径
                        var fileTreePath = Path.Combine(UploadPath, model.FileTree);
                        if (!Directory.Exists(fileTreePath))
                        {
                            Directory.CreateDirectory(fileTreePath);
                        }
                        path = Path.Combine(fileTreePath, Guid.NewGuid().ToString().Replace("-", "").Substring(8, 16));
                        Directory.CreateDirectory(path);
                        path = Path.Combine(path, fileForm.FileName);
                        using (var newfilestream = System.IO.File.Create(path))
                        {
                            if (stream.CanSeek)
                                stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(newfilestream);
                        }
                        #endregion

                        //写入数据库
                        string FileName = model.FileName;
                        if (model.FileName.IsNullOrEmpty())
                        {
                            FileName = fileForm.FileName;
                        }
                        if (!fileName.ToLower().EndsWith(extName))
                        {
                            fileName += extName;
                        }
                        FileStorageEntity file = new FileStorageEntity();
                        file.FileName = Path.GetFileNameWithoutExtension(fileName);
                        file.FileExtension = extName;
                        file.FileSize = FileSize;
                        file.Createtime = DateTime.Now;
                        file.Secret = secret;
                        file.FilePath = path;
                        file.FileTree = model.FileTree;

                        //写入数据库
                        file = await _FileStorageService.InsertAsync(file);

                        uploadresult.id = file.Id;
                        uploadresult.filesize = file.FileSize;
                        uploadresult.filename = file.FileName;
                        uploadresult.fileextension = file.FileExtension;
                    }
                }
                Toolset.TinyLogger.WriteLog("结束上传文件：" + fileName);

                return Success(uploadresult);
            }
            catch (Exception ex)
            {
                return Fail<UploadResultData>(null, ex.Message, ex);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("download/raw/{id}/{**path}")]
        public async Task<IActionResult> Download(string id, string path)
        {
            var model = _FileStorageService.Get(id);
            if (model == null)
            {
                return StatusCode(410);
            }
            string filePath = model.FilePath;
            if (path != null)
            {
                if (Path.GetFileName(filePath) != path)
                {
                    var filename = Path.GetFileName(filePath);
                    if (filename.Contains("."))
                    {
                        filePath = Path.Combine(Path.GetDirectoryName(filePath), path);
                    }
                    else
                    {
                        filePath = Path.Combine(filePath, path);
                    }
                }
            }
            if (!System.IO.File.Exists(filePath))
            {
                return StatusCode(410);
            }
            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", Path.GetFileNameWithoutExtension(filePath) + "_" + PasswordHelper.EncodeHex62(DateTime.Now.ToString("yyyyMMddHHmmss").ToLong()) + Path.GetExtension(filePath));
        }
        #endregion

        #region 自定义方法

        #endregion 自定义方法
    }
}
