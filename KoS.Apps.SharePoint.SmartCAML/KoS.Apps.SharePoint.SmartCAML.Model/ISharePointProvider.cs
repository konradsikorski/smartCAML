using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public interface ISharePointProvider
    {
        Web Web { get; }
        Web Connect(string url);
        Web Connect(string url, string userName, string password);

        List<ListItem> ExecuteQuery(ListQuery query);
        void FillListFields(SList list);
    }
}
