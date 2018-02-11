using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using KoS.Apps.SharePoint.SmartCAML.Editor.Properties;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    static class Config
    {
        public static IEnumerable<string> SharePointUrlHistory
        {
            get { return FromStringCollection(Settings.Default.SharePointUrlHistory); }
            set { Settings.Default.SharePointUrlHistory = ToStringCollection(value); }
        }

        public static SharePointProviderType LastSelectedProvider
        {
            get
            {
                var defaultValue = SharePointProviderType.SharePoint2013ClientModel;
#if DEBUG
                defaultValue = SharePointProviderType.Fake;
#endif
                SharePointProviderType provider = Enum.TryParse(Settings.Default.LastSelectedProvider, out provider)
                    ? provider
                    : defaultValue;

#if !DEBUG
               if (provider == SharePointProviderType.Fake) provider = defaultValue; 
#endif
                return provider;
            }
            set { Settings.Default.LastSelectedProvider = ((int) value).ToString(); }
        }

        public static double ConnectWindowWidth
        {
            get { return Settings.Default.ConnectWindowWidth; }
            set { Settings.Default.ConnectWindowWidth = value; }
        }

        public static double WindowWidth
        {
            get { return Settings.Default.WindowWidth; }
            set { Settings.Default.WindowWidth = value; }
        }

        public static double WindowHeight
        {
            get { return Settings.Default.WindowHeight; }
            set { Settings.Default.WindowHeight = value; }
        }

        public static bool WasMaximazed
        {
            get { return Settings.Default.WasMaximazed; }
            set { Settings.Default.WasMaximazed = value; }
        }

        public static string LastUser
        {
            get { return Settings.Default.LastUser; }
            set { Settings.Default.LastUser = value; }
        }

        public static bool UseCurrentUser
        {
            get { return Settings.Default.UseCurrentUser; }
            set { Settings.Default.UseCurrentUser = value; }
        }

        public static IEnumerable<string> UsersHistory
        {
            get { return FromStringCollection(Settings.Default.UsersHistory); }
            set { Settings.Default.UsersHistory = ToStringCollection(value); }
        }

        public static bool DisplayColumnsByTitle
        {
            get { return Settings.Default.DisplayColumnsByTitle; }
            set { Settings.Default.DisplayColumnsByTitle = value; }
        }

        public static string UserId
        {
            get
            {
                if (Guid.Empty.Equals(Settings.Default.UserId))
                {
                    Settings.Default.UserId = Guid.NewGuid();
                    Settings.Default.Save();
                }

                return Settings.Default.UserId.ToString();
            }
        }

        public static TimeSpan TotalRunTime
        {
            get { return Settings.Default.TotalRunTime; }
            set { Settings.Default.TotalRunTime = value; }
        }

        public static string LastVersion
        {
            get { return Settings.Default.LastVersion; }
            set { Settings.Default.LastVersion = value; }
        }

        public static bool InstallationCompleted
        {
            get { return Settings.Default.InstallationCompleted; }
            set { Settings.Default.InstallationCompleted = value; }
        }

        public static int? PageSize
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.Default.PageSize)) return null;

                int pageSize;
                return int.TryParse(Settings.Default.PageSize, out pageSize)
                    ? (int?)pageSize
                    : null;
            }
            set
            {
                if(value == null || value > 1)
                    Settings.Default.PageSize = value?.ToString();
            }
        }

        public static string ServiceAddress =>
#if DEBUG
            "http://localhost:7870/api/";
#else
            "https://sikorski-workshop.azurewebsites.net/api/";
#endif

        public static void Save()
        {
            Settings.Default.Save();
        }

        private static StringCollection ToStringCollection(IEnumerable<string> collection)
        {
            var newValue = new StringCollection();
            newValue.AddRange(collection.ToArray());
            return newValue;
        }

        private static IEnumerable<string> FromStringCollection(StringCollection collection)
        {
            return collection == null
                ? Enumerable.Empty<string>()
                : collection.Cast<string>();
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
