using System;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for QueryFilterControl.xaml
    /// </summary>
    public partial class QueryFilterControl : UserControl
    {
        public ComboBox ucFilterOperator;
        public TextBox ucFieldValue;

        public QueryOperator SelectedQueryOperator => ucAndOr.SelectedEnum<QueryOperator>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;
        public FilterOperator SelectedFilterOperator => ucFilterOperator.SelectedEnum<FilterOperator>().Value;
        public string SelectedValue => ucFieldValue.Text;

        public event EventHandler RemoveClick;
        public event EventHandler Changed;


        public QueryFilterControl()
        {
            InitializeComponent();
            ucAndOr.BindToEnum<QueryOperator>();
            ucAndOr.SelectedIndex = 0;
        }

        private void ucField_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var field = ((ComboBox) sender).SelectedItem;

            if (field != null)
            {
                ucFilterOperator = new ComboBox {MinWidth = 100};
                ucFilterOperator.BindToEnum<FilterOperator>();
                ucFilterOperator.SelectionChanged += (o, args) =>  Changed?.Invoke(this, EventArgs.Empty);

                ucFieldValue = new TextBox { MinWidth = 200 };
                ucFieldValue.TextChanged += (o, args) => Changed?.Invoke(this, EventArgs.Empty);

                ucContainer.Children.Add(ucFilterOperator);
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
    }
}
