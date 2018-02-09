using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;
using Xceed.Wpf.Toolkit;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Model.FieldType;
using System.Windows.Data;
using System.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class DateTimeQueryFilterController :BaseQueryFilterController
    {
        private DateTimeFieldTypeModel _model;

        public DateTimeQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
            _model = new DateTimeFieldTypeModel(field);
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            return new Control[]
            {
                BuildIncludeTimeControl(),
                BuildDateTimeControl(oldValue)
            };
        }

        private Control BuildIncludeTimeControl()
        {
            var ucIncludeTime = new CheckBox
            {
                MinWidth = _controlWidth,
                Margin = new Thickness(_controlMargin.Left, _controlMargin.Top + 4, _controlMargin.Right, _controlMargin.Bottom),
                Content = "Include time"
            };

            ucIncludeTime.Checked += (o, args) => OnValueChanged();
            ucIncludeTime.Unchecked += (o, args) => OnValueChanged();

            var modelBinding = new Binding(nameof(_model.IncludeTime))
            {
                Source = _model,
                Mode = BindingMode.TwoWay
            };
            ucIncludeTime.SetBinding(CheckBox.IsCheckedProperty, modelBinding);

            return ucIncludeTime;
        }

        private Control BuildDateTimeControl(string oldValue)
        {
            var control = new DateTimePicker
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,

                Format = DateTimeFormat.Custom,
                FormatString = "yyyy-MM-ddTHH:mm:ssZ",
                TimeFormat = DateTimeFormat.ShortTime,
                Watermark = "value",
                Text = oldValue
            };

            control.ValueChanged += (o, args) => OnValueChanged();
            control.LostFocus += (o, args) => OnValueChanged();

            var modelBinding = new Binding(nameof(_model.Date))
            {
                Source = _model,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(DateTimePicker.TextProperty, modelBinding);

            return control;
        }

        public override string GetValue()
        {
            return _model.Date ?? String.Empty;
        }

        public override void Refresh(IFilter filter)
        {
            var viewFilter = filter as Filter;
            if (viewFilter == null) return;

            _model.Date = viewFilter.FieldValue;

            var includeTime = ParseBool( viewFilter.ValueAttributes, "IncludeTimeValue", false);
            _model.IncludeTime = includeTime;
        }

        private bool ParseBool(Dictionary<string, string> collection, string key, bool defaultValue)
        {
            var value = collection
                .FirstOrDefault(a => 
                    a.Key.Equals("IncludeTimeValue", StringComparison.OrdinalIgnoreCase))
                .Value;

            return ParseBool(value, defaultValue);
        }

        private bool ParseBool(string value, bool defaultValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                bool parsedValue;
                if (bool.TryParse(value, out parsedValue))
                    return parsedValue;
                else
                    return defaultValue;
            }
            else return defaultValue;
        }

        protected override void UpdateFilter(Filter filter)
        {
            base.UpdateFilter(filter);

            filter.ValueAttributes.Add("IncludeTimeValue", _model.IncludeTime.ToString());
        }
    }
}
