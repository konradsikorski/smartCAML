using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for ConnectWindowPopup.xaml
    /// </summary>
    public partial class ConnectWindowPopup : UserControl
    {
        public ConnectWindowPopup()
        {
            InitializeComponent();
        }

        public event Action<ISharePointProvider> Connected;

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
                NewConnectWindow();
        }

        private void NewConnectWindow()
        {
            var connectWindow = ucContent.Content as ConnectWindow;

            if (connectWindow != null)
                connectWindow.DialogResult -= ConnectWindow_DialogResult;

            connectWindow = new ConnectWindow();
            connectWindow.DialogResult += ConnectWindow_DialogResult;

            ucContent.Content = connectWindow;
        }

        private void ConnectWindow_DialogResult(ConnectWindow window, ISharePointProvider client)
        {
            Connected(client);
        }
    }
}
