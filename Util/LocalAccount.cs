using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LinGuGu2.Util
{
    public class LocalAccount
    {
        private static LocalAccount _instance = new LocalAccount();

        public static LocalAccount GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalAccount();
                }

                return _instance;
            }
        }

        public IPAddress LocalIp { get; set; } // 本机的局域网ip
        public int LocalPort { get; set; } // 本机的端口号

        public string Name { get; set; } = "123"; // 本机的名字

        // 子网掩码
        public IPAddress SubnetMask { get; set; }

        private LocalAccount()
        {
            // 获取本机的局域网ip
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    LocalIp = ip;
                    Console.WriteLine("IP Address = " + ip.ToString());
                    // 获取子网掩码
                    foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        foreach (UnicastIPAddressInformation unitIpAddressInformation in adapter.GetIPProperties()
                                     .UnicastAddresses)
                        {
                            if (unitIpAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (LocalIp.Equals(unitIpAddressInformation.Address))
                                {
                                    SubnetMask = unitIpAddressInformation.IPv4Mask;
                                    Console.WriteLine("subIpMask is" + unitIpAddressInformation.IPv4Mask);
                                }
                            }
                        }
                    }
                }
            }

            // 寻找一个可使用的端口
            for (var port = 10000; port < 65535; port++)
            {
                try
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(new IPEndPoint(LocalIp, port));
                    LocalPort = port;
                    socket.Close();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("端口" + port + "被占用");
                }
            }

            Console.WriteLine("LocalPort:" + LocalPort);
        }
    }
}