using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Extensions
{
    public static class ComboBoEnumxExtension
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

        private static string GetDescription<T>(this T enumValue) where T : struct
        {
            var type = enumValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentException("The value must be of Enum type", nameof(enumValue));

            //Tries to find a DescriptionAttribute for a potential friendly name for the enum
            var enumName = enumValue.ToString();
            var memberInfo = type.GetMember(enumName);
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumName;
        }
    }
}