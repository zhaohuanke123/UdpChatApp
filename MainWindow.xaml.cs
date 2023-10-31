using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LinGuGu2.Behaviors;
using LinGuGu2.Service;
using LinGuGu2.UserControls;

namespace LinGuGu2
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            
            Loaded += (s, e) =>
            {
                var layer = AdornerLayer.GetAdornerLayer(BackgroundBorder);
                layer.Add(new GridAdorner(BackgroundBorder));
            };
            

            UdpReceiveThread udpReceiveThread = new UdpReceiveThread("127.0.0.1", 6000);
            udpReceiveThread.ReceiveAction += (s =>
            {
                App.Current.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        PushAnMessage(s, false);
                    })
                    
                );
            });
            udpReceiveThread.StartReceive();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        bool _isMaximized = false;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (_isMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1250;
                    this.Height = 830;

                    _isMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    _isMaximized = true;
                }
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (TxtMessage.Text == "")
            {
                return;
            }

            PushAnMessage(TxtMessage.Text);
            UdpUtil.SendMsg(TxtMessage.Text);
            TxtMessage.Text = "";
        }

        private void PushAnMessage(String message, bool isMyMessage = true)
        {
            if (isMyMessage)
            {
                MyMessageChat messageChat = new MyMessageChat();
                messageChat.Message = message;

                ChatStackPanel.Children.Add(messageChat);
                Console.WriteLine("Add a message form me");
            }
            else
            {
                MessageChat messageChat = new MessageChat{Message = message,Color = Brushes.Red};

                ChatStackPanel.Children.Add(messageChat);
                Console.WriteLine("Add a message form other");
            }

            // 滑到最下面
            ChatScroll.ScrollToBottom();
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            // 最下化窗口
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            // 最大化窗口
            if (_isMaximized)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1250;
                this.Height = 830;

                _isMaximized = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                _isMaximized = true;
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // 关闭窗口
            Close();
        }
    }
}