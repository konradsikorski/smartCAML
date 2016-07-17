using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public interface IFilter
    {
        QueryOperator QueryOperator { get; set; }
        FilterOperator? QueryFilter { get; set; }
        XElement ToXml();
    }

    public class Filter : IFilter
    {
        public QueryOperator QueryOperator { get; set; }
        public FilterOperator? QueryFilter { get; set; }
        public string FieldInternalName { get; set; }
        public string FieldType { get; set; }
        public string FieldValue { get; set; }
        public Dictionary<string, string> FilterAttributes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> FieldRefAttributes { get; set; } = new Dictionary<string, string>();

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
                    FieldValue
                    );
                operatorNode.Add(valueNode);
            }

            return operatorNode;
        }
    }

    public class ViewBuilder
    {
        public List<IFilter> Filters { get; private set; } = new List<IFilter>();

        public XElement ToXml()
        {
            XElement rootNode = null;
            XElement lastNode = null;

            for (int i = 0; i < Filters.Count; i++)
            {
                var filter = Filters[i];

                var operatorNode = filter.ToXml();

                var parent = (i != Filters.Count - 1)
                    ? new XElement(filter.QueryOperator.ToString(), operatorNode)
                    : operatorNode;

                if (lastNode == null)
                {
                    rootNode = lastNode = parent;
                }
                else
                {
                    lastNode.Add(parent);
                    lastNode = parent;
                }
            }

            return new XElement("Where", rootNode);
        }
    }
}