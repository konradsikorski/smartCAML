using System;
using System.ComponentModel;
using System.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs;
using System.Windows;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand MyCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "SmartCAML [v. " + VersionUtil.GetVersion() + "]";
            this.Width = Config.WindowWidth;
            this.Height = Config.WindowHeight;
            if(Config.WasMaximazed) this.WindowState = WindowState.Maximized;

            ((RoutedCommand)ConnectCommand.Command).InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
        }

        #region Event Handlers

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ConnectCommand.Command.Execute(null);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Config.WasMaximazed = this.WindowState == WindowState.Maximized;
            if (!Config.WasMaximazed)
            {
                Config.WindowHeight = this.Height;
                Config.WindowWidth = this.Width;
            }
        }

        private void UcWebs_ListExecute(object sender, EventArgs e)
        {
            NewQueryCommand.Command.Execute(null);
        }

        private void UcWebs_OnCloseWeb(object sender, Web web)
        {
            ucQueries.CloseWeb(web);
        }

        #endregion

        #region Commands

        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new ConnectWindow();

            if (dialog.ShowDialog() == true)
            {
                Connected(dialog.Client);
            }
        }

        private void NewQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucWebs?.SelectedList != null;
        }

        private async void NewQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await ucQueries.AddQuery(ucWebs.SelectedList);
        }

        private void RunQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucQueries?.SelectedQueryTab != null;
        }

        private async void RunQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var query = ucQueries.SelectedQueryTab.GetQuery();
            try
            {
                var list = ucQueries.SelectedQueryTab.List;
                StatusNotification.NotifyWithProgress("Quering list: " + list.Title);

                var items = await ucWebs.GetClient(list.Web).ExecuteQuery(query);
                ucQueries.SelectedQueryTab.QueryResult(items);

                StatusNotification.Notify("Retrived items: " + items.Count);
            }
            catch (Exception ex)
            {
                StatusNotification.Notify("Quering failed");
                ExceptionHandler.Handle(ex, "The request failed.");
            }
        }

        private void SaveChangesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucQueries?.SelectedQueryTab?.ucItems.HasChanges ?? false;
        }

        private async void SaveChangesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dirtyItems = ucQueries.SelectedQueryTab.ucItems.GetDirtyItems();
            var index = 0;

            try
            {
                foreach (var listItem in dirtyItems)
                {
                    StatusNotification.Notify("Updating item with id: " + listItem.Id, ++index, dirtyItems.Count);

                    try
                    {
                        await listItem.Update();
                    }
                    catch
                    {
                        listItem.CancelChanges();

                        if (dirtyItems.Last() == listItem)
                            MessageBox.Show("Couldn't update item with id: " + listItem.Id, "SmartCAML",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        else if (
                            MessageBox.Show(
                                "Couldn't update item with id: " + listItem.Id + "\n\n Would you like to continue?",
                                "SmartCAML",
                                MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK) break;
                    }
                }

                StatusNotification.Notify("Update completed");
            }
            catch (Exception ex)
            {
                StatusNotification.Notify("Update failed");
                ExceptionHandler.Handle(ex, "The request failed.");
            }
        }

        private void CloseQueryTabCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucQueries?.SelectedQueryTab != null;
        }

        private void CloseQueryTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ucQueries?.CloseQuery(ucQueries?.SelectedQueryTab);
        }

        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        #endregion

        private void Connected(ISharePointProvider client)
        {
            ucWebs.Add(client);
        }
    }
}
