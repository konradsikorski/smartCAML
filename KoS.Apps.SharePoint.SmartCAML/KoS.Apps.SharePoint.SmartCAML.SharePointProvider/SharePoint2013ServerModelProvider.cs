using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public class SharePoint2013ServerModelProvider : ISharePointProvider
    {
        public Web Web { get; }
        public Web Connect(string url)
        {
            throw new NotImplementedException();
        }
    }
}
