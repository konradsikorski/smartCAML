using KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces;
using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    public partial class PopupWindow : UserControl
    {
        public PopupWindow()
        {
            InitializeComponent();
        }

        public void Show(UIElement content)
        {
            if (this.Content != null)
            {
                ((UIElement)this.Content).IsVisibleChanged -= Content_IsVisibleChanged;
            }

            content.IsVisibleChanged += Content_IsVisibleChanged;
            ucContent.Content = content;
            this.Visibility = Visibility.Visible;
        }

        private void Content_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) return;

            var senderContent = sender as UIElement;
            senderContent.IsVisibleChanged -= Content_IsVisibleChanged;

            if (ucContent.Content == senderContent)
            {
                ucContent.Content = null;
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
