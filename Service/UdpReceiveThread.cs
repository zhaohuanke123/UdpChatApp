using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LinGuGu2.Util;

namespace LinGuGu2.Service
{
    public class UdpReceiveThread
    {
        public static Socket Client = null;

        public static int ReceivePort = 6660;

        // 普通消息
        public static Action<MessageType> ReceiveMessageEvent;

        // 回复连接
        public static Action<MessageType> ReceiveReplyEvent;

        // 请求连接
        public static Action<MessageType> ReceiveRequestEvent;

        public UdpReceiveThread(IPAddress localIp)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Bind(new IPEndPoint(localIp, ReceivePort));
        }

        public void StartReceive()
        {
            new Thread(() => { }).Start();
        }

        public void RunReceive()
        {
            EndPoint point = new IPEndPoint(IPAddress.Any, 0); //用来保存发送方的ip和端口号
            byte[] buffer = new byte[1024];
            
            while (true)
            {
                int length = Client.ReceiveFrom(buffer, ref point); //接收数据报
                if (length == 0)
                    continue;

                string message = Encoding.UTF8.GetString(buffer, 0, length);
                
                MessageType type = new MessageType(message);
                if (type.Type == MessageTypeEnum.Normal)
                {
                    ReceiveMessageEvent?.Invoke(type);
                }
                else if (type.Type == MessageTypeEnum.ReplyConnect)
                {
                    ReceiveReplyEvent?.Invoke(type);
                }
                else if (type.Type == MessageTypeEnum.RequestConnect)
                {
                    ReceiveRequestEvent?.Invoke(type);
                }
                Console.WriteLine("UdpReceiveThread Receive:" + message);
            }
        }
    }
}