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
        }

        private void AddOrderByButton_Click(object sender, RoutedEventArgs e)
        {
            var filter = Controller.Add();

            filter.SetBinding(OrderByBuilderControl.DisplayColumnsByTitleProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayColumnsByTitle)),
                Mode = BindingMode.TwoWay
            });

            filter.Changed += (senderChanged, eChanged) => Changed?.Invoke(senderChanged, eChanged);
        }

        internal IEnumerable<QueryOrderBy> GetOrders()
        {
            return ucFilters
                .Children
                .OfType<OrderByFilterControl>()
                .Select(c => c.GetOrder())
                .Where(f => f != null);
        }
    }
}
