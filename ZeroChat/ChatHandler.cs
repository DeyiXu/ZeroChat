using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroChat
{
    public static class ChatHandler
    {
        // 消息集合
        public const int BUFFER_SIZE = 65535;

        #region delegate
        /// <summary>
        /// 客户端关闭
        /// </summary>
        public static Action<string> ClientClose;
        /// <summary>
        /// 客户端消息
        /// </summary>
        public static Action<string, string> ClientMessage;
        /// <summary>
        /// 客户端连接
        /// </summary>
        public static Action<string> ClientConnection;
        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public static void Send(string message)
        {

        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();

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
    }
}
