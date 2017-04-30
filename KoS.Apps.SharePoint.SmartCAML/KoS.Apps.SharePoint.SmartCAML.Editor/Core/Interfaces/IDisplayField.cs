using KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces
{
    interface IDisplayField
    {
        CollectionViewSource FieldsViewSource { get; }
        BoolToStringConverter DisplayMemberConverter { get; }
    }
}
