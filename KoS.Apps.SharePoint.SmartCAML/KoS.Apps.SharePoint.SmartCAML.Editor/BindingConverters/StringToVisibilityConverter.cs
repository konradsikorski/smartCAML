using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string.IsNullOrEmpty((string)value))
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}