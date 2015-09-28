using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class ListItem
    {
        public int Id { get; set; }
        public SList List { get; private set; }
        // Key: field internal name; Value: field value
        public Dictionary<string, string> Columns = new Dictionary<string, string>();
        // Key: field internal name; Value: old (original) field value
        public readonly Dictionary<string, string> Changes = new Dictionary <string, string>();

        public bool IsDirty => Changes.Keys.Count > 0;

        public ListItem(SList list)
        {
            List = list;
        }

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
                var oldValue = Columns[name];
                if (oldValue == value) return;

                Columns[name] = value;
                if (Changes.ContainsKey(name))
                {
                    // if new value is the same as old vale than remove change from list
                    if (value == Changes[name]) Changes.Remove(name);
                }
                else Changes.Add(name, oldValue);
            }
        }

        public void Update()
        {
            List.Web.Client.SaveItem(this);
            Saved();
        }

        public void Saved()
        {
            Changes.Clear();
        }

        public void CancelChanges()
        {
            foreach (var change in Changes)
            {
                if (Columns.ContainsKey(change.Key))
                    Columns[change.Key] = change.Value;
            }

            Saved();
        }
    }
}
