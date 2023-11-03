using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
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
        public ObservableCollection<User> UserList { get; set; }


        public UserMonitorThread(List<User> userList)
        {
            UserList = new ObservableCollection<User>(userList);

            UdpReceiveThread.ReceiveReplyEvent += ReceiveReplyAction;
            UdpReceiveThread.ReceiveRequestEvent += ReceiveRequestAction;
            UdpReceiveThread.ReceiveMessageEvent += ReceiveMessageAction;
            UdpReceiveThread.ReceiveDisconnectEvent += ReceiveDisconnectAction;
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

            //网络号
            Console.WriteLine("网络号：" + new IPAddress(netId));
            // 最大主机号
            Console.WriteLine("最大主机号：" + new IPAddress(maxHostId));

            // 循环对子网中的所有ip进行连接请求
            while (true)
            {
                // 遍历所有可能的主机号,四层for循环
                StringBuilder sBuilder = new StringBuilder();
                Byte[] ipBytes = new Byte[4];
                for (ipBytes[0] = netId[0];
                     ipBytes[0] <= maxHostId[0] - (subnetMaskBytes[0] == 255 ? 0 : 1);
                     ipBytes[0]++)
                {
                    for (ipBytes[1] = netId[1];
                         ipBytes[1] <= maxHostId[1] - (subnetMaskBytes[1] == 255 ? 0 : 1);
                         ipBytes[1]++)
                    {
                        for (ipBytes[2] = netId[2];
                             ipBytes[2] <= maxHostId[2] - (subnetMaskBytes[2] == 255 ? 0 : 1);
                             ipBytes[2]++)
                        {
                            for (ipBytes[3] = netId[3];
                                 ipBytes[3] <= maxHostId[3] - (subnetMaskBytes[3] == 255 ? 0 : 1);
                                 ipBytes[3]++)
                            {
                                foreach (var port in LocalAccount.GetInstance.PortList)
                                {
                                    if (!IsRunning)
                                    {
                                        return;
                                    }

                                    IPAddress ip = new IPAddress(ipBytes);

                                    if (ip.Equals(LocalAccount.GetInstance.LocalIp) &&
                                        port == LocalAccount.GetInstance.LocalPort)
                                    {
                                        continue;
                                    }

                                    // 发送连接请求
                                    MessageType messageType = new MessageType(MessageTypeEnum.RequestConnect,
                                        "",
                                        LocalAccount.GetInstance.LocalIp.ToString()
                                    );
                                    UdpUtil.SendMsg(messageType.ToJson(), ip,
                                        port);
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        public bool IsRunning { get; set; } = true;

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
                        Console.WriteLine("CheckUser: 用户已下线：" + UserList[i]);
                    }
                }
                Thread.Sleep(1500);
            }
        }

        private void ReceiveMessageAction(MessageType messageType, EndPoint endPoint)
        {
            IPAddress endPointIp = ((IPEndPoint)endPoint).Address;
            for (var i = 0; i < UserList.Count; i++)
            {
                if (UserList[i].Ip == endPointIp.ToString() && UserList[i].Port == ((IPEndPoint)endPoint).Port)
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

        private void ReceiveRequestAction(MessageType messageType, EndPoint endPoint)
        {
            IPAddress endPointIp = ((IPEndPoint)endPoint).Address;
            int endPointPort = ((IPEndPoint)endPoint).Port;
            if (messageType.Type == MessageTypeEnum.RequestConnect)
            {
                // 如果是请求连接的消息，则回复连接
                MessageType replyMessageType = new MessageType
                (
                    MessageTypeEnum.ReplyConnect,
                    "",
                    endPointIp.ToString()
                );
                UdpUtil.SendMsg(replyMessageType.ToJson(), endPointIp,
                    endPointPort);
            }
        }

        private void ReceiveReplyAction(MessageType messageType, EndPoint endPoint)
        {
            IPAddress endPointIp = ((IPEndPoint)endPoint).Address;
            int endPointPort = ((IPEndPoint)endPoint).Port;
            if (messageType.Type == MessageTypeEnum.ReplyConnect)
            {
                // 检测用户是否存在
                foreach (var user1 in UserList)
                {
                    if (user1.Ip == endPointIp.ToString() && user1.Port == endPointPort)
                    {
                        user1.CheckOnlineCount = 5;
                        user1.IsOnline = true;
                        Console.WriteLine("用户已存在：" + user1);
                        return;
                    }
                }

                // 如果是回复连接的消息，则将该用户添加到用户列表中
                User user = new User(
                    endPointIp.ToString(),
                    endPointPort,
                    endPointIp.ToString() + ":" + endPointPort
                );
                Application.Current.Dispatcher.Invoke(() => { UserList.Add(user); });
                user.IsOnline = true;

                Console.WriteLine("添加用户：" + user);
            }
        }

        private void ReceiveDisconnectAction(MessageType messageType, EndPoint endPoint)
        {
            IPAddress endPointIp = ((IPEndPoint)endPoint).Address;
            for (var i = 0; i < UserList.Count; i++)
            {
                if (UserList[i].Ip == endPointIp.ToString() && UserList[i].Port == ((IPEndPoint)endPoint).Port)
                {
                    UserList[i].IsOnline = false;
                    return;

                    Console.WriteLine("ReceiveDisconnectAction: 用户已下线：" + UserList[i]);
                }
            }
        }
    }
}