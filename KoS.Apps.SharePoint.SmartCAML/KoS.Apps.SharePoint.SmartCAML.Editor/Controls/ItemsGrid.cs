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
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ItemsGrid.xaml
    /// </summary>
    public partial class ItemsGrid : UserControl
    {
        private SmartCAML.Model.SList _list;

        public ItemsGrid()
        {
            InitializeComponent();
        }

        public SList List
        {
            get { return _list; }
            set
            {
                _list = value;

                foreach (var column in List.Fields.Select( c => new { Header = c.Title, Bind = c.InternalName }).OrderBy( c => c.Header))
                {
                    ucItems.Columns.Add( new DataGridTextColumn
                    {
                        Header = column.Header,
                        Width = 100,
                        Binding = new Binding($"[{column.Bind}]") { Mode = BindingMode.TwoWay}
                    });
                }
            }
        }

        internal void QueryResult(List<SmartCAML.Model.ListItem> items)
        {
            ucItems.ItemsSource = items;
        }
    }
}
