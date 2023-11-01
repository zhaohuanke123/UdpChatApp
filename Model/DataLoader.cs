using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LinGuGu2.UserControls;

namespace LinGuGu2.Model;

public static class DataLoader
{
    /// <summary>
    ///  根据消息列表获取对应的UIElement列表
    /// </summary>
    /// <param name="messageList">消息列表</param>
    /// <param name="user">对应用户</param>
    /// <returns></returns>
    public static List<UIElement> GetUCsMessageList(List<ChatMessageType> messageList, User user)
    {
        List<UIElement> elements = new List<UIElement>();
        if (messageList == null || messageList.Count == 0)
        {
            return elements;
        }

        elements.Add(new ChatSeparator
        {
            Title = messageList[0].Time.ToString("yyyy-MM-dd")
        });
        for (var i = 0; i < messageList.Count; i++)
        {
            // 根据不同情况判断
            // 距离上一条消息发送超过 10分钟

            if (i != 0 && messageList[i].Time - messageList[i - 1].Time > new TimeSpan(0, 10, 0))
            {
                elements.Add(new ChatSeparator
                {
                    Title = messageList[i].Time.ToString("MM-dd HH:mm:ss")
                });
                if (!messageList[i].IsMyMessage)
                {
                    UserChat userChat = new UserChat
                    {
                        Username = user.Name,
                    };
                    elements.Add(userChat);
                }
            }
            // 上一个消息是不同人发送的 || 是第一条消息
            else if (i == 0 || messageList[i].IsMyMessage != messageList[i - 1].IsMyMessage)
            {
                if (!messageList[i].IsMyMessage)
                {
                    UserChat userChat = new UserChat
                    {
                        Username = user.Name,
                    };
                    elements.Add(userChat);
                }
            }

            if (!messageList[i].IsMyMessage)
            {
                MessageChat messageChat = new MessageChat
                {
                    Message = messageList[i].Message,
                    Color = Brushes.Green
                };
                TextBlock textBlock = new TextBlock
                {
                    Text = messageList[i].Time.ToString("HH:mm:ss"),
                };
                textBlock.Style = (Style)Application.Current.Resources["TimeText"];
                elements.Add(messageChat);
                elements.Add(textBlock);
            }
            else
            {
                MyMessageChat myMessageChat = new MyMessageChat
                {
                    Message = messageList[i].Message,
                };
                TextBlock textBlock = new TextBlock
                {
                    Text = messageList[i].Time.ToString("HH:mm:ss"),
                };
                textBlock.Style = (Style)Application.Current.Resources["TimeTextRight"];
                elements.Add(myMessageChat);
                elements.Add(textBlock);
            }
        }

        return elements;
    }

    /// <summary>
    ///  根据上一条消息列表获取对应的UIElement列表
    ///  用于新消息
    ///  </summary>
    public static List<UIElement> GetUCsForNewMessage(List<ChatMessageType> messageList, User user)
    {
        List<UIElement> elements = new List<UIElement>();
        if (messageList == null || messageList.Count == 0)
        {
            return elements;
        }

        ChatMessageType newMessage = messageList.Last();
        if (messageList.Count > 1)
        {
            ChatMessageType lastMessage = messageList[messageList.Count - 2];

            Boolean isTimeSpanOut = false;
            isTimeSpanOut = newMessage.Time - lastMessage.Time > new TimeSpan(0, 10, 0);
            if (isTimeSpanOut)
            {
                elements.Add(new ChatSeparator
                {
                    Title = newMessage.Time.ToString("MM-dd HH:mm:ss")
                });
            }

            // 上一个消息是不同人发送的
            Console.WriteLine("newMessage.IsMyMessage != lastMessage.IsMyMessage: " +
                              (newMessage.IsMyMessage != lastMessage.IsMyMessage));
            if (isTimeSpanOut || newMessage.IsMyMessage != lastMessage.IsMyMessage)
            {
                if (!newMessage.IsMyMessage)
                {
                    UserChat userChat = new UserChat
                    {
                        Username = user.Name,
                    };
                    elements.Add(userChat);
                }
            }
        }
        else if (!newMessage.IsMyMessage)
        {
            UserChat userChat = new UserChat
            {
                Username = user.Name,
            };
            elements.Add(userChat);
        }


        if (!newMessage.IsMyMessage)
        {
            MessageChat messageChat = new MessageChat
            {
                Message = newMessage.Message,
                Color = Brushes.Green
            };
            TextBlock textBlock = new TextBlock
            {
                Text = newMessage.Time.ToString("HH:mm:ss"),
            };
            textBlock.Style = (Style)Application.Current.Resources["TimeText"];
            elements.Add(messageChat);
            elements.Add(textBlock);
        }
        else
        {
            MyMessageChat myMessageChat = new MyMessageChat
            {
                Message = newMessage.Message,
            };
            TextBlock textBlock = new TextBlock
            {
                Text = newMessage.Time.ToString("HH:mm:ss"),
            };
            textBlock.Style = (Style)Application.Current.Resources["TimeTextRight"];
            elements.Add(myMessageChat);
            elements.Add(textBlock);
        }

        return elements;
    }

    // StackPanel的扩展方法
    public static void AddAllMessage(this StackPanel stackPanel, List<UIElement> elements)
    {
        if (elements == null || elements.Count == 0)
            return;

        foreach (var element in elements)
        {
            stackPanel.Children.Add(element);
        }
    }
}