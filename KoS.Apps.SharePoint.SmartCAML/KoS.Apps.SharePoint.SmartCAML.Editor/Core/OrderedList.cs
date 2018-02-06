using KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Core
{
    public class OrderedList<T> where T: IOrderListElement, new()
    {
        public Panel Container { get; }
        public event EventHandler Changed;

        public OrderedList(Panel container)
        {
            Container = container;
        }

        public T Add()
        {
            var element = new T();
            element.RemoveClick += QueryFilterControl_OnRemoveClick;
            element.Changed += (sender, args) => Changed?.Invoke(this, EventArgs.Empty);
            element.Up += MoveFilterUp;
            element.Down += MoveFilterDown;

            Container.Children.Add(element.Control);
            Changed?.Invoke(this, EventArgs.Empty);
            return element;
        }

        private void QueryFilterControl_OnRemoveClick(object sender, EventArgs e)
        {
            Container.Children.Remove((UIElement)sender);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void MoveFilterDown(object control, EventArgs eventArgs)
        {
            var element = (UIElement)control;
            var index = Container.Children.IndexOf(element);

            if (index < Container.Children.Count - 1)
                MoveFilter(element, index + 1);
        }

        private void MoveFilterUp(object control, EventArgs eventArgs)
        {
            var element = (UIElement)control;
            var index = Container.Children.IndexOf(element);

            if (index > 0)
                MoveFilter(element, index - 1);
        }

        private void MoveFilter(UIElement control, int newIndex)
        {
            Container.Children.Remove(control);
            Container.Children.Insert(newIndex, control);

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
