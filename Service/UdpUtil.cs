using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LinGuGu2.Service
{
    public class UdpUtil
    {
        public static void SendMsg(String message, IPAddress endIp, int port, int reSentTime = 5)
        {
            if (reSentTime <= 0)
                return;
            try
            {
                EndPoint point = new IPEndPoint(endIp, port);
                int sioUdpConnReset = -1744830452;
                UdpReceiveThread.Client?.IOControl(
                    (IOControlCode)sioUdpConnReset,
                    new byte[] { 0, 0, 0, 0 },
                    null
                );

                var bf = Encoding.UTF8.GetBytes(message.ToString());
                UdpReceiveThread.Client?.SendTo(bf, point);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                SendMsg(message, endIp, port, reSentTime - 1);
            }
        }
    }
}