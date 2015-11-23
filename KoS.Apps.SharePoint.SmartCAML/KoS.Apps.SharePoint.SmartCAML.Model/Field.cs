using System;
using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class Field
    {
        public string InternalName { get; set; }
        public string Title { get; set; }

        public bool IsReadonly { get; set; }
        public bool IsHidden { get; set; }
        public string Group { get; set; }
        public FieldType Type { get; set; }
        public Guid Id { get; set; }
        public SList List { get; set; }
    }
}
