using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.UI.Xaml.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for SharePointLoginDIalog.xaml
    /// </summary>
    public partial class SharePointLoginDIalog : Window
    {
        private string _url;

        public SharePointLoginDIalog(string url)
        {
            _url = url;

            InitializeComponent();
        }

        private void ucBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //var c = Application.GetCookie(new Uri(_url));
            //ucBrowser.LoadCompleted
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ucBrowser.Navigate(new Uri(_url));
            ucWeb.Navigate(_url);

        }

        private void ucBrowser_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {

        }

        private void ucWeb_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ucWeb_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            if( e.Uri.AbsoluteUri == _url)
            {
                var c = Application.GetCookie(new Uri(_url));
            }
        }
    }
}
