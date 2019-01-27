using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class EmptyQueryFilterController : BaseQueryFilterController
    {
        public EmptyQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            return Enumerable.Empty<Control>();
        }

        public override string GetValue()
        {
            return null;
        }
    }
}
