using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LinGuGu2.Service;
using LinGuGu2.UserControls;
using Newtonsoft.Json;

namespace LinGuGu2.Model;

public static class DataLoader
{
    /// <summary>
    ///  根据消息列表获取对应的UIElement列表
    /// </summary>
    /// <param name="messageList">消息列表</param>
    /// <param name="user">对应用户</param>
    /// <returns></returns>
    public static List<UIElement> GetUCsMessageList(List<ChatMessage> messageList, User user)
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
        List<ChatMessage> messageListTemp = new List<ChatMessage>();
        for (var i = 0; i < messageList.Count; i++)
        {
            messageListTemp.Add(messageList[i]);
            GetUCsForNewMessage(messageListTemp, user).ForEach(element => elements.Add(element));

            // if (i != 0 && messageList[i].Time - messageList[i - 1].Time > new TimeSpan(0, 10, 0))
            // {
            //     elements.Add(new ChatSeparator
            //     {
            //         Title = messageList[i].Time.ToString("MM-dd HH:mm:ss")
            //     });
            //     if (!messageList[i].IsMyMessage)
            //     {
            //         UserChat userChat = new UserChat
            //         {
            //             Username = user.Name,
            //         };
            //         elements.Add(userChat);
            //     }
            // }
            // // 上一个消息是不同人发送的 || 是第一条消息
            // else if (i == 0 || messageList[i].IsMyMessage != messageList[i - 1].IsMyMessage)
            // {
            //     if (!messageList[i].IsMyMessage)
            //     {
            //         UserChat userChat = new UserChat
            //         {
            //             Username = user.Name,
            //         };
            //         elements.Add(userChat);
            //     }
            // }
            //
            // if (!messageList[i].IsMyMessage)
            // {
            //     MessageChat messageChat = new MessageChat
            //     {
            //         Message = messageList[i].Message,
            //         Color = Brushes.Green
            //     };
            //     TextBlock textBlock = new TextBlock
            //     {
            //         Text = messageList[i].Time.ToString("HH:mm:ss"),
            //     };
            //     textBlock.Style = (Style)Application.Current.Resources["TimeText"];
            //     elements.Add(messageChat);
            //     elements.Add(textBlock);
            // }
            // else
            // {
            //     MyMessageChat myMessageChat = new MyMessageChat
            //     {
            //         Message = messageList[i].Message,
            //     };
            //     TextBlock textBlock = new TextBlock
            //     {
            //         Text = messageList[i].Time.ToString("HH:mm:ss"),
            //     };
            //     textBlock.Style = (Style)Application.Current.Resources["TimeTextRight"];
            //     elements.Add(myMessageChat);
            //     elements.Add(textBlock);
            // }
        }

        return elements;
    }

    /// <summary>
    ///  根据上一条消息列表获取对应的UIElement列表
    ///  用于新消息
    ///  </summary>
    public static List<UIElement> GetUCsForNewMessage(List<ChatMessage> messageList, User user)
    {
        List<UIElement> elements = new List<UIElement>();
        if (messageList == null || messageList.Count == 0)
        {
            return elements;
        }

        ChatMessage newMessage = messageList.Last();

        if (newMessage.Type == ChatMessageTypeEnum.Online)
        {
            elements.Add(new ChatSeparator
            {
                Title = "对方上线了"
            });
            return elements;
        }
        else if (newMessage.Type == ChatMessageTypeEnum.Offline)
        {
            elements.Add(new ChatSeparator
            {
                Title = "对方下线了"
            });
            return elements;
        }

        if (messageList.Count > 1)
        {
            ChatMessage lastMessage = messageList[messageList.Count - 2];

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

    public static void SaveData()
    {
        if (MainWindow.UserMonitorThread != null)
        {
            var userList = MainWindow.UserMonitorThread.UserList;

            // 将用户列表保存到本地
            var json = JsonConvert.SerializeObject(userList);

            string folderPath = "./data";
            // 文件夹路径
            string filePath = Path.Combine(folderPath, "userList.json"); // 文件路径

            // 检查文件夹是否存在，如果不存在则创建
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 检查文件是否存在，如果不存在则创建一个空文件
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close(); // 创建文件并关闭以释放资源}
            }

            //  写入到本地文件 
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("保存用户列表成功" + json);
        }
    }

    public static List<User> LoadData()
    {
        string folderPath = "./data";
        // 文件夹路径
        string filePath = Path.Combine(folderPath, "userList.json"); // 文件路径

        // 检查文件夹是否存在，如果不存在则创建
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 检查文件是否存在，如果不存在则创建一个空文件
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close(); // 创建文件并关闭以释放资源}
        }

        // 读取本地文件
        try
        {
            var json = File.ReadAllText(filePath);
            var userList = JsonConvert.DeserializeObject<List<User>>(json);
            if (userList != null)
            {
                return userList;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return new List<User>();
    }
}