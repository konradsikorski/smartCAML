using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            return RefactorUtil.GetDescription(enumValue);
        }
    }
}
