using System;
using System.ComponentModel;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    class RefactorUtil
    {
        public static string GetDescription(object enumValue)
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
