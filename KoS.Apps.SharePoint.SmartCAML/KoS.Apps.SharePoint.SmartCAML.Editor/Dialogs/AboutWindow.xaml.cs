using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            ucVersion.Text = VersionUtil.GetVersion();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/konradsikorski/smartCAML");
        }
    }
}
