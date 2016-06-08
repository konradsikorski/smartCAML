using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Model;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    /// <summary>
    /// Interaction logic for WebsTreeControl.xaml
    /// </summary>
    public partial class WebsTreeControl : UserControl
    {
        public ObservableCollection<ListTreeItem> Webs = new ObservableCollection<ListTreeItem>();

        public event EventHandler<Web> CloseWeb;

        public SList SelectedList
            => ucLists.ItemsSource != null ?
            (ucLists.SelectedItem as SList) :
            null;

        public WebsTreeControl()
        {
            InitializeComponent();
            ucLists.ItemsSource = Webs;
        }

        public event EventHandler ListExecute;

        private void ucLists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.WebsTree.DoubleClick");
            if ((ucLists.SelectedItem as SList) == null) return;
            ListExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Add(ISharePointProvider client)
        {
            if (client == null) return;

            Webs.Add(new ListTreeItem { Client = client} );
        }

        public ISharePointProvider GetClient(Web web)
        {
            return Webs.FirstOrDefault(client => client.Client.Web == web)?.Client;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Telemetry.Instance.Native.TrackPageView("Main.WebsTree.Close");
            var web = (Web)((Button)sender).Tag;

            this.Webs.Remove(this.Webs.First(w => w.Client.Web == web));
            CloseWeb?.Invoke(this, web);
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SetCloseButtonVisibility((Grid)sender, true);
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            SetCloseButtonVisibility((Grid) sender, false);
        }

        private void SetCloseButtonVisibility(Grid grid, bool visible)
        {
            grid.Children.OfType<Button>().First().Visibility = visible 
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
