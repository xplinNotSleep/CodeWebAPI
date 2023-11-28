using AGSpatialDataCheck.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceCenter.Core.Data;
using System.IO;
using ServiceCenter.Core.NPOI;
using ServiceCenter.Core.Excel;
using ServiceCenter.Core;
using NPOI.SS.UserModel;
using AGSpatialDataCheck.Web.Base;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    /// <summary>
    /// Rule控制器
    /// </summary>
    [Route("api/Rule")]
    public class RuleApiController : ApiControllerBase
    {
        /// <summary>
        /// 控制器内服务接口
        /// </summary>
        public RuleService _RuleService { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ORuleService"></param>
        public RuleApiController(RuleService ORuleService)
        {
            _RuleService = ORuleService;
        }

        #region query
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<BaseMessage<List<RuleEntity>>> GetList([FromQuery] RuleRequestParam data)
        {
            try
            {
                if (data.PageIndex < 1)
                    data.PageIndex = 1;
                if (data.Sort.IsNullOrEmpty())
                {
                    data.Sort = "update_time";
                    data.Order = "DESC";
                }
                var param = new DbQueryParameter(data);
                var result = await _RuleService.GetPageDataAsync(param);
                return Success(result.Data, "成功", result.Total);
            }
            catch (Exception ex)
            {
                return Fail<List<RuleEntity>>(null, ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Get/{Id}")]
        public async Task<BaseMessage<RuleEntity>> Get(string Id)
        {
            try
            {
                RuleEntity data = await _RuleService.GetAsync(Id);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<RuleEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<RuleEntity>> Create([FromBody] RuleEntity data)
        {
            try
            {
                var validResult = _RuleService.Validate(data);
                if (validResult.IsValid == false)
                {
                    return Fail<RuleEntity>(null, validResult.ToString());
                }
                data.CreateTime = DateTime.Now;
                data.UpdateTime = DateTime.Now;
                data = await _RuleService.InsertAsync(data);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<RuleEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<RuleEntity>> Put([FromBody] JsonElement data)
        {
            try
            {
                RuleEntity v = await _RuleService.GetAsync(data.GetProperty("Id").GetString());
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _RuleService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<RuleEntity>(null, validResult.ToString());
                    }
                    v.UpdateTime = DateTime.Now;
                    await _RuleService.UpdateAsync(v);
                    return Success(v);
                }
                return Fail<RuleEntity>(null, "无法找到数据!");
            }
            catch (Exception ex)
            {
                return Fail<RuleEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 部分更新数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("patch/{Id}")]
        public async Task<BaseMessage<RuleEntity>> Patch(string Id, [FromBody] JsonElement data)
        {
            try
            {
                RuleEntity v = await _RuleService.GetAsync(Id);
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _RuleService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<RuleEntity>(null, validResult.ToString());
                    }
                    v.UpdateTime = DateTime.Now;
                    v = await _RuleService.UpdateAsync(v);
                }
                return Success(v);
            }
            catch (Exception ex)
            {
                return Fail<RuleEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 批量部分更新数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("batch_patch/{Ids}")]
        public async Task<BaseMessage<int>> BatchUpdate(string Ids, [FromBody] JsonPatchDocument<RuleEntity> data)
        {
            var nCount = 0;
            try
            {
                var arr = Ids.Split(',');
                List<Task> listTask = new List<Task>();
                foreach (var Id in arr)
                {
                    var v = await _RuleService.GetAsync(Id);
                    if (v != null)
                    {
                        data.ApplyTo(v);
                        var validResult = _RuleService.Validate(v);
                        if (validResult.IsValid == false)
                        {
                            Fail(0, validResult.ToString());
                        }
                        v.UpdateTime = DateTime.Now;
                        listTask.Add(_RuleService.UpdateAsync(v));
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
                await _RuleService.DeleteAsync(Id);
                return Success(1, "成功");
            }
            catch (Exception ex)
            {
                return Fail(0, ex.Message, ex);
            }
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
                List<Task> listTask = new List<Task>();
                foreach (var Id in arr)
                {
                    listTask.Add(_RuleService.DeleteAsync(Id));
                    nCount++;
                }
                await Task.WhenAll(listTask);
                return Success(nCount, "成功");
            }
            catch (Exception ex)
            {
                return Fail(nCount, ex.Message, ex);
            }
        }
        #endregion delete

        #region import
        /// <summary>
        /// 根据导入模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettemplate")]
        public async Task<IActionResult> GetTemplate()
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "规则表.xlsx");
            return File(System.IO.File.ReadAllBytes(filepath), "application/vnd.ms-excel", "规则表.xlsx");
        }
        /// <summary>
        /// 导入模板数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("import")]
        public async Task<BaseMessage> Import()
        {
            if (Request.Form.Files.Count == 0)
                return Fail();
            var baseDir = Directory.GetCurrentDirectory();
            var excelFile = Request.Form.Files[0].FileName;
            var uploadDir = Path.Combine(baseDir, "upload");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }
            var tmpFile = Path.Combine(uploadDir, Guid.NewGuid().ToString() + Path.GetExtension(excelFile));
            foreach (var fileForm in Request.Form.Files)
            {
                using (var stream = fileForm.OpenReadStream())
                {
                    using (var newfilestream = System.IO.File.Create(tmpFile))
                    {
                        if (stream.CanSeek)
                            stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(newfilestream);
                    }
                }
            }
            IExcel excelService = new NPOIService();
            var dt = excelService.XlSToDataTable(tmpFile);
            dt.TableName = _RuleService.TableName;
            System.IO.File.Delete(tmpFile);
            _RuleService.DeleteAll();
            await _RuleService.InsertBatchAsync(dt);
            return Success();
        }
        #endregion import

        #region Export
        /// <summary>
        /// 导出表格数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] RuleRequestParam data)
        {
            if (data.Sort.IsNullOrEmpty())
            {
                data.Sort = "Id";
                data.Order = "";
            }
            var param = new DbQueryParameter(data);
            var result = await _RuleService.GetDataAsync(param);
            IExcel excelService = new NPOIService();
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "规则表.xlsx");
            Dictionary<string, string> colummap = new Dictionary<string, string>();
            colummap.Add("id", "id");
            colummap.Add("RuleName", "rule_name");
            colummap.Add("RuleUrl", "rule_url");
            colummap.Add("RuleType", "rule_type");
            colummap.Add("Description", "description");
            colummap.Add("Weight", "weight");
            colummap.Add("StatusCode", "status_code");
            var stream = excelService.ListToXLSByModal(result, filepath, colummap, 0, 1, (columnName, value, wb, cell1) =>
            {
                var workbook = wb as IWorkbook;
                ICell cell = cell1 as ICell;
            }
            );
            return File(stream, "application/vnd.ms-excel", "规则表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }
        #endregion Export

        #region 自定义方法

        #endregion 自定义方法
    }
}
