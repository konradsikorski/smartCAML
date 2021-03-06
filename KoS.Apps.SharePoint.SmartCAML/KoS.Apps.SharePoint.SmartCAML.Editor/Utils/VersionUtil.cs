﻿using System.Diagnostics;
using System.Reflection;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class VersionUtil
    {
        public static string GetVersion()
        {
            var productVersion = (Debugger.IsAttached || !System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                ? FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion
                : System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            return productVersion;
        }
    }
}
