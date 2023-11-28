using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pure.Data.Validations.Results;
using ServiceCenter.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Base
{
    [Controller]
    public class BaseController : Controller
    {
        protected IActionResult ErrorView(string obj)
        {
            return Content(obj);
        }

        #region 通用消息格式
        private IActionResult ToActionResultJson(object obj)
        {
            return Json(obj);
        }
        public IActionResult Success(string msg = "成功")
        {
            return Success(null, msg, 1);
        }
        public IActionResult Success<T>(T obj, string msg = "成功", long total = 1)
        {
            BaseMessage<T> rpcmsg = new BaseMessage<T>();
            rpcmsg.Status = 1;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = (int)total;
            return ToActionResultJson(rpcmsg);
        }
        public IActionResult Success(object obj, string msg = "成功", long total = 1)
        {
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 1;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = (int)total;
            return ToActionResultJson(rpcmsg);
        }

        public Task<IActionResult> SuccessAsyn<T>(T obj, string msg = "成功", long total = 1)
        {
            BaseMessage<T> rpcmsg = new BaseMessage<T>();
            rpcmsg.Status = 1;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = (int)total;
            return Task.FromResult(ToActionResultJson(rpcmsg));
        }

        public Task<IActionResult> SuccessAsyn(object obj, string msg = "成功", int total = 1)
        {
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 1;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = total;

            return Task.FromResult(ToActionResultJson(rpcmsg));

        }

        public IActionResult Fail(string msg = "失败")
        {
            return Fail(null, msg);
        }
        public IActionResult Fail(ValidationResult o)
        {
            return Fail(null, o.ToString());
        }

        public IActionResult Fail<T>(T obj, string msg = "失败", Exception ex = null)
        {

            if (ex != null)
            {
                Toolset.Context.WriteLog(ex, ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source);
            }
            BaseMessage<T> rpcmsg = new BaseMessage<T>();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;
            return ToActionResultJson(rpcmsg);
        }
        public IActionResult Fail(object obj, string msg = "失败", Exception ex = null)
        {
            if (ex != null)
            {
                Toolset.Context.WriteLog(ex, msg + "\r\n" + ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source);
            }
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;
            return ToActionResultJson(rpcmsg);
        }

        public Task<IActionResult> FailAsyn<T>(T obj, string msg = "失败", Exception ex = null)
        {

            if (ex != null)
            {
                Toolset.Context.WriteLog(ex, ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source);
            }
            BaseMessage<T> rpcmsg = new BaseMessage<T>();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;

            return Task.FromResult(ToActionResultJson(rpcmsg));
        }
        public Task<IActionResult> FailAsyn(object obj, string msg = "失败", Exception ex = null)
        {
            if (ex != null)
            {
                Toolset.Context.WriteLog(ex, ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source);
            }
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;

            return Task.FromResult(ToActionResultJson(rpcmsg));
        }


        #endregion



        #region base methods

        public T GetRequestValue<T>(string name, T defaultValue)
        {
            //var data=  WebRequestHelper.RequestValue<T>(name, defaultValue, Toolset.HttpContext);
            var data = GetRequestValue(name);
            return data.To<T>(defaultValue);

        }
        public string GetRequestValue(string name)
        {
            var data = WebRequestHelper.RequestValue(name, HttpContext);

            if (data.IsNullOrEmpty() && RouteData.Values != null && RouteData.Values.Count > 0)
            {
                data = RouteData.Values[name] != null ? RouteData.Values[name].ToString() : "";
            }
            return data;
        }
        public Dictionary<string, object> GetQueryParamDictByStringArray(string[] arr)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string s in arr)
            {
                string value = GetRequestValue(s);
                dict.Add(s, value);
            }
            return dict;
        }
        public Dictionary<string, object> GetQueryParamDict()
        {
            //return GetQueryParamDictByString(HttpContext.Request.QueryString);
            Dictionary<string, object> dataDic = new Dictionary<string, object>();
            foreach (var item in HttpContext.Request.Query)
            {
                dataDic.Add(item.Key, item.Value.ToString());

            }
            return dataDic;
        }
        /// <summary>
        /// 将获取的formData存入字典数组
        /// </summary>
        public Dictionary<string, object> GetQueryParamDictByString(string formData)
        {
            try
            {
                //将参数存入字符数组
                string[] dataArry = formData.Split('&');

                //定义字典,将参数按照键值对存入字典中
                Dictionary<string, object> dataDic = new Dictionary<string, object>();
                //遍历字符数组
                for (int i = 0; i <= dataArry.Length - 1; i++)
                {
                    //当前参数值
                    string dataParm = dataArry[i];
                    //"="的索引值
                    int dIndex = dataParm.IndexOf("=");
                    //参数名作为key
                    string key = dataParm.Substring(0, dIndex);
                    //参数值作为Value
                    string value = dataParm.Substring(dIndex + 1, dataParm.Length - dIndex - 1);
                    //将编码后的Value解码
                    string deValue = System.Web.HttpUtility.UrlDecode(value, System.Text.Encoding.GetEncoding("utf-8"));
                    //将参数以键值对存入字典
                    dataDic.Add(key, deValue);
                }

                return dataDic;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Log(string str)
        {
            Toolset.TinyLogger.WriteLog(str);
        }
        public void LogError(string str, Exception ex)
        {
            Toolset.TinyLogger.WriteLog(str, ex);
        }

        public string MapPath(string path)
        {
            return Toolset.HttpContextWrappers.Server.MapPath(path);
        }
        public string FormatImportMsg(string msg, bool success = false)
        {
            if (success == true)
            {
                return "<div class='import-msg-item import-msg-item-success'>" + msg + "</div>";

            }
            else
            {
                return "<div class='import-msg-item import-msg-item-fail'>" + msg + "</div>";

            }

        }

        public bool IsDev()
        {
            return Toolset.Context.Configuration.EnvInfo.IsDev;
        }

        //public static void Set<T>(this ISession session, string key, T value)
        //{
        //    session.SetString(key, JsonConvert.SerializeObject(value));
        //}

        //public static T Get<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);
        //    return value == null ? default(T) :
        //                          JsonConvert.DeserializeObject<T>(value);
        //}
        #endregion
    }
}