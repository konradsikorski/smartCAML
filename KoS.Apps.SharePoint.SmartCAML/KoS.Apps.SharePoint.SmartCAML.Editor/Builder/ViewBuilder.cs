using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class ViewBuilder
    {
        public List<IFilter> Filters { get; private set; } = new List<IFilter>();
        public List<QueryOrderBy> OrderBy { get; private set; } = new List<QueryOrderBy>();

        public XElement ToXml()
        {
            return new XElement("Query",
                BuildWhereNode(),
                BuildOrderByNode()
                );
        }

        private XElement BuildWhereNode()
        {
            XElement rootFilterNode = null;
            XElement lastFilterNode = null;

            for (int i = 0; i < Filters.Count; i++)
            {
                var filter = Filters[i];

                var operatorNode = filter.ToXml();

                var parent = (i != Filters.Count - 1)
                    ? new XElement(filter.QueryOperator.ToString(), operatorNode)
                    : operatorNode;

                if (lastFilterNode == null)
                {
                    rootFilterNode = lastFilterNode = parent;
                }
                else
                {
                    lastFilterNode.Add(parent);
                    lastFilterNode = parent;
                }
            }

            return new XElement("Where", rootFilterNode);
        }

        private XElement BuildOrderByNode()
        {
            return 
                OrderBy.Count == 0
                ? null
                : new XElement(
                    "OrderBy",
                    OrderBy.Select(ob => ob.ToXml())
                );
        }

        public static ViewBuilder FromXml(string xml)
        {
            var viewBuilder = new ViewBuilder();
            if (string.IsNullOrWhiteSpace(xml)) return viewBuilder;

            XDocument doc;
            try
            {
                doc = XDocument.Parse(xml);
            }
            catch(System.Xml.XmlException)
            {
                return null;
            }

            var queryNode = doc.Element("Query");

            // read where
            // ... todo

            // read orderBy
            var orderNode = queryNode?.Element("OrderBy") ?? doc.Element("OrderBy");

            if (orderNode != null)
            {
                foreach (var node in orderNode.Descendants("FieldRef"))
                {
                    bool isAscending;
                    var ascending = node.Attribute("Ascending")?.Value;
                    if (!bool.TryParse(ascending, out isAscending)) isAscending = true;

                    viewBuilder.OrderBy.Add(new QueryOrderBy
                    {
                        FieldName = node.Attribute("Name")?.Value,
                        Direction = isAscending ? Enums.OrderByDirection.Ascending : Enums.OrderByDirection.Descending
                    });
                }
            }

            return viewBuilder;
        }
    }
}