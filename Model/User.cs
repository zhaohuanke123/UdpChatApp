using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LinGuGu2.Model;
using LinGuGu2.Service;
using Newtonsoft.Json;

namespace LinGuGu2.Model
{
    /// <summary>
    /// 储存局域网中用户的信息
    /// </summary>
    [Serializable]
    public class User : ObservableObject
    {
        public User()
        {
        }

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

        private bool _isChatWith;

        [JsonIgnore]
        public bool IsChatWith
        {
            get => _isChatWith;
            set
            {
                SetProperty(ref _isChatWith, value);
                if (value)
                {
                    MessageCount = "";
                }
            }
        }

        private Brush _color;

        [JsonIgnore]
        public Brush Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private bool _isOnline;

        [JsonIgnore]
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                if (_isOnline == value)
                    return;
                if (Ip == LocalAccount.GetInstance.LocalIp.ToString() && Port == LocalAccount.GetInstance.LocalPort)
                {
                    return;
                }

                SetProperty(ref _isOnline, value);
                if (value)
                {
                    Color = Brushes.Green;
                    AddMessage(new ChatMessage(
                        false,
                        "",
                        DateTime.Now,
                        ChatMessageTypeEnum.Online
                    ));
                }
                else
                {
                    Color = Brushes.Gray;
                    AddMessage(new ChatMessage(
                        false,
                        "",
                        DateTime.Now,
                        ChatMessageTypeEnum.Offline
                    ));
                }
            }
        }

        private string _messageCount = "";

        [JsonIgnore]
        public string MessageCount
        {
            get => _messageCount;
            set => SetProperty(ref _messageCount, value);
        }

        [JsonIgnore] public int CheckOnlineCount { get; set; } = 0;

        [JsonIgnore] public Action<ChatMessage> MessageListChangeEvent;
        public List<ChatMessage> MessageList { get; private set; } = new();
        [JsonIgnore] public Action OnLineEvent { get; set; }

        // 当List发生变化时触发事件
        public void AddMessage(ChatMessage message)
        {
            MessageList.Add(message);
            if (!message.IsMyMessage)
            {
                MessageListChangeEvent?.Invoke(message);
            }

            if (IsChatWith)
            {
            }
            else
            {
                int count = 0;
                count = MessageCount == "" ? 1 : int.Parse(MessageCount) + 1;
                MessageCount = count.ToString();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}