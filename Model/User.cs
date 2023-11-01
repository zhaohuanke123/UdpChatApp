using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using LinGuGu2.Model;
using LinGuGu2.Service;

namespace LinGuGu2.Model
{
    /// <summary>
    /// 储存局域网中用户的信息
    /// </summary>
    public class User
    {
        public User(IPAddress ip, int port, string name)
        {
            Ip = ip;
            Port = port;
            Name = name;
        }

        /// <summary>
        /// 用户的ip地址
        /// </summary>
        public IPAddress Ip { get; set; }

        /// <summary>
        /// 用户的端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户的名字
        /// </summary>
        public string Name { get; set; }

        public Boolean IsOnline { get; set; } = false;

        public Action<ChatMessageType> MessageListChangeEvent;
        public List<ChatMessageType> MessageList { get; private set; } = new();

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