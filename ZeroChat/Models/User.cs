using System.Net.WebSockets;

namespace ZeroChat.Models
{
    public class User
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// WebSocket
        /// </summary>
        public WebSocket WebSocket { get; set; }

        //多端情况下使用
        ///// <summary>
        ///// Socket
        ///// </summary>
        //public Socket Socket { get; set; }
    }
}