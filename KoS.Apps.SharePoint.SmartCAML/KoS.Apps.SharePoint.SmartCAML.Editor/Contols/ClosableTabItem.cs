using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoS.Apps.SharePoint.SmartCAML.Editor.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Contols
{
    public class ClosableTabItem : TabItem
    {
        public string HeaderText
        {
            get { return HeaderTab.HeaderText; }
            set { HeaderTab.HeaderText = value; }
        }
        private ClosableTabHeader HeaderTab => (ClosableTabHeader) Header;

        public ClosableTabItem()
        {
            Header = new ClosableTabHeader();
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            HeaderTab.CloseButtonVisible = true;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            HeaderTab.CloseButtonVisible = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if(!IsSelected) HeaderTab.CloseButtonVisible = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if(!IsSelected) HeaderTab.CloseButtonVisible = false;
        }
    }
}
