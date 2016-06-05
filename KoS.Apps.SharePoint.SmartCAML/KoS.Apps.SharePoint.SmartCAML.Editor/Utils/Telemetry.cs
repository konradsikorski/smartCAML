using System;
using Microsoft.ApplicationInsights;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    public class Telemetry
    {
        public TelemetryClient Native { get; private set; }

        private static Telemetry _instance;
        public static Telemetry Instance
        {
            get { return _instance ?? (_instance = new Telemetry()); }
        }

        private Telemetry()
        {
            Native = new TelemetryClient {InstrumentationKey = "5e6fcdc2-686f-4a2b-bb9a-8cee304ea210" };

            // Set session data:
            Native.Context.User.Id = Config.UserId;
            Native.Context.User.UserAgent = "SmartCAML v:" + VersionUtil.GetVersion();
            Native.Context.Session.Id = Guid.NewGuid().ToString();
            Native.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
        }

        public void Start()
        {
        }

        public void Close()
        {
            if (Native != null)
            {
                Native.Flush(); // only for desktop apps

                // Allow time for flushing:
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
