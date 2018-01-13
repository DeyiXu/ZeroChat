using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZeroChat.Models;

namespace ZeroChat
{
    public class Program
    {
        private static ConcurrentDictionary<string, UserSocket> clientPool = new ConcurrentDictionary<string, UserSocket>();
        public static void Main(string[] args)
        {
            ChatHandler.ClientClose = ClientClose;
            ChatHandler.ClientMessage = ClientMessage;
            ChatHandler.ClientConnection = ClientConnection;

            Global.chatRooms.Add(new ChatRoom(1, "房间1", 20));
            Global.chatRooms.Add(new ChatRoom(2, "房间2", 20));
            Global.chatRooms.Add(new ChatRoom(3, "房间3", 20));

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static void ClientConnection(string connectionId, WebSocket webSocket)
        {
            //添加用户
            UserSocket userSocket = new UserSocket
            {
                WebSocket = webSocket,
                WebSocketConnectionId = connectionId
            };
            clientPool.TryAdd(connectionId, userSocket);

            ChatHandler.Send("连接成功", webSocket);
        }

        public static void ClientMessage(string connectionId, string message)
        {
            string[] values = message.Split("-");
            switch (values[0])
            {
                case "jinfangjian":
                    UserSocket userSocket = null;
                    clientPool.TryGetValue(connectionId, out userSocket);
                    User user = new User()
                    {
                        Name = connectionId,
                        UserSocket = userSocket
                    };
                    int chatId = int.Parse(values[1]);
                    // N 号房间 添加用户
                    for (int i = 0; i < Global.chatRooms.Count; i++)
                    {
                        if (Global.chatRooms[i].Id== chatId)
                        {
                            Global.chatRooms[i].AddUser(user, out string outMesg);
                            //发送信息
                            ChatHandler.Send(outMesg, userSocket.WebSocket);
                            break;
                        }
                    }
                    break;
            }
        }

        public static void ClientClose(string connectionId)
        {
           
        }

    }
}
