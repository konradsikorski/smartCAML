using System.Diagnostics;
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
        private readonly Stopwatch _run;

        public App()
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            this.Exit += OnExit;
            Telemetry.Instance.Start();
            _run = Stopwatch.StartNew();
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            _run.Stop();
            Telemetry.Instance.Native.TrackMetric("RunDuration", _run.Elapsed.TotalMinutes);
            Telemetry.Instance.Close();

            Config.TotalRunTime = Config.TotalRunTime.Add(_run.Elapsed);
            Config.Save();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            ExceptionHandler.Handle(dispatcherUnhandledExceptionEventArgs.Exception,
                "Unexpected error ocured. The application will be closed");
        }
    }
}
