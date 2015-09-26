using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
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

        public QueryOperator SelectedQueryOperator => ucAndOr.SelectedEnum<QueryOperator>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;
        public FilterOperator? SelectedFilterOperator => ucFilterOperator.SelectedEnum<FilterOperator>().GetValueOrDefault();
        public string SelectedValue => ValueSelector();

        private Func<string> ValueSelector; 

        public event EventHandler RemoveClick;
        public event EventHandler Changed;
        public event EventHandler Up;
        public event EventHandler Down;


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
                    ucFilterOperator = new ComboBox {MinWidth = 100, Margin = new Thickness(4, 0, 0, 0)};
                    ucFilterOperator.BindToEnum<FilterOperator>();
                    ucFilterOperator.SelectionChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

                    ucContainer.Children.Add(ucFilterOperator);
                }

                if(ucFieldValue != null ) ucContainer.Children.Remove(ucFieldValue);
                ucFieldValue = BuildFieldValueControl(field);

                ucContainer.Children.Add(ucFieldValue);
            }
        }

        private void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
        }

        public void BuildQuery(QueryBuilder builder)
        {
            if (SelectedField == null) return;

            builder.New(SelectedQueryOperator, SelectedFilterOperator, SelectedField.Type , SelectedField.InternalName,
                SelectedValue);
        }

        private Control BuildFieldValueControl(Field field)
        {
            if (field.Type == FieldType.DateTime)
            {
                var control = new DateTimePicker {MinWidth = 100, Margin = new Thickness(4, 0, 4, 0)};
                control.TimeFormat = DateTimeFormat.ShortDate;
                ValueSelector = () => control.Text;

                if (((FieldDateTime) field).DateOnly)
                {
                    control.TimePickerAllowSpin = false;
                    control.TimePickerShowButtonSpinner = false;
                    control.TimePickerVisibility = Visibility.Collapsed;
                }

                control.ValueChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);
                control.LostFocus += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

                return control;
            }
            else
            {
                var control = new ComboBox {MinWidth = 100, Margin = new Thickness(4, 0, 4, 0)};
                control.IsEditable = true;
                ValueSelector = () => control.Text;

                control.SelectionChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);
                control.LostFocus += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

                if (field.Type == FieldType.Choice) control.ItemsSource = ((FieldChoice) field).Choices.OrderBy(c => c);
                else if (field.Type == FieldType.Boolean) control.ItemsSource = new[] {"0", "1"};

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
