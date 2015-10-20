using System;
using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    public static class StatusNotification
    {
        private static MainWindow MainWindow => (MainWindow) Application.Current.MainWindow;
        private static TextBlock StatusMessage => MainWindow.ucStatusMessage;
        private static ProgressBar StatusProgress => MainWindow.ucStatusProgress;

        public static void Notify(string message, int progressStep, int progressMax)
        {
            StatusMessage.Text = message;
            StatusProgress.Value = progressStep -1;
            StatusProgress.Maximum = progressMax;
            StatusProgress.IsIndeterminate = false;
            StatusProgress.ToolTip = $"{progressStep}/{progressMax}";
        }

        public static void Notify(string message)
        {
            StatusMessage.Text = message;
            StatusProgress.Value = 0;
            StatusProgress.Maximum = 100;
            StatusProgress.IsIndeterminate = false;
            StatusProgress.ToolTip = String.Empty;
        }

        public static void NotifyWithProgress(string message)
        {
            StatusMessage.Text = message;
            StatusProgress.Value = 0;
            StatusProgress.Maximum = 100;
            StatusProgress.ToolTip = String.Empty;
            StatusProgress.IsIndeterminate = true;
        }
    }
}
