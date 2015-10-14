using System.Windows;
using System.Windows.Threading;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            var message = "Unexpected error ocured. The application will be closed.\n\n" +
                          dispatcherUnhandledExceptionEventArgs.Exception;

            MessageBox.Show(message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //Shutdown();
        }
    }
}
