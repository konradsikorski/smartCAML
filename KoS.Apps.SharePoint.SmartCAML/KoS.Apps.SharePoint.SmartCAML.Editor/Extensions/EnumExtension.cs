using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            return RefactorUtil.GetDescription(enumValue);
        }

        public static List<KeyValuePair<T, string>> ToDictionary<T>(this T enumValue) where T : struct
        {
            return ToDictionary<T>();
        }

        public static List<KeyValuePair<T, string>> ToDictionary<T>() where T : struct
        {
            return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(p => new KeyValuePair<T, string>(p, p.GetDescription()))
                   .ToList();
        }
    }
}
