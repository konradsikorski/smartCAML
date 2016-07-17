using System.Collections.Generic;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class UserQueryFilterController : DropdownQueryFilterController
    {
        public UserQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _control.DisplayMemberPath = "Text";
            _control.SelectedValuePath = "Value";
            _control.ItemsSource = new[]
            {
                new {Value = "0", Text = "False"},
                new {Value = "1", Text = "True"}
            };

            return controls;
        }
    }
}
