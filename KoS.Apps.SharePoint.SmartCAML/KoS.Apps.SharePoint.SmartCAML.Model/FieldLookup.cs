using System;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class FieldLookup : Field, IMultiValueField
    {
        public bool AllowMultivalue { get; set; }
        public Guid LookupWebId { get; set; }
        public string LookupList { get; set; }
        public string LookupField { get; set; }
    }
}
