using System.Windows;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.AttachedProperties
{
    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPassword = DependencyProperty.RegisterAttached(
                "BoundPassword", 
                typeof (string), 
                typeof (PasswordBoxAssistant),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged)
            );

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
                "BindPassword", 
                typeof (bool), 
                typeof (PasswordBoxAssistant),
                new PropertyMetadata(false, OnBindPasswordChanged)
            );

        public static readonly DependencyProperty BindPlaceholder = DependencyProperty.RegisterAttached(
                "BindPlaceholder",
                typeof(UIElement),
                typeof(PasswordBoxAssistant),
                new PropertyMetadata(null, OnBindPlaceholderChanged)
            );

        private static readonly DependencyProperty UpdatingPassword = DependencyProperty.RegisterAttached(
                "UpdatingPassword", 
                typeof (bool), 
                typeof (PasswordBoxAssistant),
                new PropertyMetadata(false)
            );

        public static string GetBindPlaceholder(DependencyObject dp)
        {
            return (string)dp.GetValue(BindPlaceholder);
        }

        public static void SetBindPlaceholder(DependencyObject dp, string value)
        {
            dp.SetValue(BindPlaceholder, value);
        }

        private static void OnBindPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (PasswordBox)d;

            if (e.OldValue as UIElement != null) {
                box.PasswordChanged -= ShowHidePlaceholderOnPasswordChanged;
                box.IsVisibleChanged -= Box_IsVisibleChanged;
            }

            if (e.NewValue as UIElement != null) {
                box.PasswordChanged += ShowHidePlaceholderOnPasswordChanged;
                box.IsVisibleChanged += Box_IsVisibleChanged;
            }
        }

        private static void Box_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var box = (PasswordBox)sender;
            ShowHidePlaceholder(box);
        }

        private static void ShowHidePlaceholderOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;
            ShowHidePlaceholder(box);
        }

        private static void ShowHidePlaceholder(PasswordBox box)
        {
            var placeholder = GetBoundPlaceholder(box);

            if (box.Visibility == Visibility.Visible && box.Password == string.Empty)
                placeholder.Visibility = Visibility.Visible;
            else
                placeholder.Visibility = Visibility.Collapsed;
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            var box = (PasswordBox)d;
            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string) e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event

            var box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool) (e.OldValue);
            bool needToBind = (bool) (e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox) sender;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);
            // push the new password into the BoundPassword property
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool) dp.GetValue(BindPassword);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string) dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool) dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
}
