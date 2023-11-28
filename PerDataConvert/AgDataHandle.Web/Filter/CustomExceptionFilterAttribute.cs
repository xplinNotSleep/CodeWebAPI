using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using ServiceCenter.Core;

namespace AgDataHandle.Web
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IConfiguration _configuration;

        public CustomExceptionFilterAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void OnException(ExceptionContext context)
        {
            #region 捕获程序异常，友好提示
            //ResponseDto response = new ResponseDto()
            //{
            //    Success = false,
            //    Message = "服务器忙，请稍后再试"
            //};
            BaseMessage response = new BaseMessage()
            {
                Message = "服务异常，原因：" + context.Exception.Message,
                Status = 0
            };
            if (_configuration["AppSetting:DisplayExceptionOrNot"] == "1")
            {
                response.Message = context.Exception.Message;
            }
            Toolset.TinyLogger.WriteLog("访问Url:" + context.HttpContext.Request.GetUrl() + " ,出错：" +  context.Exception.ToFriendlyString());
            context.Result = new JsonResult(response);
            #endregion
        }
    }
}