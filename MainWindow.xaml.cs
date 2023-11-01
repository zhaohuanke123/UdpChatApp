using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LinGuGu2.Behaviors;
using LinGuGu2.Service;
using LinGuGu2.UserControls;
using LinGuGu2.Util;

namespace LinGuGu2
{
    public partial class MainWindow : Window
    {
        User _currentUser;

        public MainWindow()
        {
            InitializeComponent();

            UdpReceiveThread udpReceiveThread = new UdpReceiveThread(LocalAccount.GetInstance.LocalIp);
            Thread thread1 = new Thread(udpReceiveThread.RunReceive);

            UserMonitorThread userMonitorThread = new UserMonitorThread();
            Thread thread = new Thread(userMonitorThread.RunMonitor);
            UserMonitorThread.UserListChangeEvent += (user =>
            {
                if (_currentUser == user)
                    return;

                if (_currentUser != null)
                {
                    _currentUser.MessageListChangeEvent -= OnFrontUserMessageHandle;
                }
                _currentUser = user;
                _currentUser.MessageListChangeEvent += OnFrontUserMessageHandle;

                App.Current.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        Item item = new Item();
                        item.Title = user.Name;
                        
                        item.MouseDoubleClick += (sender, args) =>
                        {
                            ChatStackPanel.Children.Clear();
                            // 将消息列表中的消息显示到聊天框中
                            for (var i = 0; i < user.MessageList.Count; i++)
                            {
                                if (user.MessageList[i].Sender == user.Name)
                                {
                                    PushAnMessage(user.MessageList[i].Sender, true);
                                }
                                else
                                {
                                    PushAnMessage(user.MessageList[i].Sender, false);
                                }
                            }
                        };
                        
                        GroupStack.Children.Add(item);
                    })
                );
            });
        }

        private void OnBackUserMessageHandle(MessageType messageType)
        {
            App.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    if (_currentUser.Name == messageType.Sender)
                    {
                        // TODO:处于后台时，如果收到消息，则在左侧的列表中显示未读消息
                    }
                })
            );
        }

        private void OnFrontUserMessageHandle(MessageType messageType)
        {
            App.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    if (_currentUser.Name == messageType.Sender)
                    {
                        PushAnMessage(messageType.Message, false);
                    }
                })
            );
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        bool _isMaximized = false;

        public double LastWidth { get; set; } = 1250;
        public double LastHeight { get; set; } = 830;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeCheck();
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (TxtMessage.Text == "")
            {
                return;
            }

            MessageType messageType = new MessageType(
                MessageTypeEnum.Normal,
                TxtMessage.Text,
                LocalAccount.GetInstance.LocalIp.ToString(),
                _currentUser.Ip.ToString()
            );
            PushAnMessage(messageType.Message, true);
            _currentUser.AddMessage(messageType,false);
            UdpUtil.SendMsg(messageType.ToJson(), _currentUser.Ip, UdpReceiveThread.ReceivePort);
            TxtMessage.Text = "";
        }

        private void PushAnMessage(String message, bool isMyMessage = true)
        {
            if (isMyMessage)
            {
                MyMessageChat messageChat = new MyMessageChat();
                messageChat.Message = message;
                ChatStackPanel.Children.Add(messageChat);
            }
            else
            {
                MessageChat messageChat = new MessageChat { Message = message, Color = Brushes.Green };

                ChatStackPanel.Children.Add(messageChat);
                Console.WriteLine("Add a message form other");
            }

            ChatScroll.ScrollToBottom();
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            // 最下化窗口
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            MaximizeCheck();
        }

        private void MaximizeCheck()
        {
            // 最大化窗口
            if (_isMaximized)
            {
                WindowState = WindowState.Normal;
                Width = LastWidth;
                Height = LastHeight;
                _isMaximized = false;
            }
            else
            {
                LastWidth = Width;
                LastHeight = Height;
                WindowState = WindowState.Maximized;
                _isMaximized = true;
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // 关闭窗口
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}