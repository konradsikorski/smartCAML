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
            var control = CreateControl();
            control.Text = oldValue;

            return new[] { control };
        }

        private TextBox CreateControl()
        {
            var control = new TextBox
            {
                MinWidth = _controlWidth,
                Margin = _controlMargin,
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
