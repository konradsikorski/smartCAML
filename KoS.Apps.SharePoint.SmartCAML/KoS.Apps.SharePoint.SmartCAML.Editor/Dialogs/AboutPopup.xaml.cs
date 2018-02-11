using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System.Windows;
using System.Windows.Media;
using NLog;
using NLog.Targets;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : UserControl
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private static bool _pendingRestart;

        public AboutWindow()
        {
            Telemetry.Instance.Native?.TrackPageView("About");
            InitializeComponent();
            ucVersion.Text = VersionUtil.GetVersion();
        }


        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native?.TrackEvent("About.GitHub");
            Process.Start("https://github.com/konradsikorski/smartCAML");
        }

        private void LogsFileButton_Click(object sender, RoutedEventArgs e)
        {
            var logsTarget = LogManager.Configuration.FindTargetByName<FileTarget>("File");
            if (logsTarget == null) return;

            var logsFilePath = logsTarget.FileName.Render(null);

            if (File.Exists(logsFilePath))
                Process.Start(logsFilePath);
            else
                MessageBox.Show($"Could not find log file:\n{logsFilePath}");
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
                    Telemetry.Instance.Native?.TrackEvent("About.UpdateAvailable", new Dictionary<string, string>
                    {
                        {"currentVersion", VersionUtil.GetVersion() },
                        {"newVersion", updateInfo.AvailableVersion.ToString(4) }
                    });
                    UpdateStatusMessage($"New version '{updateInfo.AvailableVersion.ToString(4)}' is available.", true);
                    ucUpdateButton.Visibility = Visibility.Visible;
                }
                else
                {
                    UpdateStatusMessage("Application is up to date.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusError("Could not check updates, try again later.", ex);
            }
        }

        private void UcUpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native?.TrackEvent("About.Update");
            UpdateStatusMessage("Updating...", false);

            ClickOnceHelper.DoUpdateAsync(
                (o, args) =>
                {
                    if (args.Error != null)
                    {
                        UpdateStatusError("Update failed.", args.Error);
                    }
                    else if (args.Cancelled)
                    {
                        UpdateStatusSuccess("Update canceled.");
                    }
                    else
                    {
                        _pendingRestart = true;
                        UpdateStatusSuccess("Update completed. Restart the application.");
                    }
                },
                (o, args) =>
                {
                    ucUpdateMessage.Text = $"Updating {args.ProgressPercentage}%...";
                }
            );
        }

        private void UpdateStatusSuccess(string message, bool? installButtonVisible = null)
        {
            ucUpdateMessage.Foreground = Brushes.ForestGreen;
            ucUpdateMessage.Text = message;

            if (installButtonVisible.HasValue)
                ucUpdateButton.Visibility = installButtonVisible == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void UpdateStatusError(string message, Exception ex)
        {
            Log.Error(ex);

            ucUpdateMessage.Foreground = Brushes.DarkRed;
            ucUpdateMessage.Text = message;
        }

        private void UpdateStatusMessage(string message, bool? installButtonVisible = null)
        {
            ucUpdateMessage.Foreground = Brushes.Gray;
            ucUpdateMessage.Text = message;

            if (installButtonVisible.HasValue)
                ucUpdateButton.Visibility = installButtonVisible == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
