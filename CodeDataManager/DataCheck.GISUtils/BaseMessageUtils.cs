using ServiceCenter.Core;
using System;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.GISUtils
{
    public class BaseMessageUtils
    {
        #region 
        protected BaseMessage Success(string msg = "成功")
        {
            return Success(null, msg, 1);
        }

        //protected BaseMessage<T> Success<T>(T obj, string msg = "成功", long total = 1)
        //{
        //    BaseMessage<T> rpcmsg = new BaseMessage<T>();
        //    rpcmsg.Status = 1;
        //    rpcmsg.Message = msg;
        //    rpcmsg.Item = obj;
        //    rpcmsg.Total = (int)total;
        //    return (rpcmsg);
        //}
        protected BaseMessage Success(Object obj, string msg = "成功", long total = 1)
        {
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 1;
            rpcmsg.Message = msg;
            rpcmsg.Item = obj;
            rpcmsg.Total = (int)total;
            return (rpcmsg);
        }

        //protected Task<BaseMessage<T>> SuccessAsyn<T>(T obj, string msg = "成功", long total = 1)
        //{
        //    BaseMessage<T> rpcmsg = new BaseMessage<T>();
        //    rpcmsg.Status = 1;
        //    rpcmsg.Message = msg;
        //    rpcmsg.Item = obj;
        //    rpcmsg.Total = (int)total;
        //    return Task.FromResult((rpcmsg));
        //}

        //protected Task<BaseMessage> SuccessAsyn(Object obj, string msg = "成功", int total = 1)
        //{
        //    BaseMessage rpcmsg = new BaseMessage();
        //    rpcmsg.Status = 1;
        //    rpcmsg.Message = msg;
        //    rpcmsg.Item = obj;
        //    rpcmsg.Total = total;

        //    return Task.FromResult((rpcmsg));

        //}


        protected BaseMessage Fail(string msg = "失败")
        {
            return Fail(null, msg);
        }

        protected BaseMessage Fail(Exception ex)
        {
            Toolset.TinyLogger.WriteLog(ex.Message, ex);
            BaseMessage result = new BaseMessage();
            result.Status = 0;
            result.Message = ex.Message;
            result.Item = false;
            return result;
        }

        protected BaseMessage Fail(Object obj, string msg = "失败", Exception ex = null)
        {
            if (ex != null)
            {
                Toolset.TinyLogger.WriteLog(ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source, ex);
            }
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg + (ex != null ? ("," + ex.Message) : "");
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;
            return (rpcmsg);
        }

        //protected BaseMessage<T> Fail<T>(T obj, string msg = "失败", Exception ex = null)
        //{
        //    if (ex != null)
        //    {
        //        Toolset.TinyLogger.WriteLog(ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source, ex);
        //    }
        //    BaseMessage<T> rpcmsg = new BaseMessage<T>();
        //    rpcmsg.Status = 0;
        //    rpcmsg.Message = msg + (ex != null ? ("," + ex.Message) : "");
        //    rpcmsg.Item = obj;
        //    rpcmsg.Total = 0;
        //    return (rpcmsg);
        //}

        //protected Task<BaseMessage<T>> FailAsyn<T>(T obj, string msg = "失败", Exception ex = null)
        //{
        //    if (ex != null)
        //    {
        //        Toolset.TinyLogger.WriteLog(ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source, ex);
        //    }
        //    BaseMessage<T> rpcmsg = new BaseMessage<T>();
        //    rpcmsg.Status = 0;
        //    rpcmsg.Message = msg + (ex != null ? ("," + ex.Message) : "");
        //    rpcmsg.Item = obj;
        //    rpcmsg.Total = 0;

        //    return Task.FromResult((rpcmsg));
        //}
        protected Task<BaseMessage> FailAsyn(Object obj, string msg = "失败", Exception ex = null)
        {
            if (ex != null)
            {
                Toolset.TinyLogger.WriteLog(ex.Message + " -- " + ex.StackTrace + " -- " + ex.Source, ex);
            }
            BaseMessage rpcmsg = new BaseMessage();
            rpcmsg.Status = 0;
            rpcmsg.Message = msg + (ex != null ? ("," + ex.Message) : "");
            rpcmsg.Item = obj;
            rpcmsg.Total = 0;

            return Task.FromResult((rpcmsg));
        }

        

        #endregion
    }
}
