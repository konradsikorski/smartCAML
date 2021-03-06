﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Microsoft.SharePoint;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ServerProvider
{
    public class SharePoint2013ServerModelProvider : ISharePointProvider
    {
        private string _userName;
        private string _password;

        public Web Web { get; private set; }

        private SPSite CreateSite(string url)
        {
            var site = new SPSite(url);

            if (!String.IsNullOrEmpty(_userName))
            {
                using (site)
                {
                    using (var web = site.OpenWeb())
                    {
                        var userToken = web.AllUsers[_userName].UserToken;
                        return new SPSite(url, userToken);
                    }
                }
            }

            return site;
        }

        public async Task<Web> Connect(string url)
        {
            using (var site = CreateSite(url))
            {
                using (var web = site.OpenWeb())
                {
                    Web = new Web(this)
                    {
                        Id = web.ID,
                        Url = url,
                        Title = web.Title
                    };

                    await Task.Factory.StartNew(() =>
                    {
                        Web.Lists = web.Lists.Cast<SPList>().Select(l => new SList
                        {
                            Web = Web,
                            Title = l.Title,
                            Id = l.ID,
                            IsHidden = l.Hidden
                        }).ToList();
                    });

                    return Web;
                }
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

        public async Task<List<ListItem>> ExecuteQuery(ListQuery query, int pageSize)
        {
            using (var site = CreateSite(query.List.Web.Url))
            {
                using (var web = site.OpenWeb())
                {
                    var serverList = web.Lists.TryGetList(query.List.Title);
                    var listQuery = new SPQuery
                    {
                        Query = query.Query,
                        ViewAttributes = "Scope='RecursiveAll'"
                    };

                    return await Task.Factory.StartNew(() =>
                        serverList.GetItems(listQuery).Cast<SPListItem>()
                            .Select(i => new ListItem(query.List)
                            {
                                Id = i.ID,
                                Columns =
                                    query.List
                                        .Fields
                                        .ToDictionary(
                                            f => f.InternalName,
                                            f => i.GetFormattedValue(f.InternalName))
                            })
                            .ToList()
                    );
                }
            }
        }

        public async Task FillListFields(SList list)
        {
            using (var site = CreateSite(list.Web.Url))
            {
                using (var web = site.OpenWeb())
                {
                    await Task.Factory.StartNew(() =>
                    {
                        var serverList = web.Lists.TryGetList(list.Title);
                        list.Fields = serverList.Fields.Cast<SPField>()
                            .Select(f => CreateField(f))
                            .ToList();
                    });
                }
            }
        }

        public async Task SaveItem(ListItem item)
        {
            using (var site = CreateSite(item.List.Web.Url))
            {
                using (var web = site.OpenWeb())
                {
                    await Task.Factory.StartNew(() =>
                    {
                        var serverList = web.Lists[item.List.Id];
                        var serverItem = serverList.GetItemById(item.Id);

                        foreach (var change in item.Changes)
                        {
                            serverItem[change.Key] = change.Value;
                        }

                        serverItem.Update();
                    });
                }
            }
        }

        private Model.Field CreateField(SPField listField)
        {
            Model.Field field;

            switch (listField.Type)
            {
                case SPFieldType.Choice:
                    field = new Model.FieldChoice
                    {
                        Choices = ((SPFieldChoice)listField).Choices.Cast<string>().ToList()
                    };
                    break;
                case SPFieldType.MultiChoice:
                    field = new Model.FieldChoice
                    {
                        Choices = ((SPFieldMultiChoice)listField).Choices.Cast<string>().ToList()
                    };
                    break;

                case SPFieldType.DateTime:
                    field = new Model.FieldDateTime { DateOnly = ((SPFieldDateTime)listField).DisplayFormat == SPDateTimeFieldFormatType.DateOnly };
                    break;

                case SPFieldType.Lookup:
                    field = new Model.FieldLookup {AllowMultivalue = ((SPFieldLookup)listField).AllowMultipleValues};
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
            field.Type = (Model.FieldType)listField.Type;

            return field;
        }

        public async Task FillContentTypes(SList list, bool fillAlsoWeb = true)
        {

            using (var site = CreateSite(list.Web.Url))
            {
                using (var web = site.OpenWeb())
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (list.ContentTypes == null)
                        {
                            var serverList = web.Lists.TryGetList(list.Title);
                            list.ContentTypes = Converter.ToContentTypes(serverList.ContentTypes);
                        }

                        if (list.Web.ContentTypes == null)
                        {
                            list.Web.ContentTypes = Converter.ToContentTypes(web.ContentTypes);
                        }
                    });
                }
            }
        }

        public Task<List<KeyValuePair<string, string>>> GetLookupItems(FieldLookup lookup)
        {
            throw new NotImplementedException();
        }
    }
}
