using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Xceed.Wpf.Toolkit;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryFilterControl.xaml
    /// </summary>
    public partial class QueryFilterControl : UserControl
    {
        //public ComboBox ucFilterOperator;
        public Control ucFieldValue;
        public ComboBox ucLookupAs;
        public CheckBox ucIncludeTime;

        public QueryOperator SelectedQueryOperator => ucAndOr.SelectedEnum<QueryOperator>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;
        public FilterOperator? SelectedFilterOperator => ucFilterOperator.SelectedEnum<FilterOperator>().GetValueOrDefault();
        public string SelectedValue => ValueSelector();

        private Func<string> ValueSelector = () => null;
        private Func<QueryOptions> QueryOptions = () => null;  

        public event EventHandler RemoveClick;
        public event EventHandler Changed;
        public event EventHandler Up;
        public event EventHandler Down;

        private int _controlWidth = 100;
        private Thickness _controlMargin = new Thickness(4, 0, 0, 0);

        private CollectionViewSource FilterOperatorViewSource => (CollectionViewSource) this.Resources["FilterOperatorViewSource"];

        public QueryFilterControl()
        {
            InitializeComponent();
            ucAndOr.BindToEnum<QueryOperator>(0);

            ConfigureFieldOperatorControl();
        }

        private void UpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Up?.Invoke(this, EventArgs.Empty);
        }

        private void DownButton_OnClick(object sender, RoutedEventArgs e)
        {
            Down?.Invoke(this, EventArgs.Empty);
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
        }

        private void ucField_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var field = SelectedField;

            if (field != null)
            {
                if (ucLookupAs != null) ucContainer.Children.Remove(ucLookupAs);
                if (ucIncludeTime != null) ucContainer.Children.Remove(ucIncludeTime);
                RefreshValueField(field);
                
                FilterOperatorViewSource?.View.Refresh();
            }
        }

        private void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigureFieldOperatorControl()
        {
            ucFilterOperator.BindToEnumUsingSource<FilterOperator>(FilterOperatorViewSource, FilterOperator.Eq);

            ucFilterOperator.SelectionChanged += (o, args) =>
            {
                if (SelectedFilterOperator == FilterOperator.IsNotNull || SelectedFilterOperator == FilterOperator.IsNull)
                {
                    if (ucFieldValue != null)
                    {
                        ucContainer.Children.Remove(ucFieldValue);
                        ucFieldValue = null;
                        ValueSelector = () => null;
                    }

                    if (ucLookupAs != null)
                    {
                        ucContainer.Children.Remove(ucLookupAs);
                        ucLookupAs = null;
                        QueryOptions = () => null;
                    }
                }
                else if (ucFieldValue == null) RefreshValueField(SelectedField);

                Changed?.Invoke(this, EventArgs.Empty);
            };
        }

        private void RefreshValueField(Field field)
        {
            if (ucFieldValue != null) ucContainer.Children.Remove(ucFieldValue);
            ucFieldValue = BuildFieldValueControl(field);

            if (ucFieldValue != null) ucContainer.Children.Add(ucFieldValue);
        }

        public void BuildQuery(QueryBuilder builder)
        {
            if (SelectedField == null) return;

            builder.New(SelectedQueryOperator, SelectedFilterOperator, SelectedField.Type , SelectedField.InternalName,
                SelectedValue, QueryOptions());
        }

        private Control BuildFieldValueControl(Field field)
        {
            var oldValue = ValueSelector();
            QueryOptions = () => null;

            switch (field.Type)
            {
                case FieldType.DateTime:
                    return CreateDateTime(field, oldValue);
                case FieldType.Choice:
                    return CreateChoice(field, oldValue);
                case FieldType.Boolean:
                    return CreateBoolean(oldValue);
                case FieldType.User:
                    return CreateUser(oldValue);
                case FieldType.ContentTypeId:
                    return CreateContentTypeId(field, oldValue);
                case FieldType.Lookup:
                    return CreateLookup(oldValue);

                default:
                    return CreateDropDown(oldValue);
            }
        }

        private Control CreateLookup(string oldValue)
        {
            var control = CreateDropDown(oldValue);

            ucLookupAs = new ComboBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                IsEditable = false,
                ItemsSource = new[] { "by lookup id", "by lookup text" },
                SelectedIndex = 0
            };
            QueryOptions = () => new QueryOptions {IsLookupId = ucLookupAs.SelectedIndex == 0};
            ucLookupAs.SelectionChanged += (sender, args) =>  Changed?.Invoke(this, EventArgs.Empty);

            ucContainer.Children.Add(ucLookupAs);

            return control;
        }

        private Control CreateContentTypeId(Field field, string oldValue)
        {
            var control = CreateDropDown(oldValue);

            control.DisplayMemberPath = "Name";
            control.SelectedValuePath = "Id";

            control.DropDownOpened += async (sender, args) =>
            {
                if (field.List.ContentTypes == null || field.List.Web.ContentTypes == null)
                {
                    StatusNotification.NotifyWithProgress("Loading Content Types");
                    await field.List.Web.Client.FillContentTypes(field.List);
                    StatusNotification.Notify("Content Types loaded");
                }

                var contentTypes = field.List.ContentTypes
                    .OrderBy(ct => ct.Name)
                    .Concat(field.List.Web.ContentTypes.OrderBy(ct => ct.Name));

                control.ItemsSource =
                    contentTypes.Select(ct => new
                    {
                        ct.Id,
                        Name = field.List.ContentTypes.Contains(ct) ? "List." + ct.Name : ct.Name
                    });
            };

            return control;
        }

        private Control CreateUser(string oldValue)
        {
            var control = CreateDropDown(oldValue);
            control.ItemsSource = new[] { "@Me", "Browse..." };

            return control;
        }

        private Control CreateBoolean(string oldValue)
        {
            var control = CreateDropDown(oldValue);
            control.DisplayMemberPath = "Text";
            control.SelectedValuePath = "Value";
            control.ItemsSource = new[]
            {
                        new {Value = "0", Text = "False"},
                        new {Value = "1", Text = "True"}
                    };

            return control;
        }

        private Control CreateChoice(Field field, string oldValue)
        {
            var control = CreateDropDown(oldValue);
            control.ItemsSource = ((FieldChoice) field).Choices.OrderBy(c => c);

            return control;
        }

        private Control CreateDateTime(Field field, string oldValue)
        {
            // todo: http://www.codeproject.com/Articles/414414/SharePoint-Working-with-Dates-in-CAML
            if (ucIncludeTime != null) ucContainer.Children.Remove(ucIncludeTime);
            ucIncludeTime = new CheckBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                Padding = new Thickness(0, 2, 0, 0),
                Content = "Include time",
                IsChecked = !((FieldDateTime)field).DateOnly
            };

            ucContainer.Children.Add(ucIncludeTime);

            //-----
            var control = new DateTimePicker
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,

                Format = DateTimeFormat.Custom,
                FormatString = "yyyy-MM-ddTHH:mm:ssZ",
                TimeFormat = DateTimeFormat.ShortTime,
                Watermark = "value",
                Text = oldValue
            };

            ValueSelector = () => control.Text ?? String.Empty;

            control.ValueChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);
            control.LostFocus += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

            return control;
        }

        private ComboBox CreateDropDown(string oldValue)
        {
            var control = new ComboBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                IsEditable = true,
                Text = oldValue
            };
            ValueSelector = () => String.IsNullOrEmpty(control.SelectedValue?.ToString()) ? control.Text ?? String.Empty : (control.SelectedValue?.ToString());

            control.SelectionChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);
            control.LostFocus += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

            return control;
        }
    }
}
