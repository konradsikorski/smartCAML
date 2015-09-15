using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public interface ISharePointProvider
    {
        Web Web { get; }
        Web  Connect(string url);

        List<ListItem> ExecuteQuery(ListQuery query);

        //IEnumerable<SharePointList> GetLists();
        //Web GetWeb();
    }
}
