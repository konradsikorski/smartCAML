using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    abstract class BaseQueryFilterController : IQueryFilterController
    {
        protected readonly int _controlWidth = 100;
        protected readonly Thickness _controlMargin = new Thickness(4, 0, 0, 0);

        public Field Field { get; private set; }
        public FilterOperator? FilterOperator { get; private set; }
        protected List<Control> Controls { get; } = new List<Control>();
        protected Panel Parent { get; private set; }

        public event EventHandler ValueChanged;

        protected BaseQueryFilterController(Field field, FilterOperator? filterOperator)
        {
            Field = field;
            FilterOperator = filterOperator;
        }

        public void Initialize(Panel parent, string oldValue)
        {
            this.Parent = parent;
            Controls.AddRange(InitializeControls(oldValue));

            foreach (var chield in Controls)
            {
                Parent.Children.Add(chield);
            }
        }

        protected abstract IEnumerable<Control> InitializeControls(string oldValue);

        protected void AddToParent(Control control)
        {
            Controls.Add(control);
            Parent.Children.Add(control);
        }

        protected void RemoveFromParent(Control control)
        {
            Parent.Children.Remove(control);
            Controls.Remove(control);
        }

        public void Dispose()
        {
            foreach (var chield in Controls)
            {
                Parent.Children.Remove(chield);
            }
        }

        public abstract string GetValue();

        protected void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual IFilter GetFilter(QueryOperator queryOperator)
        {
            var filter = new Filter
            {
                QueryFilter = FilterOperator,
                QueryOperator = queryOperator,
                FieldInternalName = Field.InternalName,
                FieldType = Field.Type.ToString(),
                FieldValue = GetValue()
            };

            UpdateFilter(filter);

            return filter;
        }

        protected virtual void UpdateFilter(Filter filter)
        {
        }
    }
}
