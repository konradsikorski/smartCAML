using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Client = Microsoft.SharePoint.Client;
using Model = KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider
{
    static class Converter
    {
        public static List<Model.ContentType> ToContentTypes(ContentTypeCollection contentTypes)
        {
            return contentTypes.Cast<Client.ContentType>().Select(ct => new Model.ContentType
            {
                Id = ct.StringId,
                Name = ct.Name
            }).ToList();
        }

        public static string UrlValueToString(FieldUrlValue value)
        {
            return value.Description + "#;" + value.Url;
        }

        public static string LookupCollectionValueToString(FieldLookupValue[] value)
        {
            return value.Select(lv => LookupValueToString(lv))
                            .Aggregate((all, current) => all + "&" + current);
        }

        public static string LookupValueToString(FieldLookupValue value)
        {
            return value.LookupId + "#;" + value.LookupValue;
        }

        public static string ChoiceMultiValueToString(string[] value)
        {
           return value.Aggregate((all, current) => all + "&" + current);
        }
    }
}
