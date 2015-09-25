using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class ListItem
    {
        public int Id { get; set; }
        public Dictionary<string, string> Columns = new Dictionary<string, string>();

        public string this[string name]
        {
            get
            {
                return Columns.ContainsKey(name)
                    ? Columns[name]
                    : null;
            }
        }

    }
}
