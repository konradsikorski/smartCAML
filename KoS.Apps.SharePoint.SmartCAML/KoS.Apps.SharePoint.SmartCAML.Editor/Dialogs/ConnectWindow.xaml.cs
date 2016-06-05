using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
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
            Telemetry.Instance.Native.TrackPageView("Connect");
            InitializeComponent();

#if DEBUG
            ucApiFake.Visibility = Visibility.Visible;
#endif

            this.Width = Config.ConnectWindowWidth;
            this.DataContext = new ConnectWindowModel();
        }

        private async void ucConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackEvent("OK", new Dictionary<string, string>
            {
                {"ProviderType", Model.ProviderType.ToString() },
                {"UseCurrentUser", Model.UseCurrentUser.ToString() }
            });

            var client = SharePointProviderFactory.Create(Model.ProviderType);

            try
            {
                Model.IsConnecting = true;
                StatusNotification.NotifyWithProgress("Connecting...");

                var userName = !Model.UseCurrentUser ? Model.UserName : null;
                var userPassword = !Model.UseCurrentUser ? Model.UserPassword: null;

                if (await client.Connect(Model.SharePointWebUrl, userName, userPassword) != null)
                {
                    Client = client;
                    Model.AddNewUrl(Model.SharePointWebUrl);
                    Model.AddUserToHistory();
                    Model.Save();
                    DialogResult = true;
                }
                StatusNotification.Notify("Connected");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleConnection(ex);
            }

            Model.IsConnecting = false;
        }

        private void ucCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackEvent("Cancel");
            DialogResult = false;
        }

        private void AdvanceOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackEvent("AdvanceOptions");
            Model.ShowAdvanceOptions = true;
        }

        private void HideAdvanceOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackEvent("AdvanceOptions");
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
