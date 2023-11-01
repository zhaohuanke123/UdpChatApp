using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using LinGuGu2.Service;

namespace LinGuGu2.Util
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
            _messageList = new List<MessageType>();
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

        public Action<MessageType> MessageListChangeEvent;
        private List<MessageType> _messageList;
        public List<MessageType> MessageList => _messageList;
        
        // 当List发生变化时触发事件
        public void AddMessage(MessageType messageType, bool isFrontUser=true)
        {
            _messageList.Add(messageType);
            if (isFrontUser)
            {
                MessageListChangeEvent?.Invoke(messageType);
            }
        }

        public override string ToString()
        {
            return  Name;
        }
    }
}