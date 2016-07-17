using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class ContentTypeIdQueryFilterController : DropdownQueryFilterController
    {
        public ContentTypeIdQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _control.DisplayMemberPath = "Name";
            _control.SelectedValuePath = "Id";

            _control.DropDownOpened += async (sender, args) =>
            {
                if (Field.List.ContentTypes == null || Field.List.Web.ContentTypes == null)
                {
                    StatusNotification.NotifyWithProgress("Loading Content Types");
                    await Field.List.Web.Client.FillContentTypes(Field.List);
                    StatusNotification.Notify("Content Types loaded");
                }

                var contentTypes = Field.List.ContentTypes
                    .OrderBy(ct => ct.Name)
                    .Concat(Field.List.Web.ContentTypes.OrderBy(ct => ct.Name));

                _control.ItemsSource =
                    contentTypes.Select(ct => new
                    {
                        ct.Id,
                        Name = Field.List.ContentTypes.Contains(ct) ? "List." + ct.Name : ct.Name
                    });
            };

            return controls;
        }
    }
}
