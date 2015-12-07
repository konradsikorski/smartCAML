using System;
using System.IO;
using System.Net;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class ExceptionHandler
    {
        public static void HandleConnection(Exception ex)
        {
            if(ex is FileNotFoundException) HandleConnection((FileNotFoundException)ex);
            else if (ex is WebException) HandleConnection((WebException)ex);
            else
            {
                StatusNotification.Notify("Connection failed");
                MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void HandleConnection(FileNotFoundException ex)
        {
            StatusNotification.Notify("Connection failed");

            if (ex.Message.Contains("Microsoft.SharePoint"))
                MessageBox.Show("Could not load file or assembly 'Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c'.\n\n" +
                    "Make shure you are running application on SharePoint sever or change the connection type to 'Client' in 'advance settings'.", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void HandleConnection(WebException ex)
        {
            StatusNotification.Notify("Connection failed");

            if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                MessageBox.Show("Could not find the server. Please check the URL.", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if ( ex.Response is HttpWebResponse)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response.StatusCode == HttpStatusCode.BadGateway)
                    MessageBox.Show("Could not find the server. Please check the URL.", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    MessageBox.Show("You are not authorized to open this site", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show(ex.ToString(), "Connection failed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Handle(Exception ex, string message = null)
        {
            MessageBox.Show($"{message}\n\n{ex}", "SmartCAML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
