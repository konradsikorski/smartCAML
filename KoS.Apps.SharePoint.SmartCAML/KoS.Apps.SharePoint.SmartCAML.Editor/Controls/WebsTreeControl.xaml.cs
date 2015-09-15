using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for WebsTreeControl.xaml
    /// </summary>
    public partial class WebsTreeControl : UserControl
    {
        readonly ObservableCollection<Web> Webs = new ObservableCollection<Web>();

        public SharePointList SelectedList
            => ucLists.ItemsSource != null ?
            (ucLists.SelectedItem as SharePointList):
            null;

        public WebsTreeControl()
        {
            InitializeComponent();
            ucLists.ItemsSource = Webs;
        }

        public event EventHandler ListExecute;

        private void ucLists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((ucLists.SelectedItem as SharePointList) == null) return;
            ListExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Add(Web web)
        {
            if (web == null) return;

            Webs.Add(web);
        }
    }
}
