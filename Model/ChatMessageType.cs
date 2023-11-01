using System;
using Newtonsoft.Json;

namespace LinGuGu2.Model
{
    /// <summary>
    ///  聊天消息类型
    /// </summary>
    [Serializable]
    public class ChatMessageType
    {
        public Boolean IsMyMessage { get; set; }
        public String Message { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageType()
        {
        }

        public ChatMessageType(Boolean isMyMessage, String message, DateTime time)
        {
            IsMyMessage = isMyMessage;
            Message = message;
            Time = time;
        }

        public ChatMessageType(string json)
        {
            ChatMessageType type = null;
            try
            {
                type = JsonConvert.DeserializeObject<ChatMessageType>(json);
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
                Console.Error.WriteLine("Json解析错误");
                Console.Error.WriteLine("Receive:" + json);
            }

            if (type != null)
            {
                IsMyMessage = type.IsMyMessage;
                Message = type.Message;
                Time = type.Time;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}