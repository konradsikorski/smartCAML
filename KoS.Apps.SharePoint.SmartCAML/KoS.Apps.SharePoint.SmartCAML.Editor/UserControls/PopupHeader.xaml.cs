using System;
using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    public partial class PopupHeader : UserControl
    {
        public event Action<object, RoutedEventArgs> Closed;

        public PopupHeader()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(PopupHeader), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(sender, e);
        }
    }
}
