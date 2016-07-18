using System.Collections.Generic;
using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
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