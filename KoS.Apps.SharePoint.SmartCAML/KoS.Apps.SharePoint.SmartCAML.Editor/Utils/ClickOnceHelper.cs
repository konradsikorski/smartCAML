using System;
using System.Collections.Generic;
using System.ComponentModel;
#if OLD_ClickOnce
using System.Deployment.Application;
#endif
using System.Threading.Tasks;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class ClickOnceHelper
    {
#if OLD_ClickOnce
        public static async Task<UpdateCheckInfo> CheckNewVersion()
        {
            
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                
                return await Task.Factory.StartNew( () => ad.CheckForDetailedUpdate());
            }

            throw new Exception();
       }

        public static ApplicationDeployment DoUpdateAsync(AsyncCompletedEventHandler onComplete = null, DeploymentProgressChangedEventHandler onProgress = null)
        {
            var ad = ApplicationDeployment.CurrentDeployment;
            if (onComplete != null) ad.UpdateCompleted += onComplete;
            if (onProgress != null) ad.UpdateProgressChanged += onProgress;
            ad.UpdateAsync();
            return ad;
        }

        public static void RestartApplication()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
#endif
    }
}
