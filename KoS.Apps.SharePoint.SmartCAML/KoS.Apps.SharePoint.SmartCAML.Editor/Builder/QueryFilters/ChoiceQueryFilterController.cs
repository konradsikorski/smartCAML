using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class ChoiceQueryFilterController : DropdownQueryFilterController
    {
        private readonly string[] _choices;

        public ChoiceQueryFilterController(Field field, FilterOperator? filterOperator) : base(field, filterOperator)
        {
            _choices = ((FieldChoice)field).Choices.OrderBy(c => c).ToArray();
        }

        protected override IEnumerable<Control> InitializeControls(string oldValue)
        {
            var controls = base.InitializeControls(oldValue);

            _control.ItemsSource = _choices;
            
            return controls;
        }
    }
}
