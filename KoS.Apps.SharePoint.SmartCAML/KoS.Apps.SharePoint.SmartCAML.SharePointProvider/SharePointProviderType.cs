using System.ComponentModel;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public enum SharePointProviderType
    {
        [Description("Fake")]
        Fake = 0,
        [Description("SharePoint Client")]
        SharePoint2013ClientModel = 2,
        [Description("SharePoint Online")]
        SharePointOnline = 3
    }
}
