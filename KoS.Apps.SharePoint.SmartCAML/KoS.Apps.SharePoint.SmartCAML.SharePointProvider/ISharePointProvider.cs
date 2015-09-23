using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public interface ISharePointProvider
    {
        Web Web { get; set; }
        Web Connect(string url);

        List<ListItem> ExecuteQuery(ListQuery query);
        void FillListFields(SList list);
    }
}
