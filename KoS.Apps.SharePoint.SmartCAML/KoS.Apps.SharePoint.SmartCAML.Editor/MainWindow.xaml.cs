using System;
using KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Controls;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ISharePointProvider Client { get; set; }

        public MainWindow()
        {
            InitializeComponent();
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
            Client = client;
            ucWebs.Add(Client.Web);
        }

        private void RunQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var query = ucQueries.SelectedQueryTab.GetQuery();
            var items = Client.ExecuteQuery(query);
            ucQueries.SelectedQueryTab.QueryResult(items);
        }

        private void RunQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
