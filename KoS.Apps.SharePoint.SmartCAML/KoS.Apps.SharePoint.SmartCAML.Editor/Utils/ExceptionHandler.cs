using System;
using System.IO;
using System.Net;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class ExceptionHandler
    {
        public static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static string HandleConnection(Exception ex)
        {
            Telemetry.Instance.Native?.TrackException(ex);
            Log.Error(ex);
            StatusNotification.Notify("Connection failed");

            if (ex is FileNotFoundException) return HandleConnection((FileNotFoundException)ex);
            else if (ex is WebException) return HandleConnection((WebException)ex);
            else return ex.Message;
        }

        private static string HandleConnection(FileNotFoundException ex)
        {
            if (ex.Message.Contains("Microsoft.SharePoint"))
               return
                    "Could not load file or assembly 'Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c'.\n\n" +
                    "Make shure you are running application on SharePoint sever or change the connection type to 'Client' in 'advance settings'.";
            else
                return ex.Message;
        }

        private static string HandleConnection(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                return "Could not find the server. Please check the URL.";
            }
            else if (ex.Response is HttpWebResponse)
            {
                var response = (HttpWebResponse)ex.Response;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadGateway:
                        return "Could not find the server. Please check the URL.";
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        return "You are not authorized to open this site";
                    default:
                        return ex.Message;
                }
            }
            else
                return ex.Message;
        }

        public static void Handle(Exception ex, string message = null)
        {
            Telemetry.Instance.Native?.TrackException(ex);
            Log.Error(ex);
            MessageBox.Show($"{message}\n\n{ex.Message}", "SmartCAML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
