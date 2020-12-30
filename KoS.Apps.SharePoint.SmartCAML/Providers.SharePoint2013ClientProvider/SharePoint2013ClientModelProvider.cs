using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Client = Microsoft.SharePoint.Client;
using Field = Microsoft.SharePoint.Client.Field;
using FieldType = Microsoft.SharePoint.Client.FieldType;
using ListItem = Microsoft.SharePoint.Client.ListItem;
using TaxonomyField = Microsoft.SharePoint.Client.Taxonomy.TaxonomyField;
using Web = KoS.Apps.SharePoint.SmartCAML.Model.Web;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider
{
    public class SharePoint2013ClientModelProvider : Model.ISharePointProvider
    {
        private string _userName;
        private string _password;

        public Model.Web Web { get; private set; }
        public bool IsSharePointOnline { get; private set; }

        public SharePoint2013ClientModelProvider(bool isOnline = false)
        {
            IsSharePointOnline = isOnline;
        }

        public async Task<Model.Web> Connect(string url)
        {
            using (var context = CreateContext(url?.Trim()))
            {
                context.Load(context.Web, 
                    w => w.Id,
                    w => w.Title);
                await Task.Factory.StartNew(() => context.ExecuteQuery());

                context.Load(context.Web.Lists, lists => lists.Include(
                    l => l.Id,
                    l => l.Hidden,
                    l => l.Title));

                await Task.Factory.StartNew(() => context.ExecuteQuery());

                Web = new Model.Web(this)
                {
                    Id = context.Web.Id,
                    Title = context.Web.Title,
                    Url = url
                };

                Web.Lists = context.Web.Lists.Cast<List>().Select(l => new Model.SList
                {
                    Web = Web,
                    Title = l.Title,
                    Id = l.Id,
                    IsHidden = l.Hidden
                }).ToList();

                return Web;
            }
        }

        public async Task<Web> Connect(string url, string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) != String.IsNullOrEmpty(password))
                throw new ArgumentException("The user or password is null.");

            _userName = userName;
            _password = password;

            return await Connect(url);
        }

        public async Task FillContentTypes(SList list, bool fillAlsoWeb = true)
        {
            using (var context = CreateContext(list.Web.Url))
            {
                if (list.ContentTypes == null)
                {
                    var serverList = context.Web.Lists.GetById(list.Id);
                    context.Load(serverList.ContentTypes, contentTypes => contentTypes.Include(
                        ct => ct.StringId,
                        ct => ct.Name
                        ));

                    await Task.Factory.StartNew(() => context.ExecuteQuery());

                    list.ContentTypes = Converter.ToContentTypes(serverList.ContentTypes);
                }

                if (list.Web.ContentTypes == null)
                {
                    context.Load(context.Web.ContentTypes, contentTypes => contentTypes.Include(
                        ct => ct.StringId,
                        ct => ct.Name
                        ));

                    await Task.Factory.StartNew(() => context.ExecuteQuery());

                    list.Web.ContentTypes = Converter.ToContentTypes(context.Web.ContentTypes);
                }
            }
        }

        public async Task<List<KeyValuePair<string, string>>> GetLookupItems(Model.FieldLookup lookup)
        {
            if (lookup.LookupWebId == Guid.Empty || string.IsNullOrEmpty(lookup.LookupList))
                return null;

            using (var context = CreateContext(lookup.List.Web.Url))
            {
                var lookupField = string.IsNullOrEmpty(lookup.LookupField) ? "Title" : lookup.LookupField;
                var caml = $@"
                    <Query>
                        <Where />
                    </Query>";

                ListItemCollection pageItems = null;
                var pageSize = 2000;
                var items = new List<KeyValuePair<string, string>>();

                var web = context.Site.OpenWebById(lookup.LookupWebId);
                var list = web.Lists.GetById(Guid.Parse(lookup.LookupList));

                do
                {
                    var rowLimit = $"<RowLimit>{pageSize}</RowLimit>";
                    var listQuery = new CamlQuery
                    {
                        ViewXml = $"<View Scope='RecursiveAll'>{caml}{rowLimit}</View>",
                        ListItemCollectionPosition = pageItems?.ListItemCollectionPosition
                    };

                    pageItems = list.GetItems(listQuery);
                    context.Load(pageItems, 
                        elements => elements.Include(
                            i => i.Id,
                            i => i[lookupField]
                        ),
                        elements => elements.ListItemCollectionPosition);

                    await Task.Factory.StartNew(() => context.ExecuteQuery());

                    items.AddRange(
                        pageItems
                            .Cast<ListItem>()
                            .Select(i => new KeyValuePair<string, string>(
                               i.Id.ToString(),
                               i[lookupField] != null ? i[lookupField].ToString() : string.Empty
                               )
                            )
                        );
                }
                while (pageItems?.ListItemCollectionPosition != null);

                return items
                    .OrderBy( i => i.Value)
                    .ToList();
            }
        }

        #region Get Items

        public async Task<List<Model.ListItem>> ExecuteQuery(Model.ListQuery query, int? pageSize)
        {
            using (var context = CreateContext(query.List.Web.Url))
            {
                var queryString = query.Query.StartsWith("<Query>", StringComparison.OrdinalIgnoreCase)
                    ? query.Query
                    : $"<Query>{query.Query}</Query>";

                var serverList = context.Web.Lists.GetById(query.List.Id);
                ListItemCollection pageItems = null;
                var items = new List<Model.ListItem>();

                do
                {
                    var rowLimit = pageSize.HasValue ? $"<RowLimit>{pageSize}</RowLimit>" : string.Empty;
                    var listQuery = new CamlQuery {
                        ViewXml = $"<View Scope='RecursiveAll'>{queryString}{rowLimit}</View>",
                        ListItemCollectionPosition = pageItems?.ListItemCollectionPosition
                    };

                    pageItems = serverList.GetItems(listQuery);
                    context.Load(pageItems);

                    await Task.Factory.StartNew(() => context.ExecuteQuery());

                    items.AddRange(
                        pageItems
                            .Cast<ListItem>()
                            .Select(i => new Model.ListItem(query.List)
                            {
                                Id = i.Id,
                                Columns =
                                    query.List
                                    .Fields
                                    .ToDictionary(
                                        f => f.InternalName,
                                        f => ElementSelector(f, i)
                                        )
                            })
                        );
                }
                while (pageItems?.ListItemCollectionPosition != null);

                return items;
            }
        }

        private string ElementSelector(Model.Field field, ListItem item)
        {
            if (!item.FieldValues.ContainsKey(field.InternalName)) return null;

            var value = item.FieldValues[field.InternalName];
            if (value == null) return null;

            switch (field.Type)
            {
                case Model.FieldType.Lookup:
                    if (value is FieldLookupValue) return Converter.LookupValueToString((FieldLookupValue)value);
                    if (value is FieldLookupValue[]) return Converter.LookupCollectionValueToString((FieldLookupValue[])value);
                    return value.ToString();

                case Model.FieldType.User:
                    if (value is FieldUserValue) return Converter.LookupValueToString((FieldUserValue)value);
                    if (value is FieldUserValue[]) return Converter.LookupCollectionValueToString((FieldUserValue[])value);
                    return value.ToString();

                case Model.FieldType.Url:
                    if (value is FieldUrlValue) return Converter.UrlValueToString((FieldUrlValue)value);
                    return value.ToString();

                case Model.FieldType.MultiChoice:
                    if (value is string[]) return Converter.ChoiceMultiValueToString((string[])value);
                    return value.ToString();

                case Model.FieldType.DateTime:
                    return ((DateTime)value).ToLocalTime().ToString(CultureInfo.CurrentCulture);

                case Model.FieldType.Taxonomy:
                    if (value is TaxonomyFieldValue) return Converter.TaxonomyValueToString((TaxonomyFieldValue) value);
                    if (value is TaxonomyFieldValueCollection) return Converter.TaxonomyCollectionValueToString((TaxonomyFieldValueCollection)value);
                    return value.ToString();

                default:
                    return value.ToString();

            }
        }

        #endregion

        #region List Fields

        public async Task FillListFields(Model.SList list)
        {
            using (var context = CreateContext(list.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(list.Id);
                context.Load(serverList.Fields, fields => fields.Include(
                    f => f.Id,
                    f => f.Hidden,
                    f => f.ReadOnlyField,
                    f => f.Title,
                    f => f.InternalName,
                    f => f.Group,
                    f => f.FieldTypeKind,
                    f => f.TypeAsString
                    ));

                await Task.Factory.StartNew(() => context.ExecuteQuery());

                var listFields = serverList.Fields.Cast<Field>().ToList();
                LoadFields(context, listFields);

                list.Fields = listFields.Select(f => CreateField(f)).ToList();
            }
        }

        private void LoadFields(ClientContext context, List<Field> listFields)
        {
            foreach (var listField in listFields)
            {
                switch (listField.FieldTypeKind)
                {
                    case FieldType.Lookup:
                        context.Load((Client.FieldLookup)listField,
                            f => f.AllowMultipleValues,
                            f => f.LookupField,
                            f => f.LookupList,
                            f => f.LookupWebId
                            );
                        break;
                    case FieldType.User:
                        context.Load((Client.FieldUser)listField,
                            f => f.AllowMultipleValues,
                            f => f.LookupField,
                            f => f.LookupList,
                            f => f.LookupWebId
                            );
                        break;
                    case FieldType.Choice:
                        context.Load((Client.FieldChoice)listField, f => f.Choices);
                        break;
                    case FieldType.MultiChoice:
                        context.Load((Client.FieldMultiChoice)listField, f => f.Choices);
                        break;
                    case FieldType.DateTime:
                        context.Load((Client.FieldDateTime)listField, f => f.DisplayFormat);
                        break;
                    case FieldType.Invalid:
                        switch (listField.TypeAsString)
                        {
                            case "TaxonomyFieldType":
                            case "TaxonomyFieldTypeMulti":
                                context.Load((TaxonomyField) listField,
                                    f => f.TermSetId,
                                    f => f.TextField
                                    );
                                break;
                        }
                        break;
                }
            }

            if (context.HasPendingRequest) context.ExecuteQuery();
        }

        private Model.Field CreateField(Field listField)
        {
            Model.Field field;

            switch (listField.FieldTypeKind)
            {
                case FieldType.Choice:
                    field = new Model.FieldChoice
                    {
                        Choices = ((Client.FieldChoice)listField).Choices
                    };
                    break;
                case FieldType.MultiChoice:
                    field = new Model.FieldChoice
                    {
                        Choices = ((Client.FieldMultiChoice)listField).Choices
                    };
                    break;

                case FieldType.DateTime:
                    field = new Model.FieldDateTime { DateOnly = ((Client.FieldDateTime)listField).DisplayFormat == DateTimeFieldFormatType.DateOnly };
                    break;

                case FieldType.Lookup:
                    field = new Model.FieldLookup {
                        AllowMultivalue = ((Client.FieldLookup)listField).AllowMultipleValues,
                        LookupField = ((Client.FieldLookup)listField).LookupField,
                        LookupList = ((Client.FieldLookup)listField).LookupList,
                        LookupWebId = ((Client.FieldLookup)listField).LookupWebId
                    };
                    break;

                case FieldType.User:
                    field = new Model.FieldLookup {
                        AllowMultivalue = ((Client.FieldUser)listField).AllowMultipleValues,
                        LookupField = ((Client.FieldLookup)listField).LookupField,
                        LookupList = ((Client.FieldUser)listField).LookupList,
                        LookupWebId = ((Client.FieldUser)listField).LookupWebId
                    };
                    break;

                case FieldType.Invalid:
                    switch (listField.TypeAsString)
                    {
                        case "TaxonomyFieldType":
                            field = new Model.FieldTaxonomy {AllowMultivalue = false, Type = Model.FieldType.Taxonomy, IsReadonly = true};
                            break;
                        case "TaxonomyFieldTypeMulti":
                            field = new Model.FieldTaxonomy { AllowMultivalue = true, Type = Model.FieldType.Taxonomy, IsReadonly = true};
                            break;
                        default:
                            field = new Model.Field();
                            break;
                    }
                    break;

                default:
                    field = new Model.Field();
                    break;
            }

            field.Id = listField.Id;
            field.IsHidden = listField.Hidden;
            field.IsReadonly |= listField.ReadOnlyField;
            field.Title = listField.Title;
            field.InternalName = listField.InternalName;
            field.Group = listField.Group;
            if( field.Type == 0 ) field.Type = (Model.FieldType)listField.FieldTypeKind;

            return field;
        }

        #endregion

        #region Save Items

        public async Task SaveItem(Model.ListItem item)
        {
            using (var context = CreateContext(item.List.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(item.List.Id);
                var serverItem = serverList.GetItemById(item.Id);

                foreach (var change in item.Changes)
                {
                    var field = item.List.Fields.First(f => f.InternalName == change.Key);
                    var value = ConvertFieldValue(field, item[change.Key]);

                    serverItem[change.Key] = value;
                }

                serverItem.Update();
                await Task.Factory.StartNew(() => context.ExecuteQuery());
            }
        }

        private object ConvertFieldValue(Model.Field field, string value)
        {
            if (field.Type == Model.FieldType.Lookup)
            {
                if (((Model.FieldLookup) field).AllowMultivalue) return Converter.ToLookupCollectionValue(value);
                else return Converter.ToLookupValue(value);
            }
            if (field.Type == Model.FieldType.Url) return Converter.ToUrlValue(value);
            if (field.Type == Model.FieldType.MultiChoice) return Converter.ToMultiChoiceValue(value);
            if (field.Type == Model.FieldType.User)
            {
                if (((Model.FieldLookup)field).AllowMultivalue) return Converter.ToUserCollectionValue(value);
                else return Converter.ToUserValue(value);
            }
            if (field.Type == Model.FieldType.DateTime) return DateTime.Parse(value).ToUniversalTime();
            if (field.Type == Model.FieldType.Taxonomy)
            {
                if (((Model.FieldTaxonomy)field).AllowMultivalue) return Converter.ToTaxonomyCollectionValue(value);
                else return Converter.ToTaxonomyValue(value);
            }

            return value;
        }

        #endregion

        #region Create Context

        private ClientContext CreateContext(string url)
        {
            var context = new ClientContext(url);

            var authManager = new PnP.Framework.AuthenticationManager();
            return authManager.GetContext(url);

            //if (!String.IsNullOrEmpty(_userName))
            //{
            //    context.Credentials = IsSharePointOnline
            //        ? (ICredentials)new Microsoft.SharePoint.Client. SharePointOnlineCredentials(_userName, ConvertPassword(_password))
            //        : new NetworkCredential(_userName, _password);
            //}
            //else
            //{
            //    // Ensure use of Windows Authentication
            //    //Add the header that tells SharePoint to use Windows authentication.
            //    context.ExecutingWebRequest += (sender, args) => args.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            //}

            return context;
        }

        private SecureString ConvertPassword(string password)
        {
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }

            return securePassword;
        }

        #endregion
    }
}
