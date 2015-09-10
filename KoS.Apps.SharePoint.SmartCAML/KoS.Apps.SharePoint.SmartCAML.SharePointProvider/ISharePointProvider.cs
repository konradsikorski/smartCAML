using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public interface ISharePointProvider
    {
        bool Connect(string url);

        IEnumerable<SharePointList> GetLists();
    }
}
