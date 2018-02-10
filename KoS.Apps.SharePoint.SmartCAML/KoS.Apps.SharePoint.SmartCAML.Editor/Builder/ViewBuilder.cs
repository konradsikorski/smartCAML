using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using System;
using NLog;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class ViewBuilder
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

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

            var doc = TryParseDocument(xml);
            if (doc == null) return null;

            var queryNode = doc.Element("Query");

            // read where
            var whereNode = queryNode?.Element("Where") ?? doc.Element("Where");
            var filters = CamlParser.TryParse(whereNode);

            viewBuilder.Filters = filters;

            // read orderBy
            var orderByNode = queryNode?.Element("OrderBy") ?? doc.Element("OrderBy");
            var orderBy = ParseOrderByNode(orderByNode);

            viewBuilder.OrderBy = orderBy;

            return viewBuilder;
        }

        private static XDocument TryParseDocument(string xml)
        {
            try
            {
                return XDocument.Parse(xml);
            }
            catch (System.Xml.XmlException ex)
            {
                Log.Warn(ex);
                return null;
            }
        }

        private static List<QueryOrderBy> TryParseOrderByNode(XElement orderByNode)
        {
            try
            {
                return ParseOrderByNode(orderByNode);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        private static List<QueryOrderBy> ParseOrderByNode(XElement orderByNode)
        {
            var orderBy = new List<QueryOrderBy>();
            if (orderByNode == null) return orderBy;

            foreach (var node in orderByNode.Descendants("FieldRef"))
            {
                bool isAscending;
                var ascending = node.Attribute("Ascending")?.Value;
                if (!bool.TryParse(ascending, out isAscending)) isAscending = true;

                orderBy.Add(new QueryOrderBy
                {
                    FieldName = node.Attribute("Name")?.Value,
                    Direction = isAscending ? Enums.OrderByDirection.Ascending : Enums.OrderByDirection.Descending
                });
            }

            return orderBy;
        }
    }
}