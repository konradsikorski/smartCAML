using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters
{
    public class Filter : IFilter
    {
        public QueryOperator QueryOperator { get; set; }
        public FilterOperator? QueryFilter { get; set; }
        public string FieldInternalName { get; set; }
        public string FieldType { get; set; }
        public string FieldValue { get; set; }
        public bool FieldValueIsUserId { get; set; }
        public Dictionary<string, string> FilterAttributes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> FieldRefAttributes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ValueAttributes { get; set; } = new Dictionary<string, string>();

        public XElement ToXml()
        {
            var operatorNode =
                new XElement(QueryFilter?.ToString() ?? string.Empty,
                    // attributes
                    FilterAttributes.Select( a => new XAttribute(a.Key, a.Value ?? string.Empty)),
                    // chieldNodes
                    new XElement("FieldRef",
                        // attributes
                        new XAttribute("Name", FieldInternalName),
                        FieldRefAttributes.Select(a => new XAttribute(a.Key, a.Value ?? string.Empty))
                        )
                    );

            if (FieldValue != null)
            {
                var valueNode = new XElement("Value",
                    new XAttribute("Type", FieldType),
                    ValueAttributes.Select(a => new XAttribute(a.Key, a.Value ?? string.Empty)),
                    FieldValueIsUserId ? (object)new XElement("UserID") : FieldValue
                    );
                operatorNode.Add(valueNode);
            }

            return operatorNode;
        }
    }
}