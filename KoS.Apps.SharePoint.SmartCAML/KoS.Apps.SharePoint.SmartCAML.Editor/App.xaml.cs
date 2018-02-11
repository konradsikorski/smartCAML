using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using KoS.Apps.SharePoint.SmartCAML.Editor.Properties;
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
            _run = Stopwatch.StartNew();

            Log.Info($"User settings file location: {ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath}");

            CheckIfUpdated();
            Telemetry.Instance.Start();
        }

        private void CheckIfUpdated()
        {
            var currentVersion = VersionUtil.GetVersion();
            Log.Info($"Application Started | currentVersion:{currentVersion} | lastVersion:{Config.LastVersion}");

            if (currentVersion != Config.LastVersion)
            {
                Settings.Default.Upgrade();
                Config.LastVersion = currentVersion;
                Config.InstallationCompleted = false;
                Config.Save();
            }

            if (!Config.InstallationCompleted) CompleteInstallation(currentVersion);
        }

        private void CompleteInstallation(string currentVersion)
        {
            var serviceTask = string.IsNullOrEmpty(Config.LastVersion)
                ? (Func<Task>)(async () => await new ServiceClient(Config.ServiceAddress).InstallationCompleted(currentVersion))
                : (Func<Task>)(async () => await new ServiceClient(Config.ServiceAddress).UpdateCompleted(currentVersion));

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    Log.Info($"Calling service: {Config.ServiceAddress}");
                    await serviceTask();
                    Config.InstallationCompleted = true;
                    Config.Save();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            _run.Stop();
            Telemetry.Instance.Native?.TrackMetric("RunDuration", _run.Elapsed.TotalMinutes);
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
