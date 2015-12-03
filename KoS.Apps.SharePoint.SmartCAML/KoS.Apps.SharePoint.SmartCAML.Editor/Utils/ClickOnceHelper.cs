using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Threading.Tasks;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class ClickOnceHelper
    {
        public static async Task<UpdateCheckInfo> CheckNewVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;

                //try
                //{
                    return await Task.Factory.StartNew( () => ad.CheckForDetailedUpdate());
                //}
                //catch (DeploymentDownloadException dde)
                //{
                //    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                //    return;
                //}
                //catch (InvalidDeploymentException ide)
                //{
                //    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                //    return;
                //}
                //catch (InvalidOperationException ioe)
                //{
                //    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                //    return;
                //}
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
    }
}
