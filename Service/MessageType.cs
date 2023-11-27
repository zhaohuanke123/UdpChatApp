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
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum Type { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Time { get; set; }

        public MessageType()
        {
        }

        public MessageType(MessageTypeEnum type, string message)
        {
            Type = type;
            Message = message;
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
                Time = type.Time;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
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
    }
}