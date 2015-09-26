using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class ListItem
    {
        public int Id { get; set; }
        public Dictionary<string, string> Columns = new Dictionary<string, string>();
        public List<string> Changes = new List<string>();

        public string this[string name]
        {
            get
            {
                return Columns.ContainsKey(name)
                    ? Columns[name]
                    : null;
            }
            set
            {
                if (!Columns.ContainsKey(name)) return;

                Columns[name] = value;
                if( !Changes.Contains(name)) Changes.Add(name);
            }
        }

    }
}
