using System.Windows;
using System.Windows.Threading;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

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
            ExceptionHandler.Handle(dispatcherUnhandledExceptionEventArgs.Exception,
                "Unexpected error ocured. The application will be closed");
            //Shutdown();
        }
    }
}
