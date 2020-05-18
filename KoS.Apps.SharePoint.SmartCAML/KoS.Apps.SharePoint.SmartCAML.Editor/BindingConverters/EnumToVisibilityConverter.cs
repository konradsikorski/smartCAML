using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((SharePointProviderType)value) == SharePointProviderType.SharePointOnlineWithMFA)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}