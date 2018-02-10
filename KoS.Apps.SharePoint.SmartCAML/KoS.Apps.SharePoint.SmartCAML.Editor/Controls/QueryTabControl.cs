using System;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class QueryTabControl : TabControl
    {
        public QueryTab SelectedQueryTab => ToQueryTab(this.SelectedItem);

        public IEnumerable<QueryTab> AllTabs
        {
            get {
                foreach (var item in this.Items)
                {
                    yield return (QueryTab)((TabItem)item)?.Content;
                }
            }
        }
         
        public async Task AddQuery(SList list)
        {
            if (list == null) return;
            if (list.Fields?.Count == 0)
            {
                try
                {
                    StatusNotification.NotifyWithProgress("Prepairing list: " + list.Title);
                    await list.Web.Client.FillListFields(list);
                    StatusNotification.Notify("List ready");
                }
                catch (Exception)
                {
                    StatusNotification.Notify("Prepairing list failed");
                    return;
                }
            }

            this.SelectedIndex = this.Items.Add(
                new ClosableTabItem
                {
                    HeaderText = list.Title,
                    Content = new QueryTab(list) {Margin = new Thickness(4) }
                });

            CommandManager.InvalidateRequerySuggested();
        }

        public void CloseQuery(QueryTab tabQuery)
        {
            if (tabQuery == null) return;
            if (tabQuery.Parent == null && !(tabQuery.Parent is TabItem)) return;

            this.Items.Remove(tabQuery.Parent);
        }

        public void CloseWeb(Web web)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var queryTab = ToQueryTab(this.Items[i]);
                if(queryTab.List.Web == web) this.Items.RemoveAt(i--);
            }
        }

        public QueryTab ToQueryTab(object tab)
        {
            return (QueryTab) ((TabItem) tab)?.Content;
        }
    }
}
