using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class InQueryFilterController : BaseQueryFilterController
    {
        public InQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var control = CreateControl(oldValue);

            return new[] { control };
        }

        private TextBox CreateControl(string text = "")
        {
            var control = new TextBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
                Text = text,
            };

            control.TextChanged += ControlOnTextChanged;
            control.LostFocus += (o, args) => OnValueChanged();

            return control;
        }

        private void ControlOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var last = (TextBox) Controls.Last();
            var current = (TextBox) sender;
            var lastIsEmpty = string.IsNullOrEmpty(last.Text);
            var currentIsEmpty = string.IsNullOrEmpty(current.Text);

            if (!currentIsEmpty && !lastIsEmpty)
            {
                AddToParent(CreateControl());
            }
            else if (currentIsEmpty && lastIsEmpty)
            {
                RemoveFromParent(last);
            }

            OnValueChanged();
        }

        public override string GetValue()
        {
            var control = Controls.FirstOrDefault() as TextBox;
            return control?.Text;
        }

        public override void Refresh(IFilter filter)
        {
            var viewFilter = filter as InFilter;
            if (viewFilter == null) return;
            if (viewFilter.FieldValues == null) return;

            RemoveAllValueControls();

            foreach (var value in viewFilter.FieldValues)
            {
                if (string.IsNullOrWhiteSpace(value)) continue;

                AddToParent(CreateControl(value));
            }

            AddToParent(CreateControl());
        }

        private void RemoveAllValueControls()
        {
            while(Controls.Count > 0)
            {
                RemoveFromParent(Controls[0]);
            }
        }

        public override IFilter GetFilter(QueryOperator queryOperator)
        {
            return new InFilter
            {
                QueryOperator = queryOperator,
                FieldInternalName = Field.InternalName,
                FieldType = Field.Type.ToString(),
                FieldValues = Controls.Cast<TextBox>().Select( tb => tb.Text)
            };
        }
    }
}
