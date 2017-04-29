using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using KoS.Apps.SharePoint.SmartCAML.Editor.Events;
using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Behaviors
{
    class PasswordBoxWatermarkBehavior : System.Windows.Interactivity.Behavior<PasswordBox>
    {
        private TextBlockAdorner adorner;
        private WeakPropertyChangeNotifier notifier;
        private WeakPropertyChangeNotifier notifierVisibility;

        #region DependencyProperty's

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(PasswordBoxWatermarkBehavior));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.RegisterAttached("LabelStyle", typeof(Style), typeof(PasswordBoxWatermarkBehavior));

        public Style LabelStyle
        {
            get { return (Style)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += this.AssociatedObjectLoaded;
            this.AssociatedObject.PasswordChanged += AssociatedObjectPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= this.AssociatedObjectLoaded;
            this.AssociatedObject.PasswordChanged -= this.AssociatedObjectPasswordChanged;

            this.notifier = null;
        }

        private void AssociatedObjectPasswordChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateAdorner();
        }

        private void AssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.adorner = new TextBlockAdorner(this.AssociatedObject, this.Label, this.LabelStyle);

            this.UpdateAdorner();

            //AddValueChanged for IsFocused in a weak manner
            this.notifier = new WeakPropertyChangeNotifier(this.AssociatedObject, UIElement.IsFocusedProperty);
            this.notifier.ValueChanged += new EventHandler(this.UpdateAdorner);
             notifierVisibility = new WeakPropertyChangeNotifier(this.AssociatedObject, UIElement.VisibilityProperty);
            this.notifierVisibility.ValueChanged += new EventHandler(this.UpdateAdorner);

        }

        private void UpdateAdorner(object sender, EventArgs e)
        {
            this.UpdateAdorner();
        }


        private void UpdateAdorner()
        {
            if (!String.IsNullOrEmpty(this.AssociatedObject.Password) || this.AssociatedObject.IsFocused || this.AssociatedObject.Visibility != Visibility.Visible)
            {
                // Hide the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryRemoveAdorners<TextBlockAdorner>();
            }
            else
            {
                // Show the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryAddAdorner<TextBlockAdorner>(adorner);
            }
        }


    }

	public class TextBlockAdorner : Adorner
    {
        private readonly UIElement adornedElement;
        private readonly TextBlock m_TextBlock;

        public TextBlockAdorner(UIElement adornedElement, string label, Style labelStyle)
            : base(adornedElement)
        {
            this.adornedElement = adornedElement;
            this.m_TextBlock = new TextBlock
            {
                Style = labelStyle,
                Text = label
            };
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.m_TextBlock.Measure(constraint);
            if (double.IsInfinity(constraint.Height))
            {
                constraint.Height = this.adornedElement.DesiredSize.Height;
            }
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.m_TextBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.m_TextBlock;
        }
    }

}
