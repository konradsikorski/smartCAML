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

        public Web Web { get; private set; }

        public Web Connect(string url)
        {
            if (!url.Contains("test")) return null;

            var fields = new List<Field>
            {
                new Field { Group = "1", Title = "Title", InternalName = Internals.Title },
                new Field { Group = "1", Title = "Owner", InternalName = Internals.Owner }
            };

            Web = new Web
            {
                Title = "Test",
                Url = url,
                Lists = new List<SharePointList>
                {
                    new SharePointList { Id = 1, Name = "List1", Fields =  fields},
                    new SharePointList { Id = 2, Name = "List2", Fields =  fields },
                    new SharePointList { Id = 3, Name = "List3", Fields =  fields },
                    new SharePointList { Id = 4, Name = "List4", Fields =  fields },
                    new SharePointList { Id = 5, Name = "List5", Fields =  fields },
                    new SharePointList { Id = 6, Name = "List6", Fields =  fields },
                    new SharePointList { Id = 7, Name = "List7", Fields =  fields },
                }
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
    }
}
