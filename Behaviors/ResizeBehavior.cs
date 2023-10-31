using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LinGuGu2.Behaviors
{
    public class ResizeBehavior
    {
        public void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 获取关联的控件
            var border = d as Border;
            if (border == null) return;

            if ((bool)e.NewValue) // 如果IsResizeable属性被设置为true
            {
                // 1. 查找Thumb或生成Thumb，添加到Border的装饰层
                Thumb thumb = FindThumb(border);

                if (thumb == null)
                {
                    thumb = new Thumb();
                    thumb.Width = 10;
                    thumb.Height = 10;
                    thumb.DragDelta += Thumb_DragDelta;
                    border.Child = thumb;
                }

                // 2. 注册Thumb事件
                // Thumb_DragDelta 事件处理程序将用于调整Border的大小
            }
            else // 如果IsResizeable属性被设置为false
            {
                // 移除Thumb
                var thumb = border.Child as Thumb;
                if (thumb != null)
                {
                    border.Child = null;
                }
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var border = thumb.Parent as Border;

            if (border != null)
            {
                double left = Canvas.GetLeft(border);
                double top = Canvas.GetTop(border);

                if (double.IsNaN(left))
                {
                    left = 0;
                }

                if (double.IsNaN(top))
                {
                    top = 0;
                }

                left += e.HorizontalChange;
                top += e.VerticalChange;

                Canvas.SetLeft(border, left);
                Canvas.SetTop(border, top);
            }
        }

        private Thumb FindThumb(Border border)
        {
            foreach (var child in ((border.Parent as Grid)?.Children))
            {
                if (child is Thumb thumb)
                {
                    return thumb;
                }
            }

            return null;
        }
    }
}