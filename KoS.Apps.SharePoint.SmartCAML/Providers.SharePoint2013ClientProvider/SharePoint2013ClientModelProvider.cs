using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = KoS.Apps.SharePoint.SmartCAML.Model;
using Microsoft.SharePoint.Client;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ClientProvider
{
    public class SharePoint2013ClientModelProvider : Model.ISharePointProvider
    {
        public Model.Web Web { get; private set; }

        public Model.Web Connect(string url)
        {
            using (var context = new ClientContext(url))
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

        public List<Model.ListItem> ExecuteQuery(Model.ListQuery query)
        {
            throw new NotImplementedException();
        }

        public void FillListFields(Model.SList list)
        {
            using (var context = new ClientContext(list.Web.Url))
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
    }
}
