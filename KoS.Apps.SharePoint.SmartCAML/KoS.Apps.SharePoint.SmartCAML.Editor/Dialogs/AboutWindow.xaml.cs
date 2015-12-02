using System;
using System.Deployment.Application;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System.Windows;
using System.Windows.Media;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private static bool _pendingRestart;

        public AboutWindow()
        {
            InitializeComponent();
            ucVersion.Text = VersionUtil.GetVersion();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/konradsikorski/smartCAML");
        }

        private async void AboutWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_pendingRestart)
            {
                UpdateStatusSuccess("Update completed. Restart the application.");
                return;
            }

            try
            {
                var updateInfo = await ClickOnceHelper.CheckNewVersion();

                if (updateInfo.UpdateAvailable)
                {
                    UpdateStatusMessage($"New version '{updateInfo.AvailableVersion.ToString(4)}' is available.");
                    ucUpdateButton.Visibility = Visibility.Visible;
                }
                else
                {
                    UpdateStatusMessage("Application is up to date.");
                }
            }
            catch(Exception ex)
            {
                UpdateStatusError("Could not check updates.", ex);
            }
        }

        private void UcUpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            ucUpdateMessage.Text = "Updating...";

            try
            {
                var update = ClickOnceHelper.DoUpdate();
                update.UpdateCompleted += (o, args) =>
                {
                    _pendingRestart = true;
                    UpdateStatusSuccess("Update completed. Restart the application.");
                };
                update.UpdateProgressChanged += (o, args) => ucUpdateMessage.Text = $"Updating {(args.BytesCompleted/args.BytesTotal):P}...";

            }
            catch (DeploymentDownloadException ex)
            {
                UpdateStatusError("Update failed.", ex);
            }
            finally
            {
                ucUpdateButton.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateStatusSuccess(string message)
        {
            ucUpdateMessage.Foreground = Brushes.ForestGreen;
            ucUpdateMessage.Text = message;
        }

        private void UpdateStatusError(string message, Exception ex)
        {
            ucUpdateMessage.Foreground = Brushes.DarkRed;
            ucUpdateMessage.Text = message;
        }

        private void UpdateStatusMessage(string message)
        {
            ucUpdateMessage.Foreground = Brushes.Gray;
            ucUpdateMessage.Text = message;
        }
    }
}
