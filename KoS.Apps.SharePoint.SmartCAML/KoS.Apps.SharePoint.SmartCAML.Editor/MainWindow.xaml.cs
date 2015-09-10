using KoS.Apps.SharePoint.SmartCAML.Editor.Dialogs;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
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

        private void Connected(ISharePointProvider client)
        {
            Client = client;
            var lists = Client.GetLists();
        }
    }
}
