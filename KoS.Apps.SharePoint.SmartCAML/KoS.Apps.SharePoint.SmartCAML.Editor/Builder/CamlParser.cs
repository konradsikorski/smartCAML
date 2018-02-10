using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder
{
    public class CamlParser
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static List<IFilter> Parse(string xml)
        {
            if (String.IsNullOrEmpty(xml)) return new List<IFilter>();

            var doc = XDocument.Parse(xml);

            var queryNode = doc.Element("Query");
            var whereNode = queryNode?.Element("Where") ?? doc.Element("Where");

            return Parse(whereNode);
        }

        public static List<IFilter> TryParse(XElement whereNode)
        {
            try
            {
                return Parse(whereNode);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static List<IFilter> Parse(XElement whereNode)
        {
            if (whereNode == null) return new List<IFilter>();

            var filters = Parse(whereNode.Elements());
            return filters;
        }

        private static List<IFilter> Parse(IEnumerable<XElement>elements, QueryOperator queryOperator = QueryOperator.And)
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

            return filters.Where( f => f != null).ToList();
        }

        private static IFilter ParseFilter(QueryOperator queryOperator, XElement element)
        {
            if (element.Name.LocalName.ToLower() == "in") return ParseFilterIn(queryOperator, element);
            else return ParseFilterOther(queryOperator, element);
        }

        private static IFilter ParseFilterOther(QueryOperator queryOperator, XElement element)
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

        private static void ParseValue(XElement valueElement, Filter filter)
        {
            if (valueElement == null) return;

            foreach (var attribute in valueElement.Attributes())
            {
                if (attribute.Name.LocalName.ToLower() == "type") filter.FieldType = attribute.Value;
                else filter.ValueAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }

            filter.FieldValue = valueElement.Value;
        }

        private static void ParseFieldRef(XElement fieldRefElement, IFilter filter)
        {
            if (fieldRefElement == null) return;

            foreach (var attribute in fieldRefElement.Attributes())
            {
                if (attribute.Name.LocalName.ToLower() == "name") filter.FieldInternalName = attribute.Value;
                else filter.FieldRefAttributes.Add(attribute.Name.LocalName, attribute.Value);
            }
        }

        private static IFilter ParseFilterIn(QueryOperator queryOperator, XElement element)
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

            filter.FieldValues = valuesElement?.Elements("Value").Select(e => e.Value).ToList() ?? new List<string>();

            return filter;
        }
    }
}
