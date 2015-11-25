using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class ImageButton : Button
    {
        private Image ImageControl { get; set; } = new Image();
        private TextBlock TextControl { get; set; } = new TextBlock();

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ImageButton), new FrameworkPropertyMetadata());
        public string Text
        {
            get { return (string)this.GetValue(ImageButton.TextProperty); }
            set { this.SetValue(ImageButton.TextProperty, (object)value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata());
        public ImageSource ImageSource
        {
            get{ return (ImageSource)this.GetValue(Image.SourceProperty); }
            set{ this.SetValue(Image.SourceProperty, (object)value); }
        }

        public ImageButton()
        {
            var imageSourceBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(ImageSource)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(ImageControl, Image.SourceProperty, imageSourceBinding);
            
            var textBlockTextBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Text)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(TextControl, TextBlock.TextProperty, textBlockTextBinding);

            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(ImageControl);
            stackPanel.Children.Add(TextControl);
        }
    }
}
