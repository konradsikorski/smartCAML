using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;

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
    }
}