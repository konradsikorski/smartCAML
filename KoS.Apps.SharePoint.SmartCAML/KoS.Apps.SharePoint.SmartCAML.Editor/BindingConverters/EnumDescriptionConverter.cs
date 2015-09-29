using System;
using System.Globalization;
using System.Windows.Data;
using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.BindingConverters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return String.Empty;

            return RefactorUtil.GetDescription(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
