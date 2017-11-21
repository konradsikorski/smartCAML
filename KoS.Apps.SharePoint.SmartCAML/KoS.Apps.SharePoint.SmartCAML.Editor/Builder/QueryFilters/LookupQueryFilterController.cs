using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class LookupQueryFilterController : DropdownQueryFilterController
    {
        private ComboBox _ucLookupAs;
        private bool LookupAsId => _ucLookupAs.SelectedIndex == 0;
        private bool DataLoaded { get; set; }

        public LookupQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            return new[]
            {
                BuildLookupIdSwitch()
            }
            .Concat(controls);
        }

        private Control BuildLookupIdSwitch()
        {
            _ucLookupAs = new ComboBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                IsEditable = false,
                ItemsSource = new[] { "by lookup id", "by lookup text" },
                SelectedIndex = 0
            };

            _ucLookupAs.SelectionChanged += (o, args) => OnValueChanged();

            return _ucLookupAs;
        }

        protected override async void Value_DropDownOpened(object sender, EventArgs e)
        {
            base.Value_DropDownOpened(sender, e);

            if (!DataLoaded)
            {
                DataLoaded = true;

                try
                {
                    StatusNotification.NotifyWithProgress("Loading list items");
                    var items =  await GetListItems();
                    _control.DisplayMemberPath = "Value";
                    _control.SelectedValuePath = "Key";
                    _control.ItemsSource = _control.ItemsSource.Cast<KeyValuePair<string, string>>().Concat(items);
                    StatusNotification.Notify("List items loaded");
                }
                catch(Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                    StatusNotification.Notify("Loading list items failed");
                }
            }
        }

        public override string GetValue()
        {
            return !LookupAsId || String.IsNullOrEmpty(_control.SelectedValue?.ToString())
                ? _control.Text ?? String.Empty
                : _control.SelectedValue?.ToString();
        }

        protected async Task<List<KeyValuePair<string,string>>> GetListItems()
        {
            return await Field.List.Web.Client.GetLookupItems((FieldLookup)Field);
        }

        protected override void UpdateFilter(Filter filter)
        {
            base.UpdateFilter(filter);

            if(LookupAsId) filter.FieldRefAttributes.Add("LookupId", "TRUE");
        }
    }
}
