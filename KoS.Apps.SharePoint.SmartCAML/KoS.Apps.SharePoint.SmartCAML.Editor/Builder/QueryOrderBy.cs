using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using System.Xml.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class QueryOrderBy
    {
        public string FieldName { get; set; }
        public OrderByDirection Direction { get; set; }

        public XElement ToXml()
        {
            return
                new XElement("FieldRef",
                    new XAttribute("Name", FieldName),
                    new XAttribute("Ascending", (Direction == OrderByDirection.Ascending).ToString())
                    );
        }
    }
}
