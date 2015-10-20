using System;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Contols;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class QueryTabControl : TabControl
    {
        public QueryTab SelectedQueryTab => ToQueryTab(this.SelectedItem);
         
        public void AddQuery(SList list)
        {
            if (list == null) return;
            if (list.Fields?.Count == 0)
            {
                try
                {
                    StatusNotification.NotifyWithProgress("Prepairing list: " + list.Title);
                    list.Web.Client.FillListFields(list);
                    StatusNotification.Notify("List ready" + list.Title);
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
