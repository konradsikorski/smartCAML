using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for OrderByBuilderControl.xaml
    /// </summary>
    public partial class OrderByBuilderControl : UserControl
    {
        public event EventHandler Changed;
        public OrderedList<OrderByFilterControl> Controller { get; }

        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(nameof(DisplayColumnsByTitle), typeof(bool), typeof(OrderByBuilderControl), null);
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

        public OrderByBuilderControl()
        {
            InitializeComponent();
            Controller = new OrderedList<OrderByFilterControl>(ucFilters);
            Controller.Changed += (sender, args) => Changed?.Invoke(sender, args);
        }

        private void AddOrderByButton_Click(object sender, RoutedEventArgs e)
        {
            AddFilter();
        }

        private void AddFilter(QueryOrderBy orderBy = null)
        {
            var filter = Controller.Add();
            filter.Refresh(orderBy);

            filter.SetBinding(OrderByFilterControl.DisplayColumnsByTitleProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayColumnsByTitle)),
                Mode = BindingMode.TwoWay
            });
        }

        internal IEnumerable<QueryOrderBy> GetOrders()
        {
            return ucFilters
                .Children
                .OfType<OrderByFilterControl>()
                .Select(c => c.GetOrder())
                .Where(f => f != null);
        }

        internal void Refresh(ViewBuilder view)
        {
            ucFilters.Children.Clear();

            foreach (var orderBy in view.OrderBy)
            {
                AddFilter(orderBy);
            }
        }
    }
}
