using System.Collections.Generic;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class BooleanQueryFilterController : DropdownQueryFilterController
    {
        public BooleanQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _control.ItemsSource = new[] { "@Me", "Browse..." };

            return controls;
        }
    }
}
