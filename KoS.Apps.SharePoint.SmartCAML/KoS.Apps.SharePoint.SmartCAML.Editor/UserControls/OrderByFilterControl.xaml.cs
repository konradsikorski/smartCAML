using KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for OrderByFilterControl.xaml
    /// </summary>
    public partial class OrderByFilterControl : UserControl, IOrderListElement
    {
        public OrderByDirection SelectedDirection => ucOrderDirection.SelectedEnum<OrderByDirection>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;

        public BoolToStringConverter DisplayMemberConverter => (BoolToStringConverter)this.Resources["DisplayMemberConverter"];
        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(nameof(DisplayColumnsByTitle), typeof(bool), typeof(OrderByFilterControl), new FrameworkPropertyMetadata(DisplayColumnsByTitlePropertyChanged));
        private static void DisplayColumnsByTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (OrderByFilterControl)d;
            var source = control.FieldsViewSource;
            var sortPropertyName = (string)control.DisplayMemberConverter.Convert(e.NewValue, typeof(string), null, CultureInfo.CurrentCulture);

            source.SortDescriptions.Clear();
            source.SortDescriptions.Add(new SortDescription(sortPropertyName, ListSortDirection.Ascending));
        }

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

        public CollectionViewSource FieldsViewSource => (CollectionViewSource)this.Resources["FieldsViewSource"];

        public OrderByFilterControl()
        {
            InitializeComponent();
            ucOrderDirection.BindToEnum<OrderByDirection>(0);
        }

        public FrameworkElement Control => this;

        public event EventHandler RemoveClick;
        public event EventHandler Changed;
        public event EventHandler Up;
        public event EventHandler Down;

        private void UpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Up?.Invoke(this, EventArgs.Empty);
        }

        private void DownButton_OnClick(object sender, RoutedEventArgs e)
        {
            Down?.Invoke(this, EventArgs.Empty);
        }
        private void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public QueryOrderBy GetOrder()
        {
            return SelectedField == null
                ? null
                : new QueryOrderBy
                {
                    Direction = SelectedDirection,
                    FieldName = SelectedField?.InternalName
                };
        }
    }
}
