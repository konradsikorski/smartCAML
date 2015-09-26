using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.SharePoint.Client;
using Web = KoS.Apps.SharePoint.SmartCAML.Model.Web;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider
{
    public class SharePoint2013ClientModelProvider : Model.ISharePointProvider
    {
        private string _userName;
        private string _password;

        public Model.Web Web { get; private set; }

        private ClientContext CreateContext(string url)
        {
            var context = new ClientContext(url);

            if (!String.IsNullOrEmpty(_userName))
                context.Credentials = new NetworkCredential(_userName, _password);

            return context;
        }

        public Model.Web Connect(string url)
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

                context.ExecuteQuery();

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

        public Web Connect(string url, string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) != String.IsNullOrEmpty(password))
                throw new ArgumentException("The user or password is null.");

            _userName = userName;
            _password = password;

            return Connect(url);
        }

        public List<Model.ListItem> ExecuteQuery(Model.ListQuery query)
        {
            using (var context = CreateContext(query.List.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(query.List.Id);
                var listQuery = new CamlQuery{ ViewXml = $"<View><Query>{query.Query}</Query></View>" };

                var items = serverList.GetItems(listQuery);
                context.Load(items
                    //,i => i.Include(item => item.Id)
                    );

                context.ExecuteQuery();

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

        private string ElementSelector(Model.Field f, ListItem i)
        {
            return i.FieldValues.ContainsKey(f.InternalName)
                ? i.FieldValues[f.InternalName]?.ToString()
                : null;
        }


        public void FillListFields(Model.SList list)
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

                context.ExecuteQuery();

                list.Fields = serverList.Fields.Cast<Field>().Select(f => new Model.Field
                {
                    Id = f.Id,
                    IsHidden = f.Hidden,
                    IsReadonly = f.ReadOnlyField,
                    Title = f.Title,
                    InternalName = f.InternalName,
                    Group = f.Group,
                    Type = (Model.FieldType) f.FieldTypeKind
                }).ToList();
            }
        }

        public void SaveItem(Model.ListItem item)
        {
            using (var context = CreateContext(item.List.Web.Url))
            {
                var serverList = context.Web.Lists.GetById(item.List.Id);
                var serverItem = serverList.GetItemById(item.Id);

                foreach (var change in item.Changes)
                {
                    serverItem[change.Key] = change.Value;
                }

                serverItem.Update();

                context.Load(serverItem);
                context.ExecuteQuery();
            }
        }
    }
}
