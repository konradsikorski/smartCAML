using System;
using System.ComponentModel;
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
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "SmartCAML [v. " + VersionUtil.GetVersion() + "]";
            this.Width = Config.WindowWidth;
            this.Height = Config.WindowHeight;
            if(Config.WasMaximazed) this.WindowState = WindowState.Maximized;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new ConnectWindow();

            if (dialog.ShowDialog() == true)
            {
                Connected(dialog.Client);
            }
        }
        private void NewQuery_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucWebs?.SelectedList != null;
        }

        private void NewQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ucQueries.AddQuery(ucWebs.SelectedList);
        }

        private void UcWebs_ListExecute(object sender, EventArgs e)
        {
            NewQueryCommand.Command.Execute(null);
        }

        private void Connected(ISharePointProvider client)
        {
            ucWebs.Add(client);
        }

        private void RunQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ucQueries?.SelectedQueryTab != null;
        }

        private void RunQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var query = ucQueries.SelectedQueryTab.GetQuery();
            var items = ucWebs.GetClient(ucQueries.SelectedQueryTab.List.Web).ExecuteQuery(query);
            ucQueries.SelectedQueryTab.QueryResult(items);
        }

        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void UcWebs_OnCloseWeb(object sender, Web web)
        {
            ucQueries.CloseWeb(web);
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
    }
}
