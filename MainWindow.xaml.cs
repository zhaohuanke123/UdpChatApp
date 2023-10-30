using System.Windows;
using System.Windows.Input;
using LinGuGu2.UserControls;

namespace LinGuGu2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        bool IsMaximized = false;

        private void Boreder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1250;
                    this.Height = 830;

                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    IsMaximized = true;
                }
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            MyMessageChat messagechat = new MyMessageChat();
            messagechat.Message = TxtMessage.Text;
            TxtMessage.Text = "";
            
            ChatStackPanel.Children.Add(messagechat);
            // 滑到最下面
            ChatScroll.ScrollToBottom();
        }
    }
}