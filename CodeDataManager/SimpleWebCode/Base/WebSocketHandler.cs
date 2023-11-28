using ServiceCenter.Core;
using ServiceCenter.Core.Caching;
using System;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Base
{
    public class WebSocketHandler : Singleton<WebSocketHandler>
    {
        private ICache cache = null;
        WebSocketHandler()
        {
            cache = Toolset.CacheManager.GetCacherByProviderType("WebSocketHandler", CacheProviderType.Runtime);
        }

        private WSMessageModel CreateWebSocketDto()
        {
            WSMessageModel message = new WSMessageModel();
            message.senderId = "admin";
            message.receiverId = "admin";
            message.messageType = "text";
            return message;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <returns></returns>
        public async Task SendRefresh()
        {
            try
            {
                WSMessageModel message = CreateWebSocketDto();
                message.action = "refresh";
                await WebSocketHelper.SendAsync(message);
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog("SendLoginData", ex);
            }
        }

    }
}
