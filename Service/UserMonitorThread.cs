using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using LinGuGu2.Model;
using LinGuGu2.Util;

namespace LinGuGu2.Service
{
    /// <summary>
    /// 循环检测局域网的设备，定时发送连接请求
    /// </summary>
    public class UserMonitorThread
    {
        public List<User> UserList { get; set; }

        public UserMonitorThread(List<User> userList)
        {
            UserList = userList;

            UdpReceiveThread.ReceiveReplyEvent += ReceiveReplyAction;
            UdpReceiveThread.ReceiveRequestEvent += ReceiveRequestAction;
            UdpReceiveThread.ReceiveMessageEvent += ReceiveMessageAction;
            UdpReceiveThread.ReceiveDisconnectEvent += ReceiveDisconnectAction;
        }

        private void ReceiveDisconnectAction(MessageType messageType)
        {
            for (var i = 0; i < UserList.Count; i++)
            {
                if (UserList[i].Ip.ToString() == messageType.Sender)
                {
                    UserList[i].IsOnline = false;
                    return;
                }
            }
        }

        public void RunMonitor()
        {
            if (LocalAccount.GetInstance.LocalIp == IPAddress.Parse("127.0.0.1"))
            {
                Console.Error.WriteLine("未连接网络");
                return;
            }

            // 获取子网掩码
            var subnetMask = LocalAccount.GetInstance.SubnetMask;
            // 获取本机的ip
            var localIp = LocalAccount.GetInstance.LocalIp;
            // 获取子网掩码的字节数组
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            // 获取本机的ip的字节数组
            var localIpBytes = localIp.GetAddressBytes();

            //获取网络号
            var netId = new byte[4];
            for (var i = 0; i < subnetMaskBytes.Length; i++)
            {
                netId[i] = (byte)(localIpBytes[i] & subnetMaskBytes[i]);
            }

            // 获取子网掩码的反码
            var subnetMaskReverseBytes = new byte[4];
            for (var i = 0; i < subnetMaskBytes.Length; i++)
            {
                subnetMaskReverseBytes[i] = (byte)~subnetMaskBytes[i];
            }

            // 求得最大主机号
            var maxHostId = new byte[4];
            for (var i = 0; i < subnetMaskBytes.Length; i++)
            {
                maxHostId[i] = (byte)(netId[i] | subnetMaskReverseBytes[i]);
            }

            // 循环对子网中的所有ip进行连接请求
            while (true)
            {
                // 遍历所有可能的主机号,四层for循环
                for (var netId0 = netId[0]; netId0 <= maxHostId[0]; netId0++)
                {
                    for (var netId1 = netId[1]; netId1 <= maxHostId[1]; netId1++)
                    {
                        for (var netId2 = netId[2]; netId2 <= maxHostId[2]; netId2++)
                        {
                            for (var netId3 = netId[3]; netId3 < maxHostId[3]; netId3++)
                            {
                                var ip = netId0 + "." + netId1 + "." + netId2 + "." + netId3;

                                // 发送连接请求
                                MessageType messageType = new MessageType(MessageTypeEnum.RequestConnect,
                                    "",
                                    LocalAccount.GetInstance.LocalIp.ToString(),
                                    ip.ToString()
                                );
                                UdpUtil.SendMsg(messageType.ToJson(), ip,
                                    UdpReceiveThread.ReceivePort);
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        public void CheckUser()
        {
            while (true)
            {
                // 遍历所有用户，检测是否在线
                for (var i = 0; i < UserList.Count; i++)
                {
                    if (UserList[i].CheckOnlineCount > 0)
                    {
                        UserList[i].CheckOnlineCount--;
                    }
                    else
                    {
                        UserList[i].IsOnline = false;
                        UserList[i].OfflineEvent?.Invoke();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void ReceiveMessageAction(MessageType messageType)
        {
            for (var i = 0; i < UserList.Count; i++)
            {
                if (UserList[i].Ip.ToString() == messageType.Sender)
                {
                    ChatMessage message = new ChatMessage
                    (
                        false,
                        messageType.Message,
                        messageType.Time
                    );
                    UserList[i].AddMessage(message);
                    return;
                }
            }
        }

        private void ReceiveRequestAction(MessageType messageType)
        {
            if (messageType.Type == MessageTypeEnum.RequestConnect)
            {
                // 如果是请求连接的消息，则回复连接
                MessageType replyMessageType = new MessageType
                (
                    MessageTypeEnum.ReplyConnect,
                    "",
                    LocalAccount.GetInstance.LocalIp.ToString(),
                    messageType.Sender
                );
                UdpUtil.SendMsg(replyMessageType.ToJson(), messageType.Sender,
                    UdpReceiveThread.ReceivePort);
            }
        }

        private void ReceiveReplyAction(MessageType messageType)
        {
            if (messageType.Type == MessageTypeEnum.ReplyConnect)
            {
                // 检测用户是否存在
                foreach (var user1 in UserList)
                {
                    if (user1.Ip == messageType.Sender)
                    {
                        user1.CheckOnlineCount = 5;
                        user1.IsOnline = true;
                        Console.WriteLine("用户已存在：" + user1);
                        return;
                    }
                }

                // 如果是回复连接的消息，则将该用户添加到用户列表中
                User user = new User(
                    messageType.Sender,
                    UdpReceiveThread.ReceivePort,
                    messageType.Sender
                );
                UserList.Add(user);
                user.IsOnline = true;

                if (messageType.Sender == LocalAccount.GetInstance.LocalIp.ToString())
                {
                    user.Name = LocalAccount.GetInstance.Name;
                }
                Console.WriteLine("添加用户：" + user);
            }
        }
    }
}