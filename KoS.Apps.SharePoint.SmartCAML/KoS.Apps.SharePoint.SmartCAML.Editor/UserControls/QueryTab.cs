using System;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryTab.xaml
    /// </summary>
    public partial class QueryTab : UserControl
    {
        public SList List { get; private set; }
        public QueryTabConfig TabConfig { get; } = new QueryTabConfig();

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
            ucQueryBuilder.Changed += Designer_Changed;

            ucOrderByBuilder.DataContext = List;
            ucOrderByBuilder.Changed += Designer_Changed;

            DataObject.AddPastingHandler(ucQuery, UcQuery_OnPaste);
        }

        private void Designer_Changed(object sender, EventArgs e)
        {
            ucQuery.Text = BuildQuery().ToXml().ToString();
        }

        private void UcQuery_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;
            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            ucQuery.Tag = !string.IsNullOrWhiteSpace(text);
        }

        private void UcQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ucQuery.Text)) return;

            if (ucQuery.Tag as bool? == true)
            {
                ucQuery.Tag = null;
                TryPretyXmlPrint();
            }
        }

        private void TryPretyXmlPrint()
        {
            try
            {
                // check if it is valid xml
                ucQuery.Text = XDocument.Parse(ucQuery.Text).ToString();
            }
            catch
            {
                // ignored
            }
        }

        public ListQuery GetQuery()
        {
            var query = (XmlTab.IsSelected)
                ? ucQuery.Text
                : BuildQuery().ToXml().ToString();

            return new ListQuery { List = List, Query = query };
        }

        private Builder.ViewBuilder BuildQuery()
        {
            var query = new Builder.ViewBuilder();
            query.Filters.AddRange(ucQueryBuilder.GetFilters());
            query.OrderBy.AddRange(ucOrderByBuilder.GetOrders());
            return query;
        }

        internal void QueryResult(List<ListItem> items)
        {
            ucItems.SetFields();
            ucItems.QueryResult(items);
        }
    }

    public class QueryTabConfig : INotifyPropertyChanged
    {
        private bool _displayColymnsByTitle;

        public bool DisplayColymnsByTitle
        {
            get { return _displayColymnsByTitle; }
            set
            {
                _displayColymnsByTitle = value;
                OnPropertyChanged();
            }
        }

        public QueryTabConfig()
        {
            DisplayColymnsByTitle = Config.DisplayColumnsByTitle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
