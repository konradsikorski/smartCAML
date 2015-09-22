using System;
using System.Collections.Generic;
using System.Linq;
using KoS.Apps.SharePoint.SmartCAML.Model;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using Microsoft.SharePoint;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ServerProvider
{
    public class SharePoint2013ServerModelProvider : ISharePointProvider
    {
        public Web Web { get; private set; }

        public Web Connect(string url)
        {
            using (var site = new SPSite(url))
            {
                using (var web = site.OpenWeb())
                {
                    Web = new Web
                    {
                        Id = web.ID,
                        Url = url,
                        Title = web.Title
                    };

                    Web.Lists = web.Lists.Cast<SPList>().Select(l => new SList
                    {
                        Web = Web,
                        Title = l.Title,
                        Id = l.ID,
                        IsHidden = l.Hidden
                    }).ToList();

                    return Web;
                }
            }
        }

        public List<ListItem> ExecuteQuery(ListQuery query)
        {
            using (var site = new SPSite(query.List.Web.Id))
            {
                using (var web = site.OpenWeb())
                {
                    var serverList = web.Lists.TryGetList(query.List.Title);
                    var listQuery = new SPQuery {Query = query.Query};

                    return serverList.GetItems(listQuery).Cast<SPListItem>()
                        .Select(i => new ListItem
                        {
                            Id = i.ID,
                            Columns =
                                query.List
                                .Fields
                                .ToDictionary(
                                    f => f.InternalName,
                                    f => i.GetFormattedValue(f.InternalName))
                        })
                        .ToList();
                }
            }
        }

        public void FillListFields(SList list)
        {
            using (var site = new SPSite(list.Web.Id))
            {
                using (var web = site.OpenWeb())
                {
                    var serverList = web.Lists.TryGetList(list.Title);
                    list.Fields = serverList.Fields.Cast<SPField>().Select(f => new Field
                    {
                        Id = f.Id,
                        IsHidden = f.Hidden,
                        IsReadonly = f.ReadOnlyField,
                        Title = f.Title,
                        InternalName = f.InternalName,
                        Group = f.Group,
                        Type = (FieldType)f.Type
                    }).ToList();
                }
            }
        }
    }
}
