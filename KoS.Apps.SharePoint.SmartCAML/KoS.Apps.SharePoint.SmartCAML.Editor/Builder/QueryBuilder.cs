using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class QueryBuilder
    {
        public List<QueryFilter> Filters { get; private set; } = new List<QueryFilter>();

        public QueryFilter Or(FilterOperator @operator, FieldType type, string internalName, string value = null)
        {
            return New(QueryOperator.Or, @operator, type, internalName, value);
        }

        public QueryFilter And(FilterOperator @operator, FieldType type, string internalName, string value = null)
        {
            return New(QueryOperator.And, @operator, type, internalName, value);
        }

        public QueryFilter New(QueryOperator queryOperator, FilterOperator? @operator, FieldType type, string internalName, string value = null, bool asLookupId = false)
        {
            var filter = Build(queryOperator, @operator, type, internalName, value);

            Filters.Add(filter);
            return filter;
        }

        private QueryFilter Build(QueryOperator queryOperator, FilterOperator? @operator, FieldType type,
            string internalName, string value)
        {
            if (@operator == null) return null;

            return new QueryFilter
            {
                QueryOperator = queryOperator,
                FilterOperator = @operator.Value,
                Type = type,
                InternalName = internalName,
                Value = value
            };
        }

        public XElement ToXml()
        {
            XElement rootNode = null;
            XElement lastNode = null;

            for (int i = 0; i < Filters.Count; i++)
            {
                var filter = Filters[i];

                var operatorNode = 
                    new XElement(filter.FilterOperator.ToString(),
                        new XElement("FieldRef",
                            new XAttribute("Name", filter.InternalName)
                        )
                    );

                if (filter.Value != null)
                {
                    var valueNode = new XElement("Value",
                        new XAttribute("Type", filter.Type),
                        filter.Value);
                    operatorNode.Add(valueNode);
                }

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

    public class QueryFilter 
    {
        public QueryOperator QueryOperator { get; set; }
        public FilterOperator FilterOperator { get; set; }
        public FieldType Type { get; set; }
        public string InternalName { get; set; }
        public string Value { get; set; }
    }
}