using System;
using System.Configuration;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    [Serializable]
    public class ConnectionSettings : ApplicationSettingsBase
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public SharePointProviderType ProviderType { get; set; }
    }
}
