using System;
using System.Collections.Generic;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class DropdownQueryFilterController : BaseQueryFilterController
    {
        protected ComboBox _control;

        public DropdownQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            _control = new ComboBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                IsEditable = true,
                Text = oldValue
            };

            _control.SelectionChanged += (o, args) => OnValueChanged();
            _control.LostFocus += (o, args) => OnValueChanged();
            _control.DropDownOpened += Value_DropDownOpened;

            return new[] {_control};
        }

        protected virtual async void Value_DropDownOpened(object sender, EventArgs e)
        {
        }

        public override string GetValue()
        {
            return String.IsNullOrEmpty(_control.SelectedValue?.ToString()) 
                ? _control.Text ?? String.Empty 
                : _control.SelectedValue?.ToString();
        }
    }
}
