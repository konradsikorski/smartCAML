using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryTab.xaml
    /// </summary>
    public partial class QueryTab : UserControl
    {
        public SharePointList List { get; private set; }

        public QueryTab()
        {
            InitializeComponent();
        }

        public QueryTab(SharePointList list)
        {
            InitializeComponent();
            List = list;
        }

        public ListQuery GetQuery()
        {
            return new ListQuery { ListName = List.Name, Query = "" };
        }

        internal void QueryResult(List<SmartCAML.Model.ListItem> items)
        {
            ucItems.QueryResult(items);
        }
    }
}
