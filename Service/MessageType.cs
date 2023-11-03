using System;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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

        public MessageType(MessageTypeEnum type, string message, string sender)
        {
            Type = type;
            Message = message;
            Sender = sender;
            // 获取当前时间
            Time = DateTime.Now;
        }

        public MessageType(string json)
        {
            MessageType type = null;
            try
            {
                type = JsonConvert.DeserializeObject<MessageType>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Error.WriteLine("Json解析错误");
                Console.Error.WriteLine("Receive:" + json);
            }

            if (type != null)
            {
                Type = type.Type;
                Message = type.Message;
                Sender = type.Sender;
                Time = type.Time;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum Type { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Sender { get; set; }
        public int SenderPort { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    // 消息类型
    [Serializable]
    public enum MessageTypeEnum
    {
        /// <summary>
        /// 普通消息
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 请求连接
        /// </summary>
        RequestConnect = 1,

        /// <summary>
        /// 回复连接
        /// </summary>
        ReplyConnect = 2,

        /// <summary>
        /// 请求断开连接
        /// </summary>
        RequestDisconnect = 3,

        /// <summary>
        /// 回复断开连接
        /// </summary>
        ReplyDisconnect = 4,

        /// <summary>
        /// 请求获取用户列表
        /// </summary>
        RequestUserList = 5,

        /// <summary>
        /// 回复获取用户列表
        /// </summary>
        ReplyUserList = 6,

        /// <summary>
        /// 请求获取聊天记录
        /// </summary>
        RequestChatRecord = 7
    }
}