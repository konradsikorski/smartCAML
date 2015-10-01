using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
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

        internal void QueryResult(List<ListItem> items)
        {
            ucItems.ItemsSource = items;
        }

        private void ucItems_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;

            //var item = (ListItem)e.Row.DataContext;
            //try
            //{
            //    item.Update();
            //}
            //catch (Exception)
            //{
            //    item.CancelChanges();
            //    e.Cancel = true;
            //    ucItems.CancelEdit();
            //}
        }
    }
}
