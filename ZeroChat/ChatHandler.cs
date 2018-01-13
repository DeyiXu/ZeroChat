using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroChat
{
    public static class ChatHandler
    {
        // 消息集合
        public const int BUFFER_SIZE = 65535;

        #region delegate
        /// <summary>
        /// 客户端关闭 message
        /// </summary>
        public static Action<string> ClientClose;
        /// <summary>
        /// 客户端消息 conectionId,message
        /// </summary>
        public static Action<string, string> ClientMessage;
        /// <summary>
        /// 客户端连接 conectionId,webSocket
        /// </summary>
        public static Action<string, WebSocket> ClientConnection;
        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public static void Send(string message, params WebSocket[] webSockets)
        {
            byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in webSockets)
            {
                client.SendAsync(msgBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task Acceptor(HttpContext context, Func<Task> next)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var client = await context.WebSockets.AcceptWebSocketAsync();
            if (client.State == WebSocketState.Open)
            {
                string connectionId = context.Connection.GetConnectionId();
                //执行客户端连接回掉函数
                ClientConnection?.Invoke(connectionId, client);
            }
            await Echo(context, client);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        static async Task Echo(HttpContext context, WebSocket webSocket)
        {
            string connectionId = GetConnectionId(context.Connection);
            var buffer = new byte[BUFFER_SIZE];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                //回掉函数
                ClientMessage?.Invoke(connectionId, message);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            //执行客户端关闭回掉函数
            ClientClose?.Invoke(connectionId);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        /// <summary>
        /// 为这个ChatHandler使用的请求管道
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(Acceptor);
        }

        /// <summary>
        /// 获取WebSocketId
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static string GetConnectionId(this ConnectionInfo connectionInfo)
        {
            return $"{connectionInfo.Id}-{connectionInfo.RemoteIpAddress.ToString()}-{connectionInfo.RemotePort}";
        }
    }
}
