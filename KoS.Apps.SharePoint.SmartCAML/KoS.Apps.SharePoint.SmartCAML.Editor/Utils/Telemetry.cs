using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    public class Telemetry
    {
        private Task _activeUserTask;
        private CancellationTokenSource _activeUserTaskCancelation;
        public INative Native { get; private set; }

        private static Telemetry _instance;
        public static Telemetry Instance
        {
            get { return _instance ?? (_instance = new Telemetry()); }
        }

        private Telemetry()
        {
#if Debug
            return;
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
                Native.Flush(); // only for desktop apps

                // Allow time for flushing:
                Thread.Sleep(1000);
            }
        }
    }

    public interface INative
    {
        void TrackEvent(string name, Dictionary<string, string> arguments = null);
        void TrackPageView(string name);
        void TrackMetric(string name, double value);
        void TrackException(Exception exception);
        void Flush();
    }
}
