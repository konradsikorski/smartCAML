using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class MembershipQueryFilterController : DropdownQueryFilterController
    {
        public MembershipQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls =  base.InitializeControls(oldValue);

            _control.Text = null;
            _control.ItemsSource = new[]
            {
                "SPWeb.AllUsers",
                "SPGroup",
                "SPWeb.Groups",
                "CurrentUserGroups",
                "SPWeb.Users",
            };

            return controls;
        }

        public override string GetValue()
        {
            return null;
        }

        protected override void UpdateFilter(Filter filter)
        {
            base.UpdateFilter(filter);

            filter.FilterAttributes.Add("Type", _control.Text);
        }

        public override void Refresh(IFilter filter)
        {
            var viewFilter = filter as Filter;
            if (viewFilter == null) return;

            _control.Text = viewFilter.FilterAttributes.FirstOrDefault(a => a.Key == "Type").Value ?? string.Empty;
        }
    }
}
