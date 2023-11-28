using AGSpatialDataCheck.Domain;
using AGSpatialDataCheck.GISUtils;
using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using Pure.Ext;
using ServiceCenter.Core;
using ServiceCenter.Core.Data;
using ServiceCenter.Core.Excel;
using ServiceCenter.Core.NPOI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    /// <summary>
    /// Scheme控制器
    /// </summary>
    [Route("api/Scheme")]
    public class SchemeApiController : ApiControllerBase
    {
        /// <summary>
        /// 控制器内服务接口
        /// </summary>
        public SchemeService _SchemeService { get; private set; }
        public SchemeRuleService _SchemeRuleService { get; private set; }

        public static string BaseUrl { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="OSchemeService"></param>
        /// <param name="OSchemeRuleService"></param>
        public SchemeApiController(SchemeService OSchemeService, SchemeRuleService OSchemeRuleService)
        {
            _SchemeService = OSchemeService;
            _SchemeRuleService = OSchemeRuleService;
            BaseUrl = WebRequestHelper.GetHost(Toolset.HttpContext);
        }

        #region query
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<BaseMessage<List<SchemeEntity>>> GetList([FromQuery] SchemeRequestParam data)
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
                var result = await _SchemeService.GetPageDataAsync(param);
                return Success(result.Data, "成功", result.Total);
            }
            catch (Exception ex)
            {
                return Fail<List<SchemeEntity>>(null, ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Get/{Id}")]
        public async Task<BaseMessage<SchemeEntity>> Get(string Id)
        {
            try
            {
                SchemeEntity data = await _SchemeService.GetAsync(Id);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<SchemeEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<SchemeEntity>> Create([FromBody] SchemeEntity data)
        {
            try
            {
                var validResult = _SchemeService.Validate(data);
                if (validResult.IsValid == false)
                {
                    return Fail<SchemeEntity>(null, validResult.ToString());
                }
                data.CreateTime = DateTime.Now;
                data.UpdateTime = DateTime.Now;
                data = await _SchemeService.InsertAsync(data);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<SchemeEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<SchemeEntity>> Put([FromBody] JsonElement data)
        {
            try
            {
                SchemeEntity v = await _SchemeService.GetAsync(data.GetProperty("Id").GetString());
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _SchemeService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<SchemeEntity>(null, validResult.ToString());
                    }
                    v.UpdateTime = DateTime.Now;
                    await _SchemeService.UpdateAsync(v);
                    return Success(v);
                }
                return Fail<SchemeEntity>(null, "无法找到数据!");
            }
            catch (Exception ex)
            {
                return Fail<SchemeEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 部分更新数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("patch/{Id}")]
        public async Task<BaseMessage<SchemeEntity>> Patch(string Id, [FromBody] JsonElement data)
        {
            try
            {
                SchemeEntity v = await _SchemeService.GetAsync(Id);
                if (v != null)
                {
                    data.ApplyTo(v);
                    var validResult = _SchemeService.Validate(v);
                    if (validResult.IsValid == false)
                    {
                        return Fail<SchemeEntity>(null, validResult.ToString());
                    }
                    v.UpdateTime = DateTime.Now;
                    v = await _SchemeService.UpdateAsync(v);
                }
                return Success(v);
            }
            catch (Exception ex)
            {
                return Fail<SchemeEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 批量部分更新数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPatch("batch_patch/{Ids}")]
        public async Task<BaseMessage<int>> BatchUpdate(string Ids, [FromBody] JsonPatchDocument<SchemeEntity> data)
        {
            var nCount = 0;
            try
            {
                var arr = Ids.Split(',');
                List<Task> listTask = new List<Task>();
                foreach (var Id in arr)
                {
                    var v = await _SchemeService.GetAsync(Id);
                    if (v != null)
                    {
                        data.ApplyTo(v);
                        var validResult = _SchemeService.Validate(v);
                        if (validResult.IsValid == false)
                        {
                            Fail(0, validResult.ToString());
                        }
                        v.UpdateTime = DateTime.Now;
                        listTask.Add(_SchemeService.UpdateAsync(v));
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
                await _SchemeService.DeleteAsync(Id);
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
                    listTask.Add(_SchemeService.DeleteAsync(Id));
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
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "方案表.xls");
            return File(System.IO.File.ReadAllBytes(filepath), "application/vnd.ms-excel", "方案表.xls");
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
            dt.TableName = _SchemeService.TableName;
            System.IO.File.Delete(tmpFile);
            _SchemeService.DeleteAll();
            await _SchemeService.InsertBatchAsync(dt);
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
        public async Task<IActionResult> Export([FromQuery] SchemeRequestParam data)
        {
            if (data.Sort.IsNullOrEmpty())
            {
                data.Sort = "Id";
                data.Order = "";
            }
            var param = new DbQueryParameter(data);
            var result = await _SchemeService.GetDataAsync(param);
            IExcel excelService = new NPOIService();
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "方案表.xls");
            Dictionary<string, string> colummap = new Dictionary<string, string>();
            colummap.Add("scheme_name", "scheme_name");
            colummap.Add("status_code", "status_code");
            colummap.Add("create_time", "create_time");
            colummap.Add("update_time", "update_time");
            var stream = excelService.ListToXLSByModal(result, filepath, colummap, 0, 1, (columnName, value, wb, cell1) =>
            {
                var workbook = wb as IWorkbook;
                ICell cell = cell1 as ICell;
            }
            );
            return File(stream, "application/vnd.ms-excel", "方案表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }
        #endregion Export

        #region 自定义方法
        /// <summary>
        /// 开始质检
        /// </summary>
        /// <param name="SchemeId"></param>
        /// <returns></returns>
        [HttpPost("BeginZhiJian/{SchemeId}")]
        public async Task<BaseMessage> BeginZhiJian(string SchemeId)
        {
            try
            {
                var rules = await _SchemeRuleService.GetRulesBySchemeAsync(SchemeId);
                var options = new RequestOptions()
                {
                    ContentType = ContentTypes.JSON,
                    //5h
                    Timeout = 18000000
                };
                List<string> rets = new List<string>();
                foreach (var rule in rules)
                {
                    object obj = null;
                    if (rule.RuleType==ENUM_RULE_TYPE.多图层检查.GetHashCode())
                    {
                        var layer1 = new CheckLayer()
                        {
                            LayerPath = rule.LayerPath,
                            LayerType = rule.LayerType,
                            LayerName = rule.LayerName,
                            WhereClause = rule.WhereClause,
                            BackFieldNames = rule.BackFieldNames,
                            UniqueFieldName = rule.UniqueFieldName,
                            ExtraData = rule.ExtraData
                        };
                        var layer2 = new CheckLayer()
                        {
                            LayerPath = rule.LayerPath,
                            LayerType = rule.LayerType,
                            LayerName = rule.LayerName2,
                            WhereClause = rule.WhereClause2,
                            BackFieldNames = rule.BackFieldNames2,
                            UniqueFieldName = rule.UniqueFieldName2,
                            ExtraData = rule.ExtraData2
                        };
                        obj = new MultiSdeLayer { layer1 = layer1, layer2 = layer2 };
                    }
                    else
                    {
                        if (rule.RuleType != ENUM_RULE_TYPE.属性检查.GetHashCode())
                        {
                            obj = new CheckLayer()
                            {
                                LayerPath = rule.LayerPath,
                                LayerType = rule.LayerType,
                                LayerName = rule.LayerName,
                                WhereClause = rule.WhereClause,
                                BackFieldNames = rule.BackFieldNames,
                                UniqueFieldName = rule.UniqueFieldName,
                                ExtraData = rule.ExtraData
                            };
                        }
                        else
                        {
                            var layer = new CheckLayer()
                            {
                                LayerPath = rule.LayerPath,
                                LayerType = rule.LayerType,
                                LayerName = rule.LayerName,
                                WhereClause = rule.WhereClause,
                                BackFieldNames = rule.BackFieldNames,
                                UniqueFieldName = rule.UniqueFieldName,
                                ExtraData = rule.ExtraData
                            };
                            obj = new CheckField()
                            {
                                Layer = layer,
                                UniqueFieldName = rule.UniqueFieldName,
                                FieldName = rule.FieldName,
                                FieldCN = rule.FieldCN,
                                FieldName2 = rule.FieldName2,
                                FieldCN2 = rule.FieldCN2,
                                Value = rule.ParamValue,
                                Values = rule.ParamValues,
                                MinValue = rule.MinValue,
                                MaxValue = rule.MaxValue
                            };
                        }
                    }
                    var responseInfo = (BaseUrl + rule.RuleUrl).Post(obj, options);
                    var strret = responseInfo.GetResponseContent();
                    var BaseResponse = strret.FromJson<BaseMessage>();
                    if (BaseResponse!=null&&BaseResponse.Status != 1)
                    {
                        return BaseResponse;
                    }
                    else
                    {
                        rets.Add(BaseResponse.Message);
                    }
                }
                return Success();
            }
            catch (Exception ex)
            {
                return Fail(null, ex.Message, ex);
            }
        }
        #endregion 自定义方法
    }
}

