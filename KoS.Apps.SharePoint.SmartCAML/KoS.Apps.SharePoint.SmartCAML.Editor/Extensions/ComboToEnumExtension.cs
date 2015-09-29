using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Extensions
{
    public static class ComboToEnumExtension
    {
        public static void BindToEnum<T>( this ComboBox @this) where T : struct
        {
            var source = Enum.GetValues(typeof (T))
                .Cast<T>()
                .Select(p => new KeyValuePair<T, string>(p, p.GetDescription()))
                .ToList();

            @this.ItemsSource = source;
            @this.DisplayMemberPath = "Value";
            @this.SelectedValuePath = "Key";
        }

        public static T? SelectedEnum<T>(this ComboBox @this) where T: struct
        {
            return @this.SelectedItem != null
                ? (T?)@this.SelectedValue
                : null;
        }
    }
}