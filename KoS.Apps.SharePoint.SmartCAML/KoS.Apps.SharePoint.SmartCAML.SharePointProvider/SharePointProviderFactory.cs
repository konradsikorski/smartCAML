using System;
using KoS.Apps.SharePoint.SmartCAML.Model;
using KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider;
using KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ServerProvider;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public static class SharePointProviderFactory
    {
        public static ISharePointProvider Create(SharePointProviderType type)
        {
            switch (type)
            {
                case SharePointProviderType.Fake: return new FakeProvider();
                case SharePointProviderType.SharePoint2013ClientModel: return new SharePoint2013ClientModelProvider();
                case SharePointProviderType.SharePointOnline: return new SharePoint2013ClientModelProvider(true);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
