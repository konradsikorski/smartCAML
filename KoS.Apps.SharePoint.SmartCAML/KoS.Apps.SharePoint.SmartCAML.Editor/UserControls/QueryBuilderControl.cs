using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryBuilderControl.xaml
    /// </summary>
    public partial class QueryBuilderControl : UserControl
    {
        public event EventHandler Changed;

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
            AddFilter();
        }

        private void AddFilterButton_Click(object sender, RoutedEventArgs e)
        {
            AddFilter();
        }

        private void QueryFilterControl_OnRemoveClick(object sender, EventArgs e)
        {
            ucFilters.Children.Remove((UIElement)sender);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void AddFilter()
        {
            var filterControl = new QueryFilterControl();
            filterControl.RemoveClick += QueryFilterControl_OnRemoveClick;
            filterControl.Changed += (sender, args) => Changed?.Invoke(this, EventArgs.Empty);
            filterControl.Up += MoveFilterUp;
            filterControl.Down += MoveFilterDown;
            filterControl.SetBinding(QueryFilterControl.DisplayColumnsByTitleProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayColumnsByTitle)),
                Mode = BindingMode.TwoWay
            });

            ucFilters.Children.Add(filterControl);
        }

        private void MoveFilterDown(object control, EventArgs eventArgs)
        {
            var filterControl = (UIElement)control;
            var index = ucFilters.Children.IndexOf(filterControl);

            if (index > 0)
                MoveFilter(filterControl, index + 1);
        }

        private void MoveFilterUp(object control, EventArgs eventArgs)
        {
            var filterControl = (UIElement)control;
            var index = ucFilters.Children.IndexOf(filterControl);

            if (index > 0)
                MoveFilter(filterControl, index - 1);
        }

        private void MoveFilter(UIElement filterControl, int newIndex)
        {
            ucFilters.Children.Remove(filterControl);
            ucFilters.Children.Insert(newIndex, filterControl);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public QueryBuilder Build()
        {
            var builder = new QueryBuilder();
            foreach (var control in ucFilters.Children.OfType<QueryFilterControl>())
            {
                control.BuildQuery(builder);
            }

            return builder;
        }
    }
}
