using AGSpatialDataCheck.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceCenter.Core.Data;
using AGSpatialDataCheck.Web.Base;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    /// <summary>
    /// SchemeRule控制器
    /// </summary>
    [Route("api/SchemeRule")]
    public class SchemeRuleApiController : ApiControllerBase
    {
        /// <summary>
        /// 控制器内服务接口
        /// </summary>
        public SchemeRuleService _SchemeRuleService { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="OSchemeRuleService"></param>
        public SchemeRuleApiController(SchemeRuleService OSchemeRuleService)
        {
            _SchemeRuleService = OSchemeRuleService;
        }

		#region query
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<BaseMessage<List<SchemeRuleEntity>>> GetList([FromQuery]SchemeRuleRequestParam data)
        {
            try
            {
                if (data.PageIndex < 1)
                    data.PageIndex = 1;
                if(data.Sort.IsNullOrEmpty())
                {
                    data.Sort ="update_time";
                    data.Order = "DESC";
                }
                var param = new DbQueryParameter(data);
				var result =await _SchemeRuleService.GetPageDataAsync(param);
                return Success(result.Data, "成功", result.Total);
            }
            catch (Exception ex)
            {
                return Fail<List<SchemeRuleEntity>>(null, ex.Message, ex);
            }
        }

		 /// <summary>
		 /// 根据主键获取数据
		 /// </summary>
		 /// <param name="Id"></param>
		 /// <returns></returns>
		 [HttpGet("Get/{Id}")]
		 public async Task<BaseMessage<SchemeRuleEntity>> Get(string Id)
		 {
		     try
		     {
		         SchemeRuleEntity data =await _SchemeRuleService.GetAsync(Id); 
		         return Success(data);
		     }
		     catch (Exception ex)
		     {
		         return Fail<SchemeRuleEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<SchemeRuleEntity>> Create([FromBody] SchemeRuleEntity data)
        {
            try
            {
				var validResult = _SchemeRuleService.Validate(data);
                if (validResult.IsValid == false)
                {
                    return Fail<SchemeRuleEntity>(null, validResult.ToString());
                }
                data.CreateTime=DateTime.Now;
                data.UpdateTime=DateTime.Now;
                data =await _SchemeRuleService.InsertAsync(data);
                return Success(data);
            }
            catch (Exception ex)
            {
                return Fail<SchemeRuleEntity>(null, ex.Message, ex);
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
        public async Task<BaseMessage<SchemeRuleEntity>> Put([FromBody] JsonElement data)
        {
            try
            {
                SchemeRuleEntity v = await _SchemeRuleService.GetAsync(data.GetProperty("Id").GetString());
                if(v!=null)
                {
                    data.ApplyTo(v);
	  			  var validResult = _SchemeRuleService.Validate(v);
	  		      if (validResult.IsValid == false)
	  			  {
	  				  return Fail<SchemeRuleEntity>(null, validResult.ToString());
	  			  }
                    v.UpdateTime=DateTime.Now;
                    await _SchemeRuleService.UpdateAsync(v);
	  			  return Success(v);
                }
                return Fail<SchemeRuleEntity>(null, "无法找到数据!");
            }
            catch (Exception ex)
            {
                return Fail<SchemeRuleEntity>(null, ex.Message, ex);
            }
        }
        /// <summary>
        /// 部分更新数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
       [HttpPatch("patch/{Id}")]
       public async Task<BaseMessage<SchemeRuleEntity>> Patch(string Id, [FromBody] JsonElement data)
       {
           try
           {
               SchemeRuleEntity v = await _SchemeRuleService.GetAsync(Id);
               if (v != null)
               {
                   data.ApplyTo(v);
                   var validResult = _SchemeRuleService.Validate(v);
                   if (validResult.IsValid == false)
                   {
                       return Fail<SchemeRuleEntity>(null, validResult.ToString());
                   }
                    v.UpdateTime=DateTime.Now;
                   v = await _SchemeRuleService.UpdateAsync(v);
               }
               return Success(v);
           }
           catch (Exception ex)
           {
               return Fail<SchemeRuleEntity>(null, ex.Message, ex);
           }
       }
	     /// <summary>
	     /// 批量部分更新数据
	     /// </summary>
	     /// <param name="Ids"></param>
	     /// <param name="data"></param>
	     /// <returns></returns>
        [HttpPatch("batch_patch/{Ids}")]
        public async Task<BaseMessage<int>> BatchUpdate(string Ids, [FromBody] JsonPatchDocument<SchemeRuleEntity> data)
        {
            var nCount = 0;
            try
            {
                var arr = Ids.Split(',');
	  		  List<Task> listTask = new List<Task>();
                foreach (var Id in arr)
                {
                    var v =await _SchemeRuleService.GetAsync(Id);
                    if (v != null)
                    {
                        data.ApplyTo(v);
                        var validResult = _SchemeRuleService.Validate(v);
                        if (validResult.IsValid == false)
                        {
                            Fail(0, validResult.ToString());
                        }
                    v.UpdateTime=DateTime.Now;
                        listTask.Add(_SchemeRuleService.UpdateAsync(v));
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
                await _SchemeRuleService.DeleteAsync(Id);
                return Success(1,"成功");
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
               foreach(var Id in arr)
               {
                   listTask.Add(_SchemeRuleService.DeleteAsync(Id));
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



	  	#region 自定义方法

        #endregion 自定义方法
    }
}
