using System;
using System.Runtime.Serialization;

namespace LinGuGu2.Service
{
    /// <summary>
    /// 定义消息类型，作为udp发送的数据类型
    /// </summary>
    [Serializable]
    public class MessageType
        // 添加可序列化
    {
        public MessageType()
        {
            
        }
        public MessageType(string type, string message, string sender, string receiver, string time)
        {
            Type = type;
            Message = message;
            Sender = sender;
            Receiver = receiver;
            Time = time;
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public string Time { get; set; }

        public override string ToString()
        {
            return "Type: " + Type + "\n" +
                   "Message: " + Message + "\n" +
                   "Sender: " + Sender + "\n" +
                   "Receiver: " + Receiver + "\n" +
                   "Time: " + Time + "\n";
        }
    }
}