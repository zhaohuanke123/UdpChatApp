using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using LinGuGu2.Behaviors;
using LinGuGu2.Model;
using LinGuGu2.Service;
using LinGuGu2.UserControls;
using LinGuGu2.Util;

namespace LinGuGu2
{
    public partial class MainWindow : Window
    {
        User _currentUser;
        public static UdpReceiveThread UdpReceiveThread;
        public static UserMonitorThread UserMonitorThread;

        public MainWindow()
        {
            InitializeComponent();

            ChatStackPanel.Children.Clear();
            ChatPanel.Visibility = Visibility.Collapsed;

            UdpReceiveThread = new UdpReceiveThread(LocalAccount.GetInstance.LocalIp);
            Thread thread1 = new Thread(UdpReceiveThread.RunReceive);
            thread1.Start();

            UserMonitorThread = new UserMonitorThread(DataLoader.LoadData());
            Thread thread = new Thread(UserMonitorThread.RunMonitor);
            thread.Start();
            Thread thread2 = new Thread(UserMonitorThread.CheckUser);
            thread2.Start();

            UserListView.ItemsSource = UserMonitorThread.UserList;

            WeakReferenceMessenger.Default.Register<string, string>(this, "NotificationMessageAction", (r, m) => { });
        }

        private void OnFrontUserMessageHandle(ChatMessage message)
        {
            App.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    var elements =
                        DataLoader.GetUCsForNewMessage(_currentUser.MessageList, _currentUser);
                    ChatStackPanel.AddAllMessage(elements);
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

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (TxtMessage.Text == "")
            {
                return;
            }

            if (_currentUser == null)
            {
                MessageBox.Show("请选择一个用户");
                return;
            }

            MessageType messageType = new MessageType(
                MessageTypeEnum.Normal,
                TxtMessage.Text,
                LocalAccount.GetInstance.LocalIp.ToString(),
                _currentUser.Ip.ToString()
            );
            ChatMessage chatMessage = new ChatMessage(
                true,
                TxtMessage.Text,
                messageType.Time
            );
            _currentUser.AddMessage(chatMessage);
            TxtMessage.Text = "";

            var elements = DataLoader.GetUCsForNewMessage(_currentUser.MessageList, _currentUser);
            ChatStackPanel.AddAllMessage(elements);

            UdpUtil.SendMsg(messageType.ToJson(), _currentUser.Ip, UdpReceiveThread.ReceivePort);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeCheck();
            }
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
            DataLoader.SaveData();
            // 关闭窗口
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void UserItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                List<ChatMessage> messages = selectedUser.MessageList;

                ChatPanel.Visibility = Visibility.Visible;

                if (_currentUser != null)
                {
                    _currentUser.MessageListChangeEvent -= OnFrontUserMessageHandle;
                    _currentUser.IsChatWith = false;
                }

                _currentUser = selectedUser;
                _currentUser.IsChatWith = true;
                _currentUser.MessageListChangeEvent += OnFrontUserMessageHandle;

                ChatStackPanel.Children.Clear();
                // 将消息列表中的消息显示到聊天框中
                var elements = DataLoader.GetUCsMessageList(_currentUser.MessageList, _currentUser);
                ChatStackPanel.AddAllMessage(elements);

                ChatUserName.Text = selectedUser.Name;

                ChatScroll.ScrollToBottom();
            }
        }

        private void MessageEnterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (TxtMessage.Text == "")
                {
                    return;
                }

                if (_currentUser == null)
                {
                    MessageBox.Show("请选择一个用户");
                    return;
                }

                MessageType messageType = new MessageType(
                    MessageTypeEnum.Normal,
                    TxtMessage.Text,
                    LocalAccount.GetInstance.LocalIp.ToString(),
                    _currentUser.Ip.ToString()
                );
                ChatMessage chatMessage = new ChatMessage(
                    true,
                    TxtMessage.Text,
                    messageType.Time
                );
                _currentUser.AddMessage(chatMessage);
                TxtMessage.Text = "";

                var elements = DataLoader.GetUCsForNewMessage(_currentUser.MessageList, _currentUser);
                ChatStackPanel.AddAllMessage(elements);

                UdpUtil.SendMsg(messageType.ToJson(), _currentUser.Ip, UdpReceiveThread.ReceivePort);
            }
        }
    }
}