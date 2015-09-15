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
        public Web Web { get; private set; }

        public Web Connect(string url)
        {
            if (!url.Contains("test")) return null;

            Web = new Web
            {
                Title = "Test",
                Url = url,
                Lists = new List<SharePointList>
                {
                    new SharePointList { Id = 1, Name = "List1" },
                    new SharePointList { Id = 2, Name = "List2" },
                    new SharePointList { Id = 3, Name = "List3" },
                    new SharePointList { Id = 4, Name = "List4" },
                    new SharePointList { Id = 5, Name = "List5" },
                    new SharePointList { Id = 6, Name = "List6" },
                    new SharePointList { Id = 7, Name = "List7" },
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
                        { "Title", "Test1" },
                        { "Owner", "Owner1" }
                    }
                },
                new ListItem { Id = 1, Columns = new Dictionary<string, string>
                    {
                        { "Title", "Test2" },
                        { "Owner", "Owner2" }
                    }
                },
                new ListItem { Id = 1, Columns = new Dictionary<string, string>
                    {
                        { "Title", "Test3" },
                        { "Owner", "Owner3" }
                    }
                },
            };
        }
    }
}
