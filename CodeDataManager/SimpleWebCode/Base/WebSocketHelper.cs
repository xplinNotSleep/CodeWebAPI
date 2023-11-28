using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using ServiceCenter.Core;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Base
{

    public class WebSocketConn
    {
        public WebSocket WebSocket { get; set; }
        public string UserName { get; set; }
    }

    public class WebSocketHelper
    {
        //static private log4net.ILog log = log4net.LogManager.GetLogger(typeof(WebSocketHelper));  //日志 没用就删了
        private static ConcurrentDictionary<string, WebSocketConn> _socketConnectUsers;
        WebSocket socket;
        WebSocketHelper(WebSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static async Task Acceptor(Microsoft.AspNetCore.Http.HttpContext httpContext, Func<Task> n)
        {
            if (!httpContext.WebSockets.IsWebSocketRequest)
                return;
            var socket = await httpContext.WebSockets.AcceptWebSocketAsync();

            var username = httpContext.Request.Query["username"];
            if (_socketConnectUsers == null)
            {
                _socketConnectUsers = new ConcurrentDictionary<string, WebSocketConn>();
            }
            WebSocketConn soc = null;
            if (!_socketConnectUsers.ContainsKey(username))
            {
                soc = new WebSocketConn();
                soc.WebSocket = socket;
                soc.UserName = username;
                _socketConnectUsers.TryAdd(username, soc);
                //Toolset.TinyLogger.WriteLog("connected websocket :" + username);
            }
            else
            {
                var kv = _socketConnectUsers.FirstOrDefault(p => p.Key == username);
                kv.Value.WebSocket = socket;
                soc = kv.Value;
            }
            if (soc == null) return;
            await RecvAsync(soc, CancellationToken.None);
        }

        /// <summary>
        /// 接收客户端数据
        /// </summary>
        /// <param name="soc">webSocket 对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> RecvAsync(WebSocketConn soc, CancellationToken cancellationToken)
        {
            //string oldRequestParam = "";
            try
            {
                WebSocketReceiveResult result;
                do
                {
                    var ms = new MemoryStream();
                    var buffer = new ArraySegment<byte>(new byte[1024 * 8]);
                    result = await soc.WebSocket.ReceiveAsync(buffer, cancellationToken);
                    if (result.CloseStatus.HasValue)
                    {
                        WebSocketConn dummy = null;
                        if (_socketConnectUsers.Count > 0)
                        {
                            _socketConnectUsers.TryRemove(soc.UserName, out dummy);
                            if (dummy != null)
                            {
                                await dummy.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", default);
                                dummy.WebSocket.Dispose();
                            }
                        }
                        break;
                    }
                    ms.Write(buffer.Array, buffer.Offset, result.Count - buffer.Offset);
                    ms.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(ms);
                    var s = reader.ReadToEnd();
                    reader.Dispose();
                    ms.Dispose();

                    if (!string.IsNullOrEmpty(s))
                    {
                        WSMessageModel message = JsonConvert.DeserializeObject<WSMessageModel>(s);
                        await SendAsync(message);
                    }
                } while (result.EndOfMessage);
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog("", ex);
            }
            return "";
        }
        /// <summary>
        /// 向客户端发送数据 
        /// </summary>
        /// <param name="message">数据</param>
        /// <returns></returns>
        public static async Task SendAsync(WSMessageModel message)
        {
            try
            {
                if (_socketConnectUsers == null)
                {
                    return;
                }
                //业务逻辑
                CancellationToken cancellation = default;
                var buf = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var segment = new ArraySegment<byte>(buf);
                WebSocket socket = null;
                if (message.receiverId == "")//广播
                {
                    foreach (var user in _socketConnectUsers)
                    {
                        socket = user.Value.WebSocket;
                        await socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
                    }
                }
                else
                {
                    socket = _socketConnectUsers.Values.FirstOrDefault(p => p.UserName == message.receiverId)?.WebSocket;
                    if (socket != null)
                    {
                        await socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
                    }
                }
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog("", ex);
            }
        }

        /// 路由绑定处理
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(Acceptor);
        }
    }

    /// <summary>
    /// 消息结果
    /// </summary>
    public class WSMessageModel
    {
        /// <summary>
        /// 发送者Id
        /// </summary>
        public string senderId { get; set; }

        /// <summary>
        /// 接受者id
        /// </summary>
        public string receiverId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public object content { get; set; }
    }
}
