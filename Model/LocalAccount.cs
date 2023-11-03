using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LinGuGu2.Model
{
    public class LocalAccount
    {
        private static LocalAccount _instance = new();

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
        public IPAddress SubnetMask { get; set; }  // 子网掩码
        public int LocalPort { get; set; } // 本机的端口号
        public Socket LocalSocket { get; set; } // 本机的socket
        public string Name { get; set; } = "localHost"; // 本机的名字
        // 备选端口列表
        public int[] PortList { get; set; } = {6006, 7001, 8002, 4003, 10004, 10005, 10006, 10007};

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
            foreach (var port in PortList)
            {
                try
                {
                    LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LocalSocket.Bind(new IPEndPoint(LocalIp, port));
                    LocalPort = port;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("LocalPort:" + LocalPort);
            
            
        }
    }
}