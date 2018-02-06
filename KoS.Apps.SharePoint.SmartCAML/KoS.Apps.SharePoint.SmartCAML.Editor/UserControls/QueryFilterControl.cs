using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
using KoS.Apps.SharePoint.SmartCAML.Model;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryFilterControl.xaml
    /// </summary>
    public partial class QueryFilterControl : UserControl, IDisplayField, IOrderListElement
    {
        #region Fields
        private IFilter _initFilter;
        private IQueryFilterController _filterController;
        private IQueryFilterController FilterController
        {
            get { return _filterController; }
            set
            {
                var oldValue = _filterController?.GetValue();
                _filterController?.Dispose();
                if(_filterController != null) _filterController.ValueChanged -= FilterControllerOnValueChanged;

                _filterController = value;
                _filterController.Initialize(ucContainer, oldValue);
                _filterController.ValueChanged += FilterControllerOnValueChanged;

                // we have to check the IsLoaded because FilterController is set on Control initialization
                // and we dont want to send Change event because nothing was changed but only initialized
                if(IsLoaded) Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void FilterControllerOnValueChanged(object sender, EventArgs eventArgs)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public QueryOperator SelectedQueryOperator => ucAndOr.SelectedEnum<QueryOperator>().GetValueOrDefault();
        public Field SelectedField => (Field)ucField.SelectedItem;
        public FilterOperator? SelectedFilterOperator => ucFilterOperator.SelectedEnum<FilterOperator>().GetValueOrDefault();

        public event EventHandler RemoveClick;
        public event EventHandler Changed;
        public event EventHandler Up;
        public event EventHandler Down;

        public BoolToStringConverter DisplayMemberConverter => (BoolToStringConverter)this.Resources["DisplayMemberConverter"];
        public CollectionViewSource FieldsViewSource => (CollectionViewSource)this.Resources["FieldsViewSource"];
        private CollectionViewSource FilterOperatorViewSource => (CollectionViewSource) this.Resources["FilterOperatorViewSource"];

        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(nameof(DisplayColumnsByTitle), typeof(bool), typeof(QueryFilterControl), new FrameworkPropertyMetadata(DisplayColumnsByTitlePropertyChanged));
        private static void DisplayColumnsByTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (QueryFilterControl)d;
            var source = control.FieldsViewSource;
            var sortPropertyName = (string)control.DisplayMemberConverter.Convert(e.NewValue, typeof(string), null, CultureInfo.CurrentCulture);

            source.SortDescriptions.Clear();
            source.SortDescriptions.Add(new SortDescription(sortPropertyName, ListSortDirection.Ascending));
        }

        [Bindable(true)]
        public bool DisplayColumnsByTitle
        {
            get => (bool)GetValue(DisplayColumnsByTitleProperty);
            set => SetValue(DisplayColumnsByTitleProperty, value);
        }

        public FrameworkElement Control => this;

        #endregion
        #region Constructors

        public QueryFilterControl()
        {
            InitializeComponent();
            ucAndOr.BindToEnum<QueryOperator>(0);

            ConfigureFieldOperatorControl();
        }

        private void ConfigureFieldOperatorControl()
        {
            ucFilterOperator.BindToEnumUsingSource<FilterOperator>(FilterOperatorViewSource, FilterOperator.Eq);

            ucFilterOperator.SelectionChanged += (o, args) =>
            {
                FilterController = QueryFilterFactory.Create(SelectedField, SelectedFilterOperator);
            };
        }

        #endregion
        #region Event Handlers

        private void QueryFilterControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_initFilter != null) FilterController?.Refresh(_initFilter);
            
            // loaded event must be deattached becouse it fires every time when selected tab in QueryTab user control is changed
            this.Loaded -= QueryFilterControl_OnLoaded;
        }

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

        private void OperatorsViewSource_Filter(object sender, FilterEventArgs args)
        {
            if (SelectedField == null)
            {
                args.Accepted = false;
                return;
            }

            var fieldOperator = (KeyValuePair<FilterOperator, string>)args.Item;

            if ((fieldOperator.Key == FilterOperator.Includes || fieldOperator.Key == FilterOperator.NotIncludes)
                && (SelectedField.Type != FieldType.Lookup || !((FieldLookup)SelectedField).AllowMultivalue))
                args.Accepted = false;

            if (fieldOperator.Key == FilterOperator.Membership && SelectedField.Type != FieldType.User)
                args.Accepted = false;
        }

        private void ucField_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var field = SelectedField;

            if (field != null)
            {
                FilterController = QueryFilterFactory.Create(field, SelectedFilterOperator);
                FilterOperatorViewSource?.View.Refresh();
            }
        }

        #endregion

        public IFilter GetFilter()
        {
            return (SelectedField == null) 
                ? null
                : FilterController.GetFilter(SelectedQueryOperator);
        }

        public void Refresh(IFilter filter)
        {
            this._initFilter = filter;
            var list = (SList)DataContext;
            ucField.SelectedItem = list.Fields.FirstOrDefault(f => f.InternalName == filter.FieldInternalName);
            ucAndOr.SelectEnum(filter.QueryOperator);
            if(filter.QueryFilter.HasValue) ucFilterOperator.SelectEnum(filter.QueryFilter.Value);
        }
    }
}
