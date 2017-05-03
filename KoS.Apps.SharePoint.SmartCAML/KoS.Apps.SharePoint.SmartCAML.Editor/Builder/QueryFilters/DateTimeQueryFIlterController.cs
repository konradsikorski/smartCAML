using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Xceed.Wpf.Toolkit;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class DateTimeQueryFilterController :BaseQueryFilterController
    {
        private readonly bool _dateOnly;
        private DateTimePicker _control;

        public DateTimeQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
            _dateOnly = ((FieldDateTime)field).DateOnly;
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            // todo: http://www.codeproject.com/Articles/414414/SharePoint-Working-with-Dates-in-CAML
            var ucIncludeTime = new CheckBox
            {
                MinWidth = _controlWidth,
                Margin = new Thickness(_controlMargin.Left, _controlMargin.Top + 4, _controlMargin.Right, _controlMargin.Bottom),                
                Content = "Include time",
                IsChecked = !_dateOnly
            };

            _control = new DateTimePicker
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,

                Format = DateTimeFormat.Custom,
                FormatString = "yyyy-MM-ddTHH:mm:ssZ",
                TimeFormat = DateTimeFormat.ShortTime,
                Watermark = "value",
                Text = oldValue
            };

            _control.ValueChanged += (o, args) => OnValueChanged();
            _control.LostFocus += (o, args) => OnValueChanged();

            return new Control[]
            {
                ucIncludeTime,
                _control
            };
        }

        public override string GetValue()
        {
            return _control.Text ?? String.Empty;
        }
    }
}
