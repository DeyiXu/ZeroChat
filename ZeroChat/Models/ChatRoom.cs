using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroChat.Models
{
    /// <summary>
    /// 聊天室
    /// </summary>
    public class ChatRoom
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 聊天室的用户
        /// </summary>
        public User[] Users { get; private set; }
        /// <summary>
        /// 聊天室的最大成员数
        /// </summary>
        public int MaxUserCount { get; set; }

        /// <summary>
        /// 聊天室
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">Name</param>
        /// <param name="maxUserCount">允许加入聊天室的最大成员数</param>
        public ChatRoom(int id, string name, int maxUserCount)
        {
            Id = id;
            Name = name;
            MaxUserCount = maxUserCount;
            Users = new User[maxUserCount];
        }

        //public (bool,string) AddUser(User user)
        //{
        //    if (Users.Length == MaxUserCount)
        //        return (false, "不允许加入聊天室，聊天人数已满！");
        //    return (true,"");
        //}
        public bool AddUser(User user, out string message)
        {
            message = "";
            if (Users.Length == MaxUserCount)
            {
                message = "不允许加入聊天室，聊天人数已满！";
                return false;
            }
            //判断用户是否存在
            if (Users.Where(s => s.Name == user.Name).First() == null)
            {
                message = "不允许加入聊天室，用户名已存在！";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="name">用户名</param>
        public void DelUser(string name)
        {
            User user = Users.Where(s => s.Name == name).First();
            user?.WebSocket.Abort();
            Users = Users.Where(s => s.Name != name).ToArray();
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="webSocket">WebSocket</param>
        public void DelUser(WebSocket webSocket)
        {
            User user = Users.Where(s => s.WebSocket.SubProtocol == webSocket.SubProtocol).First();
            user?.WebSocket.Abort();
            Users = Users.Where(s => s.WebSocket.SubProtocol != webSocket.SubProtocol).ToArray();
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="msg">消息</param>
        public void Broadcast(string msg)
        {
            byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
            for (int i = 0; i < Users.Length; i++)
            {
                WebSocket clent = Users[i].WebSocket;
                if (clent != null && clent.State == WebSocketState.Open)
                {
                    Users[i].WebSocket.SendAsync(msgBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
