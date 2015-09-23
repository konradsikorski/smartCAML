using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
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
using KoS.Apps.SharePoint.SmartCAML.Editor.Model;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ISharePointProvider Client { get; private set; }
        public ConnectWindowModel Model => (ConnectWindowModel) this.DataContext;

        public ConnectWindow()
        {
            InitializeComponent();

#if DEBUG
            ucApiFake.Visibility = Visibility.Visible;
            //ucApiFake.IsChecked = true;
#endif

            this.DataContext = new ConnectWindowModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ucSharePointUrl.ItemsSource = Config.LastSharePointUrl.ToList();
            if (ucSharePointUrl.Items.Count > 0) ucSharePointUrl.SelectedIndex = 0;
        }

        private void ucConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var client = SharePointProviderFactory.Create(Model.ProviderType);
            if( client.Connect(ucSharePointUrl.Text) != null)
            {
                Client = client;
                AddNewUrl(ucSharePointUrl.Text);
                DialogResult = true;
            }
        }

        private void ucCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AdvanceOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ShowAdvanceOptions = true;
        }

        private void HideAdvanceOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ShowAdvanceOptions = false;
        }

        private void AddNewUrl(string url)
        {
            var list = ((List<string>)ucSharePointUrl.ItemsSource);

            var index = list.IndexOf(url);
            if (index >= 0) list.RemoveAt(index);
            list.Insert(0, url);

            Config.LastSharePointUrl = ucSharePointUrl.Items.Cast<string>();
        }
    }
}
