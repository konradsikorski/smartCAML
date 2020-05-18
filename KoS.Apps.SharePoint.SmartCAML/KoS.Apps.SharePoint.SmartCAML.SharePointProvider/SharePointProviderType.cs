﻿using System.ComponentModel;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public enum SharePointProviderType
    {
        [Description("Fake")]
        Fake = 0,
        [Description("SharePoint on-premisses")]
        SharePoint2013ClientModel = 2,
        [Description("SharePoint Online")]
        SharePointOnline = 3,
        [Description("SharePoint Online with MFA")]
        SharePointOnlineWithMFA = 4
    }
}
