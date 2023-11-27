using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LinGuGu2.Model;

namespace LinGuGu2.Service
{
    public class UdpUtil
    {
        public static void SendMsg(String message, string endIp, int port, int reSentTime = 5)
        {
            SendMsg( message, IPAddress.Parse(endIp), port, reSentTime);
        }

        public static void SendMsg(string message, IPAddress endIp, int port, int reSentTime = 5)
        {
            if (reSentTime <= 0)
                return;
            try
            {
                EndPoint point = new IPEndPoint(endIp, port);
                var sioUdpConnReset = -1744830452;
                LocalAccount.GetInstance.LocalSocket.IOControl(
                    (IOControlCode)sioUdpConnReset,
                    new byte[] { 0, 0, 0, 0 },
                    null
                );

                var bf = Encoding.UTF8.GetBytes(message.ToString());
                LocalAccount.GetInstance.LocalSocket.SendTo(bf, point);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                SendMsg(message, endIp, port, reSentTime - 1);
            }
        }
    }
}