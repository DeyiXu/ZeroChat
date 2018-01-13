using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroChat.Enums;

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
        public List<User> Users { get; private set; }
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
            Users = new List<User>();
        }

        //public (bool,string) AddUser(User user)
        //{
        //    if (Users.Length == MaxUserCount)
        //        return (false, "不允许加入聊天室，聊天人数已满！");
        //    return (true,"");
        //}
        public bool AddUser(User user, out string message)
        {
            message = "进入房间成功";
            if (Users.Count == MaxUserCount)
            {
                message = "不允许加入聊天室，聊天人数已满！";
                return false;
            }
            //判断用户是否存在

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name==user.Name)
                {
                    message = "不允许加入聊天室，用户名已存在！";
                    return false;
                }
            }

            //添加用户
            Users.Add(user);

            return true;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="name">用户名</param>
        public void DelUser(string name)
        {
            User user = Users.Where(s => s.Name == name).First();
            user?.UserSocket.WebSocket.Abort();
            Users = Users.Where(s => s.Name != name).ToList();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="connectionId">connectionId</param>
        public void DelUser(string connectionId, SocketEnum socketEnum)
        {
            User user = Users.Where(s => s.UserSocket.WebSocketConnectionId == connectionId).First();
            user?.UserSocket.WebSocket.Abort();
            Users = Users.Where(s => s.UserSocket.WebSocketConnectionId != connectionId).ToList();
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="msg">消息</param>
        public void Broadcast(string msg)
        {
            byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i] == null)
                {
                    break;
                }
                WebSocket clent = Users[i].UserSocket.WebSocket;
                if (clent != null && clent.State == WebSocketState.Open)
                {
                    Users[i].UserSocket.WebSocket.SendAsync(msgBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
