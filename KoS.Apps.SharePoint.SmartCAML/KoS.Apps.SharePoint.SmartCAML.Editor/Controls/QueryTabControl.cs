using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class QueryTabControl : TabControl
    {
        public QueryTab SelectedQueryTab => (QueryTab)((TabItem)this.SelectedItem).Content;
         
        public void AddQuery(SharePointList list)
        {
            if (list == null) return;
            var index = this.Items.Add(new TabItem { Content = new QueryTab(list), Header = list.Name });
            this.SelectedIndex = index;
        }
    }
}
