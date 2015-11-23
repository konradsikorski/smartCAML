using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Microsoft.SharePoint.Client;
using Client = Microsoft.SharePoint.Client;
using Field = Microsoft.SharePoint.Client.Field;
using FieldType = Microsoft.SharePoint.Client.FieldType;
using ListItem = Microsoft.SharePoint.Client.ListItem;
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
            using (var context = CreateContext(url))
            {
                context.Load(context.Web, 
                    w => w.Id,
                    w => w.Title);

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

        public async Task<List<Model.ListItem>> ExecuteQuery(Model.ListQuery query)
        {
            using (var context = CreateContext(query.List.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(query.List.Id);
                var listQuery = new CamlQuery{ ViewXml = $"<View><Query>{query.Query}</Query></View>" };

                var items = serverList.GetItems(listQuery);
                context.Load(items
                    //,i => i.Include(item => item.Id)
                    );

                await Task.Factory.StartNew(() => context.ExecuteQuery());

                return items.Cast<ListItem>()
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
                        .ToList();
            }
        }

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
                    f => f.FieldTypeKind));

                await Task.Factory.StartNew(() => context.ExecuteQuery());

                var listFields = serverList.Fields.Cast<Field>().ToList();
                LoadFields(context, listFields);

                list.Fields = listFields.Select(f => CreateField(f)).ToList();
            }
        }

        public async Task SaveItem(Model.ListItem item)
        {
            using (var context = CreateContext(item.List.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(item.List.Id);
                var serverItem = serverList.GetItemById(item.Id);

                foreach (var change in item.Changes)
                {
                    serverItem[change.Key] = item[change.Key];
                }

                serverItem.Update();
                await Task.Factory.StartNew(() => context.ExecuteQuery());
            }
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

        private string ElementSelector(Model.Field f, ListItem i)
        {
            return i.FieldValues.ContainsKey(f.InternalName)
                ? i.FieldValues[f.InternalName]?.ToString()
                : null;
        }

        private void LoadFields(ClientContext context, List<Field> listFields)
        {
            foreach (var listField in listFields)
            {
                switch (listField.FieldTypeKind)
                {
                    case FieldType.Choice:
                        context.Load((Client.FieldChoice)listField, f => f.Choices);
                        break;
                    case FieldType.MultiChoice:
                        context.Load((Client.FieldMultiChoice)listField, f => f.Choices);
                        break;
                    case FieldType.DateTime:
                        context.Load((Client.FieldDateTime)listField, f => f.DisplayFormat);
                        break;
                }
            }

            if(context.HasPendingRequest) context.ExecuteQuery();
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
                    field = new Model.FieldDateTime {DateOnly = ((Client.FieldDateTime)listField).DisplayFormat == DateTimeFieldFormatType.DateOnly };
                    break;

                case FieldType.Lookup:
                    field = new Model.FieldLookup();
                    break;

                default:
                    field = new Model.Field();
                    break;
            }

            field.Id = listField.Id;
            field.IsHidden = listField.Hidden;
            field.IsReadonly = listField.ReadOnlyField;
            field.Title = listField.Title;
            field.InternalName = listField.InternalName;
            field.Group = listField.Group;
            field.Type = (Model.FieldType) listField.FieldTypeKind;

            return field;
        }

        #region Create Context

        private ClientContext CreateContext(string url)
        {
            var context = new ClientContext(url);

            if (!String.IsNullOrEmpty(_userName))
            {
                context.Credentials = IsSharePointOnline
                    ? (ICredentials)new SharePointOnlineCredentials(_userName, ConvertPassword(_password))
                    : new NetworkCredential(_userName, _password);
            }
            else
            {
                // Ensure use of Windows Authentication
                //Add the header that tells SharePoint to use Windows authentication.
                context.ExecutingWebRequest += (sender, args) => args.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            }

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
