using System;
using System.ComponentModel;
using System.Windows;
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


        #region Dependency Property

        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(
            nameof(DisplayColumnsByTitle), 
            typeof(bool), 
            typeof(QueryDesignerControl),
            null);

        [Bindable(true)]
        public bool DisplayColumnsByTitle
        {
            get
            {
                return (bool)this.GetValue(DisplayColumnsByTitleProperty);
            }
            set
            {
                this.SetValue(DisplayColumnsByTitleProperty, value);
            }
        }
        #endregion

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
