using KoS.Apps.SharePoint.SmartCAML.Editor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for OrderByBuilderControl.xaml
    /// </summary>
    public partial class OrderByBuilderControl : UserControl
    {
        public OrderedList<OrderByFilterControl> Controller { get; }

        public OrderByBuilderControl()
        {
            InitializeComponent();
            Controller = new OrderedList<OrderByFilterControl>(ucFilters);
        }

        private void AddOrderByButton_Click(object sender, RoutedEventArgs e)
        {
            Controller.Add();
        }
    }
}
