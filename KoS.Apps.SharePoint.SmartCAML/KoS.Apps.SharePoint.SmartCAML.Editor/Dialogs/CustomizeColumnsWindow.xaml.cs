using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomizeColumnsWindow.xaml
    /// </summary>
    public partial class CustomizeColumnsWindow : Window
    {
        public ObservableCollection<ColumnVisibility> Columns { get; set; } = new ObservableCollection<ColumnVisibility>();

        public CustomizeColumnsWindow()
        {
            InitializeComponent();
        }

        public CustomizeColumnsWindow(List<ColumnVisibility> columns)
        {
            InitializeComponent();

            ucHiddenColumns.DisplayMemberPath = ucVisibleColumns.DisplayMemberPath = "Title";

            Columns = new ObservableCollection<ColumnVisibility>(columns);
            //ucHiddenColumns.ItemsSource = columns.Where(c => !c.IsVisible);
            //ucVisibleColumns.ItemsSource = columns.Where(c => c.IsVisible);

            var hiddenViewSource = new CollectionViewSource { Source = columns, IsLiveFilteringRequested = true};
            hiddenViewSource.Filter += (sender, args) => args.Accepted = !((ColumnVisibility)args.Item).IsVisible;

            var visibleViewSource = new CollectionViewSource { Source = columns, IsLiveFilteringRequested = true };
            visibleViewSource.Filter += (sender, args) => args.Accepted = ((ColumnVisibility)args.Item).IsVisible;

            ucHiddenColumns.ItemsSource = hiddenViewSource.View;
            ucVisibleColumns.ItemsSource = visibleViewSource.View;
            
        }

        private void HideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucVisibleColumns.SelectedItems.Count > 0;
        }

        private void HideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (ColumnVisibility item in ucVisibleColumns.SelectedItems)
            {
                item.IsVisible = false;
            }

            ((ICollectionView)ucHiddenColumns.ItemsSource).Refresh();
            ((ICollectionView)ucVisibleColumns.ItemsSource).Refresh();
        }

        private void HideAllColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Columns.Any(c => c.IsVisible);
        }

        private void HideAllColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (ColumnVisibility item in ucVisibleColumns.Items)
            {
                item.IsVisible = false;
            }
        }

        private void UnhideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucHiddenColumns.SelectedItems.Count > 0;
        }

        private void UnhideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (ColumnVisibility item in ucVisibleColumns.SelectedItems)
            {
                item.IsVisible = true;
            }
        }

        private void UnhideAllColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Columns.Any(c => !c.IsVisible);
        }

        private void UnhideAllColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (ColumnVisibility item in ucVisibleColumns.Items)
            {
                item.IsVisible = true;
            }
        }

        private void VisibleVIewSource_OnFilter(object sender, FilterEventArgs e)
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
