using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class CamlParser
    {
        public List<IFilter> Parse(string xml)
        {
            XDocument doc = XDocument.Parse(xml);

            var queryNode = doc.Element("Query");
            var whereNode = queryNode?.Element("Where") ?? doc.Element("Where");

            var filters = Parse(whereNode.Elements());
            return filters;
        }

        private List<IFilter> Parse(IEnumerable<XElement>elements, QueryOperator queryOperator = QueryOperator.And)
        {
            var filters = new List<IFilter>();
            if (elements == null) return filters;

            foreach (var element in elements)
            {
                if (element.Name.LocalName.ToLower() == "and" || element.Name.LocalName.ToLower() == "or")
                {
                    queryOperator = (element.Name.LocalName.ToLower() == "and")
                        ? QueryOperator.And
                        : QueryOperator.Or;

                    filters.AddRange(
                            Parse(element.Elements(), queryOperator)
                        );
                }
                else
                {
                    var filter = ParseFilter(queryOperator, element);
                    filters.Add(filter);
                }
            }

            return filters;
        }

        private IFilter ParseFilter(QueryOperator queryOperator, XElement element)
        {
            if (element.Name.LocalName.ToLower() == "in") return ParseFilterIn(queryOperator, element);
            else return ParseFilterOther(queryOperator, element);
        }

        private IFilter ParseFilterOther(QueryOperator queryOperator, XElement element)
        {
            var filter = new Filter {
                QueryOperator = queryOperator
            };

            FilterOperator queryFilter;
            if (Enum.TryParse(element.Name.LocalName, out queryFilter))
            {
                filter.QueryFilter = queryFilter;
            }


            foreach (var attribute in element.Attributes())
            {
                filter.FilterAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }

            //---
            var fieldRefElement = element.Element("FieldRef");
            var valueElement = element.Element("Value");

            ParseFieldRef(fieldRefElement, filter);
            ParseValue(valueElement, filter);

            return filter;
        }

        private void ParseValue(XElement valueElement, Filter filter)
        {
            if (valueElement == null) return;

            foreach (var attribute in valueElement.Attributes())
            {
                if (attribute.Name.LocalName.ToLower() == "type") filter.FieldType = attribute.Value;
                else filter.ValueAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }

            filter.FieldValue = valueElement.Value;
        }

        private void ParseFieldRef(XElement fieldRefElement, IFilter filter)
        {
            if (fieldRefElement == null) return;

            foreach (var attribute in fieldRefElement.Attributes())
            {
                if (attribute.Name.LocalName.ToLower() == "name") filter.FieldInternalName = attribute.Value;
                else filter.FieldRefAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }
        }

        private IFilter ParseFilterIn(QueryOperator queryOperator, XElement element)
        {
            var filter = new InFilter { QueryOperator = queryOperator };

            foreach (var attribute in element.Attributes())
            {
                filter.FilterAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }

            //---
            var fieldRefElement = element.Element("FieldRef");
            var valuesElement = element.Element("Values");

            ParseFieldRef(fieldRefElement, filter);

            filter.FieldValues = valuesElement.Elements("Value").Select(e => e.Value).ToList();

            return filter;
        }
    }
}
