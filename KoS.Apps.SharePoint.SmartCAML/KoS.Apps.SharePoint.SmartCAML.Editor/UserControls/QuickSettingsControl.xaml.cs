using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for QuickSettingsControl.xaml
    /// </summary>
    public partial class QuickSettingsControl : UserControl
    {
        public QuickSettingsControl()
        {
            InitializeComponent();

            ucDisplayBy.ItemsSource = new[] {"Internal Name", "Title"};
            ucDisplayBy.SelectedIndex = Config.DisplayColumnsByTitle ? 1 : 0;
        }

        private void UcDisplayBy_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.DisplayColumnsByTitle = ucDisplayBy.SelectedIndex == 1;
            ((MainWindow) Application.Current.MainWindow).UpdateConfig();
        }
    }
}
