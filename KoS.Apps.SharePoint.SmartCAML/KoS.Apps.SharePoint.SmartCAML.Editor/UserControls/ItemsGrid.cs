using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ItemsGrid.xaml
    /// </summary>
    public partial class ItemsGrid : UserControl
    {
        private SmartCAML.Model.SList _list;

        #region Dependency Property

        public static readonly DependencyProperty DisplayColumnsByTitleProperty = DependencyProperty.Register(nameof(DisplayColumnsByTitle), typeof(bool), typeof(ItemsGrid), new PropertyMetadata(DisplayColumnsByTitlePropertyChanged));
        [Bindable(true)]
        public bool DisplayColumnsByTitle
        {
            get
            {
                return (bool)this.GetValue(DisplayColumnsByTitleProperty);
            }
            set
            {
                this.SetValue(DisplayColumnsByTitleProperty, value);
            }
        }

        private static void DisplayColumnsByTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            foreach (var column in ((ItemsGrid) d).ucItems.Columns)
            {
                var header = column.Header;
                column.Header = null;
                column.Header = header;
            }
        }

        #endregion

        public ItemsGrid()
        {
            InitializeComponent();
        }

        public SList List
        {
            get { return _list; }
            set
            {
                _list = value;

                foreach (var column in List.Fields.Select( c => new
                    {
                        Header = new ColumnHeader { Title = c.Title, InternalName = c.InternalName, IsHidden = c.IsHidden, IsReadOnly = c.IsReadonly},
                        Bind = c.InternalName,
                        Field = c
                    }).OrderBy( c => DisplayColumnsByTitle ? c.Header.Title : c.Header.InternalName))
                {
                    var gridColumn = new DataGridTextColumn
                    {
                        IsReadOnly = column.Field.IsReadonly,
                        Header = column.Header,
                        Width = 100,
                        Binding = new Binding($"[{column.Bind}]") {Mode = BindingMode.TwoWay}
                    };

                    BindColumnHeaderFormat(gridColumn);
                    ucItems.Columns.Add(gridColumn);
                }
            }
        }

        public bool HasChanges => ucItems.ItemsSource?.Cast<ListItem>().Any(item => item.IsDirty) ?? false;

        internal void QueryResult(List<ListItem> items)
        {
            ucItems.ItemsSource = items;
        }

        private void BindColumnHeaderFormat(DataGridTextColumn column)
        {
            BindingOperations.SetBinding(column, DataGridTextColumn.HeaderStringFormatProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DisplayColumnsByTitle)),
                Mode = BindingMode.TwoWay,
                Converter = new BoolToStringConverter { True= "Title", False = "InternalName"}
            });
        }

        private void ucItems_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //if (e.EditAction == DataGridEditAction.Cancel) return;

            //var item = (ListItem)e.Row.DataContext;
            //try
            //{
            //    item.Update();
            //}
            //catch (Exception)
            //{
            //    item.CancelChanges();
            //    e.Cancel = true;
            //    ucItems.CancelEdit();
            //}
        }

        public List<ListItem> GetDirtyItems()
            => ucItems.ItemsSource.Cast<ListItem>().Where(item => item.IsDirty).ToList();

        public void SetFields()
        {
            

        }

        public class ColumnHeader : IFormattable
        {
            public string Title { get; set; }
            public string InternalName { get; set; }

            public bool IsHidden { get; set; }

            public bool IsReadOnly { get; set; }
            
            public override string ToString()
            {
                return Title;
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                return format?.ToLower() == nameof(InternalName).ToLower()
                    ? InternalName
                    : Title;
            }
        }

        private void HideAllHiddenColumnsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HideAllHiddenColumnsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.ItemsGrid.HideAllHiddenColumns");

            foreach (var column in ucItems.Columns)
            {
                if (((ColumnHeader)column.Header).IsHidden) column.Visibility = Visibility.Collapsed;
            }
        }

        private void HideAllReadonlyColumnsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HideAllReadonlyColumnsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.ItemsGrid.HideAllHiddenColumns");

            foreach (var column in ucItems.Columns)
            {
                if (((ColumnHeader)column.Header).IsReadOnly) column.Visibility = Visibility.Collapsed;
            }
        }

        private void HideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.ItemsGrid.HideColumn");
            var column = (DataGridColumnHeader)e.OriginalSource;
            column.Column.Visibility = Visibility.Collapsed;
        }

        private DataGridColumnHeader FromMenuItemToColumnHeader(MenuItem menuItem)
        {
            var contextMenu = (ContextMenu)menuItem.Parent;
            return (DataGridColumnHeader)contextMenu.PlacementTarget;
        }

        private void UnHideColumnCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = 
                ucItems.Columns.Any(c =>
                    c.Visibility == Visibility.Collapsed
                    );
        }

        private void UnHideColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.ItemsGrid.UnHideColumn");
            foreach (var column in ucItems.Columns)
            {
                column.Visibility = Visibility.Visible;
            }
        }

        private void CustomizeColumnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.ItemsGrid.CustomizeColumn");
            var dialog = new CustomizeColumnsWindow(
                ucItems.Columns.Select(c => new ColumnVisibility
                {
                  IsVisible  = c.Visibility == Visibility.Visible,
                  InternalName = ((ColumnHeader)c.Header).InternalName,
                  Title = ((ColumnHeader)c.Header).Title
                }).ToList()
                );

            if (dialog.ShowDialog() == true)
            {
                foreach (var gridColumn in ucItems.Columns)
                {
                    gridColumn.Visibility =
                        dialog.Columns.First(c => c.InternalName == ((ColumnHeader) gridColumn.Header).InternalName).IsVisible
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
            }
        }
    }
}
