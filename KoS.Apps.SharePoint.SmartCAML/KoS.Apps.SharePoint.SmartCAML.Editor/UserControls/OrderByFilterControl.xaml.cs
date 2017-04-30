using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for OrderByFilterControl.xaml
    /// </summary>
    public partial class OrderByFilterControl : UserControl, IOrderListElement
    {
        public OrderByDirection SelectedDirection => ucOrderDirection.SelectedEnum<OrderByDirection>().Value;
        public Field SelectedField => (Field)ucField.SelectedItem;

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

        private void ucField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
