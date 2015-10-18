using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Collections.Generic;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryTab.xaml
    /// </summary>
    public partial class QueryTab : UserControl
    {
        public SList List { get; private set; }

        public QueryTab()
        {
            InitializeComponent();
        }

        public QueryTab(SList list)
        {
            InitializeComponent();
            List = list;
            ucItems.List = list;
            ucQueryBuilder.DataContext = List;
            ucQueryBuilder.Changed += (sender, args) => ucQuery.Text = ucQueryBuilder.Build().ToXml().ToString();
        }

        public ListQuery GetQuery()
        {
            var query = (XmlTab.IsSelected)
                ? ucQuery.Text
                : ucQueryBuilder.Build().ToXml().ToString();

            return new ListQuery { List = List, Query = query };
        }

        internal void QueryResult(List<ListItem> items)
        {
            ucItems.QueryResult(items);
        }
    }
}
