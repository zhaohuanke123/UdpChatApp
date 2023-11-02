using System;
using Newtonsoft.Json;

namespace LinGuGu2.Model
{
    /// <summary>
    ///  聊天消息类型
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public Boolean IsMyMessage { get; set; }
        public String Message { get; set; }
        public DateTime Time { get; set; }
        public ChatMessageTypeEnum Type { get; set; }

        public ChatMessage()
        {
        }

        public ChatMessage(Boolean isMyMessage, String message, DateTime time,
            ChatMessageTypeEnum type = ChatMessageTypeEnum.Text)
        {
            IsMyMessage = isMyMessage;
            Message = message;
            Time = time;
            Type = type;
        }

        public ChatMessage(string json)
        {
            ChatMessage type = null;
            try
            {
                type = JsonConvert.DeserializeObject<ChatMessage>(json);
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

    public enum ChatMessageTypeEnum
    {
        Text = 1,
        Online = 2,
        Offline = 3,
    }
}