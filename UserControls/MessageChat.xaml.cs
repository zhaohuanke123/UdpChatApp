using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace LinGuGu2.UserControls
{
    public partial class MessageChat : UserControl
    {
        public MessageChat()
        {
            InitializeComponent();
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageProperty = 
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(MessageChat));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(MessageChat));

    }
}
