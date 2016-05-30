using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.Editor.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomizeColumnsWindow.xaml
    /// </summary>
    public partial class CustomizeColumnsWindow : Window
    {
        public List<ColumnVisibility> Columns { get; set; } = new List<ColumnVisibility>();

        public CustomizeColumnsWindow()
        {
            InitializeComponent();
        }

        public CustomizeColumnsWindow(List<ColumnVisibility> columns)
        {
            InitializeComponent();

            ucHiddenColumns.DisplayMemberPath = ucVisibleColumns.DisplayMemberPath = "Title";

            Columns = columns;
            ucHiddenColumns.ItemsSource = columns.Where(c => !c.IsVisible);
            ucVisibleColumns.ItemsSource = columns.Where(c => c.IsVisible);
        }

        private void HideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucVisibleColumns.SelectedItems.Count > 0;
        }

        private void HideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void HideAllColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Columns.Any(c => c.IsVisible);
        }

        private void HideAllColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void UnhideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucHiddenColumns.SelectedItems.Count > 0;
        }

        private void UnhideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void UnhideAllColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Columns.Any(c => !c.IsVisible);
        }

        private void UnhideAllColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }

    public class ColumnVisibility :INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string InternalName { get; set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        private int _order;
        public int Order
        {
            get { return _order; }
            set
            {
                if (value == _order) return;
                _order = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
