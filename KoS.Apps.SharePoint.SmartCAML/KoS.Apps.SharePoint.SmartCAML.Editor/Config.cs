using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    static class Config
    {
        public static IEnumerable<string> SharePointUrlHistory
        {
            get { return GetCollection(); }
            set { SetCollection(value); }
        }

        public static SharePointProviderType LastSelectedProvider
        {
            get
            {
                var defaultValue = SharePointProviderType.SharePoint2013ClientModel;
#if DEBUG
                defaultValue = SharePointProviderType.Fake;
#endif
                var value = GetConfig((int)defaultValue);

                SharePointProviderType provider;
                return Enum.TryParse(value, out provider)
                    ? provider
                    : defaultValue;
            }
            set { SetConfig((int)value); }
        }

        public static string LastUser
        {
            get { return GetConfig(); }
            set { SetConfig(value); }
        }

        public static bool UseCurrentUser
        {
            get { return bool.Parse(GetConfig(true)); }
            set { SetConfig(value);}
        }

        public static IEnumerable<string> UsersHistory
        {
            get { return GetCollection(); }
            set { SetCollection(value); }
        }

        private static IEnumerable<string> GetCollection([CallerMemberName] string name = "")
        {
            var value = ConfigurationManager.AppSettings[name]?.Split(new[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
            return value ?? new string[0];
        }

        private static void SetCollection<T>(IEnumerable<T> collection, [CallerMemberName] string name = "")
        {
            ConfigurationManager.AppSettings[name] = String.Join(";#", collection);
        }

        private static string GetConfig(object defaultValue = null, [CallerMemberName] string name = "")
        {
            return ConfigurationManager.AppSettings[name] ?? defaultValue?.ToString();
        }

        private static void SetConfig<T>(T value, [CallerMemberName] string name = "")
        {
            ConfigurationManager.AppSettings[name] = value?.ToString();
        }
    }
}
