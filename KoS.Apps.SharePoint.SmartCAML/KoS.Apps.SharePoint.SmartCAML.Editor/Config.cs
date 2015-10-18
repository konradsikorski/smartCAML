using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public static double ConnectWindowWidth
        {
            get { return GetConfig(360).ToDouble(); }
            set { SetConfig(value); }
        }

        public static double WindowWidth
        {
            get { return GetConfig(700).ToDouble(); }
            set { SetConfig(value); }
        }

        public static double WindowHeight
        {
            get { return GetConfig(500).ToDouble(); }
            set { SetConfig(value); }
        }

        public static bool WasMaximazed
        {
            get { return GetConfig().ToBool(); }
            set { SetConfig(value);}
        }

        public static string LastUser
        {
            get { return GetConfig(); }
            set { SetConfig(value); }
        }

        public static bool UseCurrentUser
        {
            get { return GetConfig(true).ToBool(true); }
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
            SetConfig(String.Join(";#", collection), name);
        }

        private static string GetConfig(object defaultValue = null, [CallerMemberName] string name = "")
        {
            return ConfigurationManager.AppSettings[name] ?? defaultValue?.ToString();
        }

        private static void SetConfig<T>(T value, [CallerMemberName] string name = "")
        {
            var newValue = value?.ToString();
            ConfigurationManager.AppSettings[name] = newValue;

            //---
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            if (!config.AppSettings.Settings.AllKeys.Contains(name))
                config.AppSettings.Settings.Add(name, newValue);
            else
                config.AppSettings.Settings[name].Value = newValue;

            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
