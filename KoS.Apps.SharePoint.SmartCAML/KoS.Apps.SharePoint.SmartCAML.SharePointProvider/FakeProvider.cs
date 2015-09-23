using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public class FakeProvider : ISharePointProvider
    {
        private class Internals
        {
            public const string Title = "TitleInternal";
            public const string Owner = "OwnerInternal";
        }

        public Web Web { get; set; }

        public Web Connect(string url)
        {
            if (!url.Contains("test")) return null;

            Web = new Web
            {
                Id = Guid.Empty,
                Title = "Test",
                Url = url
            };

            Web.Lists = new List<SList>
            {
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), Title = "List1", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2), Title = "List2", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3), Title = "List3", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4), Title = "List4", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5), Title = "List5", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6), Title = "List6", Web = Web},
                new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7), Title = "List7", Web = Web},
            };

            return Web;
        }

        public List<ListItem> ExecuteQuery(ListQuery query)
        {
            return new List<ListItem>
            {
                new ListItem { Id = 1, Columns = new Dictionary<string, string>
                    {
                        { Internals.Title, "Test1" },
                        { Internals.Owner, "Owner1" }
                    }
                },
                new ListItem { Id = 1, Columns = new Dictionary<string, string>
                    {
                        { Internals.Title, "Test2" },
                        { Internals.Owner, "Owner2" }
                    }
                },
                new ListItem { Id = 1, Columns = new Dictionary<string, string>
                    {
                        { Internals.Title, "Test3" },
                        { Internals.Owner, "Owner3" }
                    }
                },
            };
        }

        public void FillListFields(SList list)
        {
            var fields = new List<Field>
            {
                new Field { Group = "1", Title = "Title", InternalName = Internals.Title },
                new Field { Group = "1", Title = "Owner", InternalName = Internals.Owner }
            };

            list.Fields = fields;
        }
    }
}
