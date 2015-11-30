using System;
using System.ComponentModel;
using System.IO;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Model;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
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

            this.Width = Config.ConnectWindowWidth;
            this.DataContext = new ConnectWindowModel();
        }

        private async void ucConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var client = SharePointProviderFactory.Create(Model.ProviderType);

            try
            {
                Model.IsConnecting = true;
                StatusNotification.NotifyWithProgress("Connecting...");

                if (await client.Connect(Model.SharePointWebUrl, Model.UserName, Model.UserPassword) != null)
                {
                    Client = client;
                    Model.AddNewUrl(Model.SharePointWebUrl);
                    Model.AddUserToHistory();
                    Model.Save();
                    DialogResult = true;
                }
                StatusNotification.Notify("Connected");
            }
            catch (FileNotFoundException ex)
            {
                StatusNotification.Notify("Connection failed");

                if (ex.Message.Contains("Microsoft.SharePoint"))
                    MessageBox.Show("Could not load file or assembly 'Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c'.\n\n" +
                        "Make shure you are running application on SharePoint sever or change the connection type to 'Client' in 'advance settings'.", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusNotification.Notify("Connection failed");
                MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Model.IsConnecting = false;
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

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.UserPassword = ((PasswordBox) sender).Password;
        }

        private void ConnectWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Config.ConnectWindowWidth = this.Width;
        }
    }
}
