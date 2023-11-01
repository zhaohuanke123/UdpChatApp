using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using LinGuGu2.Model;
using LinGuGu2.Service;
using Newtonsoft.Json;

namespace LinGuGu2.Model
{
    /// <summary>
    /// 储存局域网中用户的信息
    /// </summary>
    [Serializable]
    public class User
    {
        public User(){}
        public User(string ip, int port, string name)
        {
            Ip = ip;
            Port = port;
            Name = name;
        }

        /// <summary>
        /// 用户的ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 用户的端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户的名字
        /// </summary>
        public string Name { get; set; }
        
        [JsonIgnore] public bool IsOnline { get; set; } = false;
        [JsonIgnore] public int CheckOnlineCount { get; set; } = 0;
        [JsonIgnore] public Action OfflineEvent;
        


        [JsonIgnore] public Action<ChatMessageType> MessageListChangeEvent;
        public List<ChatMessageType> MessageList { get; private set; } = new();
        public Action OnLineEvent { get; set; }

        // 当List发生变化时触发事件
        public void AddMessage(ChatMessageType message)
        {
            MessageList.Add(message);
            if (!message.IsMyMessage)
            {
                MessageListChangeEvent?.Invoke(message);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}