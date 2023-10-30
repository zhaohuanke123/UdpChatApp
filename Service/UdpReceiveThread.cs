using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LinGuGu2.Service
{
    public class UdpReceiveThread
    {
        public static Socket Client = null;
        public Action<String> ReceiveAction;

        public UdpReceiveThread(String localIp, int localPort)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Bind(new IPEndPoint(IPAddress.Parse(localIp), localPort));
        }

        public void StartReceive()
        {
            new Thread(() =>
            {
                while (true)
                {
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0); //用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    int length = Client.ReceiveFrom(buffer, ref point); //接收数据报
                    if (length == 0)
                        continue;
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine("Receive:" + message);
                    ReceiveAction?.Invoke(message);
                }
            }).Start();
        }
    }
}