using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ZeroChat.Models
{
    public class UserSocket
    {
        public string WebSocketConnectionId { get; set; }
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
