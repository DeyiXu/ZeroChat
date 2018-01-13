using System.Net.WebSockets;

namespace ZeroChat.Models
{
    public class User
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 房间的连接
        /// </summary>
        public UserSocket UserSocket { get; set; }
    }
}