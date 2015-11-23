using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        public ComboBox ucFilterOperator;
        public Control ucFieldValue;
        public ComboBox ucLookupAs;
        public CheckBox ucIncludeTime;

        public QueryOperator SelectedQueryOperator => ucAndOr.SelectedEnum<QueryOperator>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;
        public FilterOperator? SelectedFilterOperator => ucFilterOperator.SelectedEnum<FilterOperator>().GetValueOrDefault();
        public string SelectedValue => ValueSelector();

        private Func<string> ValueSelector = () => null; 

        public event EventHandler RemoveClick;
        public event EventHandler Changed;
        public event EventHandler Up;
        public event EventHandler Down;

        private int _controlWidth = 100;
        private Thickness _controlMargin = new Thickness(4, 0, 0, 0);


        public QueryFilterControl()
        {
            InitializeComponent();
            ucAndOr.BindToEnum<QueryOperator>();
            ucAndOr.SelectedIndex = 0;
        }

        private void ucField_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var field = SelectedField;

            if (field != null)
            {
                if (ucFilterOperator == null)
                {
                    ucFilterOperator = new ComboBox {MinWidth = _controlWidth, Margin = _controlMargin };
                    ucFilterOperator.BindToEnum<FilterOperator>();
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
                        }
                        else if(ucFieldValue == null) RefreshValueField(SelectedField);

                        Changed?.Invoke(this, EventArgs.Empty);
                    };

                    ucContainer.Children.Add(ucFilterOperator);
                }

                if (ucLookupAs != null) ucContainer.Children.Remove(ucLookupAs);
                if (field.Type == FieldType.Lookup)
                {
                    ucLookupAs = new ComboBox
                    {
                        MinWidth = _controlWidth,
                        Margin = _controlMargin,
                        IsEditable = false,
                        ItemsSource = new[] {"as lookup id", "as lookpu text"},
                        SelectedIndex = 0
                    };

                    ucContainer.Children.Add(ucLookupAs);
                }

                if (ucIncludeTime != null) ucContainer.Children.Remove(ucIncludeTime);
                RefreshValueField(field);
            }
        }

        private void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
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
                SelectedValue);
        }

        private Control BuildFieldValueControl(Field field)
        {
            var oldValue = ValueSelector();

            if (field.Type == FieldType.DateTime)
            {
                // todo: http://www.codeproject.com/Articles/414414/SharePoint-Working-with-Dates-in-CAML
                if (ucIncludeTime != null) ucContainer.Children.Remove(ucIncludeTime);
                ucIncludeTime = new CheckBox
                    {
                        MinWidth = _controlWidth,
                        Margin = _controlMargin,
                        Padding = new Thickness(0,2,0,0),
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
            else
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

                if (field.Type == FieldType.Choice) control.ItemsSource = ((FieldChoice) field).Choices.OrderBy(c => c);
                else if (field.Type == FieldType.Boolean)
                {
                    control.DisplayMemberPath = "Text";
                    control.SelectedValuePath = "Value";
                    control.ItemsSource = new[]
                    {
                        new {Value = "0", Text = "False"},
                        new {Value = "1", Text = "True"}
                    };
                }
                else if (field.Type == FieldType.User) control.ItemsSource = new[] { "@Me" };
                else if (field.Type == FieldType.ContentTypeId)
                {
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

                        var contentTypes = field.List.ContentTypes.Concat(field.List.Web.ContentTypes);

                        control.ItemsSource =
                            contentTypes.Select(ct => new
                            {
                                ct.Id,
                                Name = field.List.ContentTypes.Contains(ct) ? "List." + ct.Name : ct.Name
                            });
                    };
                }

                return control;
            }
        }

        private void UpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Up?.Invoke(this, EventArgs.Empty);
        }

        private void DownButton_OnClick(object sender, RoutedEventArgs e)
        {
            Down?.Invoke(this, EventArgs.Empty);
        }
    }
}
