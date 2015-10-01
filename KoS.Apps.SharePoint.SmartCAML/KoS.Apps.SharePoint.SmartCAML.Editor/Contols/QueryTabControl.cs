using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Contols;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class QueryTabControl : TabControl
    {
        public QueryTab SelectedQueryTab => (QueryTab)((TabItem)this.SelectedItem)?.Content;
         
        public void AddQuery(SList list)
        {
            if (list == null) return;
            if(list.Fields?.Count == 0) list.Web.Client.FillListFields(list);

            this.SelectedIndex = this.Items.Add(
                new ClosableTabItem
                {
                    HeaderText = list.Title,
                    Content = new QueryTab(list) {Margin = new Thickness(4) }
                });

            CommandManager.InvalidateRequerySuggested();
        }
    }
}
