using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using LinGuGu2.Behaviors;
using LinGuGu2.Model;
using LinGuGu2.Service;
using LinGuGu2.UserControls;
using LinGuGu2.Util;
using LinGuGu2.ViewModel;

namespace LinGuGu2
{
    public partial class MainWindow : Window
    {
        public static User CurrentUser { get; set; }
        public static UdpReceiveThread UdpReceiveThread;
        public static UserMonitorThread UserMonitorThread;
        Thread thread;
        Thread thread1;
        Thread thread2;

        public MainWindow()
        {
            InitializeComponent();

            SHowAllButton.Content =
                LocalAccount.GetInstance.LocalIp.ToString() + ":" + LocalAccount.GetInstance.LocalPort;

            UdpReceiveThread = new UdpReceiveThread();
            thread1 = new Thread(UdpReceiveThread.RunReceive);
            thread1.Start();
            UserMonitorThread = new UserMonitorThread(DataLoader.LoadData());
            thread = new Thread(UserMonitorThread.RunMonitor);
            thread.Start();
            thread2 = new Thread(UserMonitorThread.CheckUser);
            thread2.Start();

            UserListView.ItemsSource = UserMonitorThread.UserList;

            DataContext = new MainViewModel();
        }

        private void OnFrontUserMessageHandle(ChatMessage message)
        {
            App.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    var elements =
                        DataLoader.GetUCsForNewMessage(CurrentUser.MessageList, CurrentUser);
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
        public double LastWidth { get; set; }
        public double LastHeight { get; set; }

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
            UdpReceiveThread.IsRunning = false;
            UserMonitorThread.IsRunning = false;

            foreach (var user in UserMonitorThread.UserList)
            {
                // 发送断开连接的消息
                MessageType messageType = new MessageType(
                    MessageTypeEnum.RequestDisconnect,
                    "",
                    user.Ip
                );
                UdpUtil.SendMsg(messageType.ToJson(), user.Ip, user.Port);
            }

            DataLoader.SaveData();
            // 关闭窗口
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void UserItemLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                List<ChatMessage> messages = selectedUser.MessageList;

                ChatPanel.Visibility = Visibility.Visible;

                if (CurrentUser != null)
                {
                    CurrentUser.MessageListChangeEvent -= OnFrontUserMessageHandle;
                    CurrentUser.IsChatWith = false;
                }

                CurrentUser = selectedUser;
                CurrentUser.IsChatWith = true;
                CurrentUser.MessageListChangeEvent += OnFrontUserMessageHandle;

                ChatStackPanel.Children.Clear();
                // 将消息列表中的消息显示到聊天框中
                var elements = DataLoader.GetUCsMessageList(CurrentUser.MessageList, CurrentUser);
                ChatStackPanel.AddAllMessage(elements);

                ChatUserName.Text = selectedUser.Name;

                ChatScroll.ScrollToBottom();
            }
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                // 清除所选用户的聊天记录
                selectedUser.MessageList.Clear();
                ChatStackPanel.Children.Clear();
            }
        }

        private void ShowAllUserButton(object sender, RoutedEventArgs e)
        {
            UserMonitorThread.UserList.Add(
                new User(
                    LocalAccount.GetInstance.LocalIp.ToString(),
                    LocalAccount.GetInstance.LocalPort,
                    LocalAccount.GetInstance.LocalIp.ToString() + ":" + LocalAccount.GetInstance.LocalPort
                )
            );
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                // 删除所选用户
                UserMonitorThread.UserList.Remove(selectedUser);
                ChatStackPanel.Children.Clear();
            }
        }
    }
}