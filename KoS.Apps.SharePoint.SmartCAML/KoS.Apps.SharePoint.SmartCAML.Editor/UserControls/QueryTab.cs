using KoS.Apps.SharePoint.SmartCAML.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

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

            ucQueryDesigner.DataContext = List;
            ucQueryDesigner.Changed += Designer_Changed;
        }

        private void Designer_Changed(object sender)
        {
            ucQueryXml.Refresh( ucQueryDesigner.BuildQuery().ToXml().ToString() );
        }

        public ListQuery GetQuery()
        {
            var query = (XmlTab.IsSelected)
                ? ucQueryXml.GetQueryXml()
                : ucQueryDesigner.BuildQuery().ToXml().ToString();

            return new ListQuery { List = List, Query = query };
        }

        internal void QueryResult(List<ListItem> items)
        {
            ucItems.SetFields();
            ucItems.QueryResult(items);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( DesignerTab.IsSelected && ucQueryXml.Modified)
            {
                ucQueryXml.Modified = false;
                var view = ucQueryXml.BuildQuery();

                if (view != null)
                {
                    ucQueryDesigner.Refresh(view);
                }
            }
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
