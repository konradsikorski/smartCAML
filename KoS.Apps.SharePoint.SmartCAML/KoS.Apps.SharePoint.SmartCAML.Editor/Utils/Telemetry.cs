using System;
using System.Threading;
using System.Threading.Tasks;
#if OLD_ApplicationInsigts
using Microsoft.ApplicationInsights;
#endif

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    public class Telemetry
    {
        private Task _activeUserTask;
        private CancellationTokenSource _activeUserTaskCancelation;
#if OLD_ApplicationInsigts
        public TelemetryClient Native { get; private set; }
#else
        public dynamic Native = null;
#endif
        private static Telemetry _instance;
        public static Telemetry Instance
        {
            get { return _instance ?? (_instance = new Telemetry()); }
        }

        private Telemetry()
        {
#if OLD_ApplicationInsigts
#if Debug
            return;
#endif
            Native = new TelemetryClient {InstrumentationKey = "5e6fcdc2-686f-4a2b-bb9a-8cee304ea210" };

            // Set session data:
            Native.Context.User.Id = Config.UserId;
            Native.Context.User.UserAgent = "SmartCAML/" + VersionUtil.GetVersion();
            Native.Context.User.AccountId = Config.UserId;
            Native.Context.Session.Id = Guid.NewGuid().ToString();
            Native.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            Native.Context.Device.Language = Thread.CurrentThread.CurrentUICulture.Name;
            Native.Context.Device.ScreenResolution = $"{System.Windows.SystemParameters.PrimaryScreenWidth}x{System.Windows.SystemParameters.PrimaryScreenHeight}";
            Native.Context.Component.Version = VersionUtil.GetVersion();
#endif
        }

        public void Start()
        {
            _activeUserTaskCancelation = new CancellationTokenSource();
            var cancelationToken = _activeUserTaskCancelation.Token;
            _activeUserTask = Task.Run(() =>
            {
                while (!cancelationToken.IsCancellationRequested)
                {
                    Native?.TrackMetric("ActiveUser", 1);
                    Thread.Sleep(60000);
                }
            }, _activeUserTaskCancelation.Token);
        }

        public void Close()
        {
            if (_activeUserTask != null)
            {
                _activeUserTaskCancelation.Cancel();
                _activeUserTask = null;
            }

            if (Native != null)
            {
                Native?.Flush(); // only for desktop apps

                // Allow time for flushing:
                Thread.Sleep(1000);
            }
        }

        public void TrackEvent(string eventName, System.Collections.Generic.Dictionary<string, string> dictionary = null)
        {
            Telemetry.Instance.Native?.TrackEvent("Main.CloseQueryTab", dictionary);
        }

        public void TrackPageView(string pageName)
        {
            Telemetry.Instance.Native?.TrackPageView("Main");
        }
    }
}
