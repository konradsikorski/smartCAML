using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

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
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns");
            InitializeComponent();
        }

        public CustomizeColumnsWindow(List<ColumnVisibility> columns)
        {
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns");
            InitializeComponent();

            ucHiddenColumns.DisplayMemberPath = ucVisibleColumns.DisplayMemberPath = "InternalName";

            Columns = new ObservableCollection<ColumnVisibility>(columns);

            this.DataContext = new
            {
                Columns
            };
        }

        private void VisibleViewSource_OnFilter(object sender, FilterEventArgs args)
        {
            args.Accepted = ((ColumnVisibility) args.Item).IsVisible;
        }

        private void HiddenViewSource_OnFilter(object sender, FilterEventArgs args)
        {
            args.Accepted = !((ColumnVisibility)args.Item).IsVisible;
        }

        private void HideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucVisibleColumns.SelectedItems.Count > 0;
        }

        private void HideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns.HideColumn");
            foreach (ColumnVisibility item in ucVisibleColumns.SelectedItems)
            {
                item.IsVisible = false;
            }
        }

        private void HideAllColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Columns.Any(c => c.IsVisible);
        }

        private void HideAllColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns.HideAllColumn");
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
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns.UnhideColumn");
            foreach (ColumnVisibility item in ucHiddenColumns.SelectedItems)
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
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns.UnhideAllColumn");
            foreach (ColumnVisibility item in ucHiddenColumns.Items)
            {
                item.IsVisible = true;
            }
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("CustomizeColumns.OK");
            DialogResult = true;
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
