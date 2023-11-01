using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using LinGuGu2.Model;
using LinGuGu2.Util;

namespace LinGuGu2.Service
{
    /// <summary>
    /// 循环检测局域网的设备，定时发送连接请求
    /// </summary>
    public class UserMonitorThread
    {
        List<User> _userList = new();
        public List<User> UserList => _userList;

        public UserMonitorThread()
        {
            UdpReceiveThread.ReceiveReplyEvent += ReceiveReplyAction;
            UdpReceiveThread.ReceiveRequestEvent += ReceiveRequestAction;
            UdpReceiveThread.ReceiveMessageEvent += ReceiveMessageAction;
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
                                UdpUtil.SendMsg(messageType.ToJson(), IPAddress.Parse(ip),
                                    UdpReceiveThread.ReceivePort);
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private void ReceiveMessageAction(MessageType messageType)
        {
            for (var i = 0; i < _userList.Count; i++)
            {
                if (_userList[i].Ip.ToString() == messageType.Sender)
                {
                    ChatMessageType message = new ChatMessageType
                    (
                        false,
                        messageType.Message,
                        messageType.Time
                    );
                    _userList[i].AddMessage(message);
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
                UdpUtil.SendMsg(replyMessageType.ToJson(), IPAddress.Parse(messageType.Sender),
                    UdpReceiveThread.ReceivePort);
            }
        }

        private void ReceiveReplyAction(MessageType messageType)
        {
            if (messageType.Type == MessageTypeEnum.ReplyConnect)
            {
                // 检测用户是否存在
                foreach (var user1 in _userList)
                {
                    if (user1.Ip.ToString() == messageType.Sender)
                    {
                        Console.WriteLine("用户已存在：" + user1);
                        return;
                    }
                }

                // 如果是回复连接的消息，则将该用户添加到用户列表中
                User user = new User(
                    IPAddress.Parse(messageType.Sender),
                    UdpReceiveThread.ReceivePort,
                    messageType.Sender
                );

                if (messageType.Sender == LocalAccount.GetInstance.LocalIp.ToString())
                {
                    user.Name = LocalAccount.GetInstance.Name;
                }

                _userList.Add(user);
                Console.WriteLine("添加用户：" + user);
            }
        }
    }
}