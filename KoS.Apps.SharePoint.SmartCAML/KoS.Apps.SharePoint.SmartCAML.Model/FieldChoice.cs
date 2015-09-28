using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class FieldChoice : Field, IMultiValueField
    {
        public FieldChoice() { Type = FieldType.Choice; }
        public IEnumerable<string> Choices { get; set; }

        public bool AllowMultivalue => Type == FieldType.MultiChoice;
    }
}
