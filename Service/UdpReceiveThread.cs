using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LinGuGu2.Model;
using LinGuGu2.Util;

namespace LinGuGu2.Service
{
    public class UdpReceiveThread
    {
        // 普通消息
        public static Action<MessageType, EndPoint> ReceiveMessageEvent;

        // 回复连接
        public static Action<MessageType, EndPoint> ReceiveReplyEvent;

        // 请求连接
        public static Action<MessageType, EndPoint> ReceiveRequestEvent;
        public static Action<MessageType, EndPoint> ReceiveDisconnectEvent;

        public UdpReceiveThread()
        {
        }

        public bool IsRunning { get; set; } = true;

        public void RunReceive()
        {
            EndPoint point = new IPEndPoint(IPAddress.Any, 0); //用来保存发送方的ip和端口号
            byte[] buffer = new byte[1024];

            while (true)
            {
                if (!IsRunning)
                {
                    return;
                }

                int length = LocalAccount.GetInstance.LocalSocket.ReceiveFrom(buffer, ref point); //接收数据报
                if (length == 0)
                    continue;

                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine("收到来自：" + point + " 的消息：" + message);

                MessageType type = new MessageType(message);
                if (type.Type == MessageTypeEnum.Normal)
                {
                    ReceiveMessageEvent?.Invoke(type, point);
                }
                else if (type.Type == MessageTypeEnum.ReplyConnect)
                {
                    ReceiveReplyEvent?.Invoke(type, point);
                }
                else if (type.Type == MessageTypeEnum.RequestConnect)
                {
                    ReceiveRequestEvent?.Invoke(type, point);
                }
                else if (type.Type == MessageTypeEnum.RequestDisconnect)
                {
                    ReceiveDisconnectEvent?.Invoke(type, point);
                }
            }
        }
    }
}