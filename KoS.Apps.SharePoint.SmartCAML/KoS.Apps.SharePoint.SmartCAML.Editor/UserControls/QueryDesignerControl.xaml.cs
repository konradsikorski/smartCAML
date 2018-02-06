using System;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for QueryDesignerControl.xaml
    /// </summary>
    public partial class QueryDesignerControl : UserControl
    {
        public bool Modified { get; set; }

        public QueryDesignerControl()
        {
            InitializeComponent();

            ucQueryBuilder.Changed += (sender, args) => Modified = true;
            ucOrderByBuilder.Changed += (sender, args) => Modified = true;
        }

        public ViewBuilder BuildQuery()
        {
            var query = new ViewBuilder();
            query.Filters.AddRange(ucQueryBuilder.GetFilters());
            query.OrderBy.AddRange(ucOrderByBuilder.GetOrders());
            return query;
        }

        public void Refresh(ViewBuilder view)
        {
            ucQueryBuilder.Refresh(view);
            ucOrderByBuilder.Refresh(view);
            Modified = false;
        }
    }
}
