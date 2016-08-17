using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider
{
    static class Converter
    {
        private const string LookupValueSeparator = "#;";
        private const string LookupCollectionItemSeparator = "|";

        public static List<Model.ContentType> ToContentTypes(ContentTypeCollection contentTypes)
        {
            return contentTypes.Cast<ContentType>().Select(ct => new Model.ContentType
            {
                Id = ct.StringId,
                Name = ct.Name
            }).ToList();
        }

        #region From Value to String

        public static string UrlValueToString(FieldUrlValue value)
        {
            return value.Description + LookupValueSeparator + value.Url;
        }

        public static string LookupCollectionValueToString(FieldLookupValue[] value)
        {
            if (value.Length == 0) return string.Empty;

            return value.Select(lv => LookupValueToString(lv))
                            .Aggregate((all, current) => all + LookupCollectionItemSeparator + current);
        }

        public static string LookupValueToString(FieldLookupValue value)
        {
            return value.LookupId + LookupValueSeparator + value.LookupValue;
        }

        public static string ChoiceMultiValueToString(string[] value)
        {
            if(value.Length == 0) return string.Empty;

           return value.Aggregate((all, current) => all + LookupCollectionItemSeparator + current);
        }

        public static string TaxonomyValueToString(TaxonomyFieldValue value)
        {
            if (value == null) return string.Empty;
            return value.Label;
        }

        public static string TaxonomyCollectionValueToString(TaxonomyFieldValueCollection value)
        {
            if (value == null) return string.Empty;
            if (value.ToList().Count == 0) return string.Empty;

            return value
                .ToList()
                .Select(lv => TaxonomyValueToString(lv))
                .Aggregate((all, current) => all + LookupCollectionItemSeparator + current);
        }

        #endregion
        #region From String to Value

        public static FieldLookupValue ToLookupValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var lookupSplit = value.Split(new[] { LookupValueSeparator }, StringSplitOptions.None);

            int lookupId;
            return int.TryParse(lookupSplit[0], out lookupId)
                ? new FieldLookupValue {LookupId = lookupId}
                : null;
        }

        public static FieldLookupValue[] ToLookupCollectionValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            return value
                .Split(new[] { LookupCollectionItemSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(lv => ToLookupValue(lv))
                .Where(lv => lv != null)
                .ToArray();
        }

        public static FieldUrlValue ToUrlValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var valueSplit = value.Split(new[] {LookupValueSeparator}, StringSplitOptions.None);

            return new FieldUrlValue
            {
                Description = valueSplit[0],
                Url = valueSplit.Length > 0 ? valueSplit[1] : null
            };
        }

        public static string[] ToMultiChoiceValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            return value
                .Split(new[] {LookupCollectionItemSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static FieldUserValue[] ToUserCollectionValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            return value
                .Split(new[] { LookupCollectionItemSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(lv => ToUserValue(lv))
                .Where(lv => lv != null)
                .ToArray();
        }

        public static FieldUserValue ToUserValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var lookupSplit = value.Split(new[] { LookupValueSeparator }, StringSplitOptions.None);

            int lookupId;
            return int.TryParse(lookupSplit[0], out lookupId)
                ? new FieldUserValue { LookupId = lookupId }
                : null;
        }

        #endregion
    }
}
