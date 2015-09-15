using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public enum SharePointProviderType
    {
        Fake = 0,
        SharePoint2010ServerModel = 1,
        SharePoint2010ClientModel = 2,
        SharePoint2013ServerModel = 3,
        SharePoint2013ClientModel = 4
    }
    public static class SharePointProviderFactory
    {
        public static ISharePointProvider Create(SharePointProviderType type)
        {
            switch (type)
            {
                case SharePointProviderType.Fake: return new FakeProvider();
                case SharePointProviderType.SharePoint2010ServerModel:
                case SharePointProviderType.SharePoint2010ClientModel:
                case SharePointProviderType.SharePoint2013ServerModel:
                case SharePointProviderType.SharePoint2013ClientModel:
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
