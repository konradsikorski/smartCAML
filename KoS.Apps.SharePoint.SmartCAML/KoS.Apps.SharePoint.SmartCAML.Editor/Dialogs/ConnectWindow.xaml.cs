using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using System.Windows;
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
#endif

            this.DataContext = new ConnectWindowModel();
        }

        private void ucConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var client = SharePointProviderFactory.Create(Model.ProviderType);
            if( client.Connect(Model.SharePointWebUrl) != null)
            {
                Client = client;
                Model.AddNewUrl(Model.SharePointWebUrl);
                Model.Save();
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
    }
}
