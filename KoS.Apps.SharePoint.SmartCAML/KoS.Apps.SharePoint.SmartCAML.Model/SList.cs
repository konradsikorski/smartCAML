using System;
using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class SList
    {
        private List<Field> _fields = new List<Field>();
        public Web Web { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsHidden { get; set; }
        public List<ContentType> ContentTypes { get; set; }

        public List<Field> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                _fields.ForEach( f => f.List = this);
            }
        }
    }
}
