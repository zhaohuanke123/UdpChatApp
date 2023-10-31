using System.Windows;
using System.Windows.Controls;

namespace LinGuGu2.Helpers
{
    public class Resize
    {
        public static bool GetIsResizeable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsResizeableProperty);
        }

        public static void SetIsResizeable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsResizeableProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsResizeable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsResizeableProperty =
            DependencyProperty.RegisterAttached("IsResizeable", typeof(bool), typeof(Resize),
                new PropertyMetadata(false));

        public static ControlTemplate GetResizeTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(ResizeTemplateProperty);
        }

        public static void SetResizeTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ResizeTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ResizeTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeTemplateProperty =
            DependencyProperty.RegisterAttached("ResizeTemplate", typeof(ControlTemplate), typeof(Resize),
                new PropertyMetadata(null));
    }
}