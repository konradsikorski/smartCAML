using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryBuilderControl.xaml
    /// </summary>
    public partial class QueryBuilderControl : UserControl
    {
        public event EventHandler Changed;
        public OrderedList<QueryFilterControl> Controller { get; }

        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(nameof(DisplayColumnsByTitle), typeof(bool), typeof(QueryBuilderControl), null);
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

        public QueryBuilderControl()
        {
            InitializeComponent();
            Controller = new OrderedList<QueryFilterControl>(ucFilters);
            AddFilter();
        }

        private void AddFilterButton_Click(object sender, RoutedEventArgs e)
        {
            AddFilter();
        }

        private void AddFilter()
        {
            Controller
                .Add()
                .SetBinding(QueryFilterControl.DisplayColumnsByTitleProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayColumnsByTitle)),
                Mode = BindingMode.TwoWay
            });
        }

        public ViewBuilder Build()
        {
            var builder = new ViewBuilder();
            foreach (var control in ucFilters.Children.OfType<QueryFilterControl>())
            {
                control.BuildQuery(builder);
            }

            return builder;
        }
    }
}
