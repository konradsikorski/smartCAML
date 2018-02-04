using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.UserControls
{
    /// <summary>
    /// Interaction logic for QueryXmlControl.xaml
    /// </summary>
    public partial class QueryXmlControl : UserControl
    {
        public bool Modified { get; set; }

        public QueryXmlControl()
        {
            InitializeComponent();
        }

        private void ucQuery_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Modified) TryPretyXmlPrint();
        }

        private void ucQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            Modified = true;
        }

        public ViewBuilder BuildQuery()
        {
            return ViewBuilder.FromXml(ucQuery.Text);
        }

        public string GetQueryXml()
        {
            return ucQuery.Text;
        }

        public void Refresh(string xml)
        {
            ucQuery.Text = xml;
            Modified = false;
        }

        private void TryPretyXmlPrint()
        {
            var xml = ucQuery.Text;
            if (string.IsNullOrWhiteSpace(xml)) return;

            try
            {
                // check if it is valid xml
                ucQuery.Text = XDocument.Parse(xml).ToString();
            }
            catch
            {
                // ignored
            }
        }
    }
}
