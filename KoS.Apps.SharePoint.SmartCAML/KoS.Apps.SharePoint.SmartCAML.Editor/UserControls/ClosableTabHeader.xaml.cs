using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ClosableTabHeader.xaml
    /// </summary>
    public partial class ClosableTabHeader : UserControl
    {
        public string HeaderText
        {
            get { return (string) ucTabName.Text; }
            set { ucTabName.Text = value; }
        }

        public bool CloseButtonVisible
        {
            get { return ucCloseButton.Visibility == Visibility.Visible; }
            set { ucCloseButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ClosableTabHeader()
        {
            InitializeComponent();
        }

        private void UcCloseButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ucCloseButton.Foreground = Brushes.Crimson;
        }

        private void UcCloseButton_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ucCloseButton.Foreground = Brushes.Gray;
        }

        private void UcCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.Tabs.Close");
            var tabItem = (TabItem) this.Parent;
            ((TabControl)tabItem.Parent).Items.Remove(tabItem);
        }
    }
}
