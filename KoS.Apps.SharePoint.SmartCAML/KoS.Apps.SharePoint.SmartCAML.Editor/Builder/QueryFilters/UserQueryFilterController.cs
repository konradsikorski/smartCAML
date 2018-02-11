using System.Collections.Generic;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Core;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class UserQueryFilterController : LookupQueryFilterController
    {
        public UserQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _control.DisplayMemberPath = "Value";
            _control.SelectedValuePath = "Key";
            _control.ItemsSource = new[] { new KeyValuePair<string, string>(Consts.UserId, Consts.UserId) };

            return controls;
        }
    }
}
