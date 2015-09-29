using System.Diagnostics;
using System.Reflection;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class VersionUtil
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            return version;
        }
    }
}
