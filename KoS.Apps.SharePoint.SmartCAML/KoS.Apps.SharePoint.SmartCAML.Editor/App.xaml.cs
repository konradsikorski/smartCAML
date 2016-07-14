using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.ServiceProxy;
using NLog;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch _run;

        public App()
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            this.Exit += OnExit;
            Telemetry.Instance.Start();
            _run = Stopwatch.StartNew();

            CheckIfUpdated();
        }

        private void CheckIfUpdated()
        {
            var currentVersion = VersionUtil.GetVersion();
            if (currentVersion == Config.LastVersion) return;

            Func<Task> task;
            if( string.IsNullOrEmpty(Config.LastVersion))
                 task = async  () =>
                {
                    try
                    {
                        await new ServiceClient(Config.ServiceAddress).InstallationCompleted(currentVersion);
                        //TODO: move to task continue
                        Config.LastVersion = currentVersion;
                        Config.Save();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw;
                    }
                };
            else
                task = async () =>
                {
                    try
                    {
                        await new ServiceClient(Config.ServiceAddress).UpdateCompleted(currentVersion);
                        //TODO: move to task continue
                        Config.LastVersion = currentVersion;
                        Config.Save();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw;
                    }
                };

            Task.Factory.StartNew(task);
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
