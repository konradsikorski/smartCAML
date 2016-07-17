using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class LookupQueryFilterController : DropdownQueryFilterController
    {
        private ComboBox _ucLookupAs;
        private bool LookupAsId => _ucLookupAs.SelectedIndex == 0;

        public LookupQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _ucLookupAs = new ComboBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                IsEditable = false,
                ItemsSource = new[] { "by lookup id", "by lookup text" },
                SelectedIndex = 0
            };

            _ucLookupAs.SelectionChanged += (o, args) => OnValueChanged();

            return new[]
            {
                _ucLookupAs
            }.Concat(controls);
        }

        protected override void UpdateFilter(Filter filter)
        {
            base.UpdateFilter(filter);

            if(LookupAsId) filter.FieldRefAttributes.Add("LookupId", "TRUE");
        }
    }
}
