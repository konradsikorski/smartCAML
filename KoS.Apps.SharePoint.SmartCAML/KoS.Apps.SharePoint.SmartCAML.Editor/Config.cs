using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    static class Config
    {
        public static IEnumerable<string> LastSharePointUrl
        {
            get
            {
                var lastSharePointUrl = ConfigurationManager.AppSettings[nameof(LastSharePointUrl)]?.Split(new[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
                return lastSharePointUrl ?? new string[0];
            }
            set { ConfigurationManager.AppSettings[nameof(LastSharePointUrl)] = String.Join(";#", value); }
        }

        public static SharePointProviderType LastSelectedProvider
        {
            get
            {
                var defaultValue = SharePointProviderType.SharePoint2013ClientModel;
                var value = ConfigurationManager.AppSettings[nameof(LastSelectedProvider)] ?? ((int)defaultValue).ToString();

                SharePointProviderType provider;
                return Enum.TryParse(value, out provider)
                    ? provider
                    : defaultValue;
            }
            set { ConfigurationManager.AppSettings[nameof(LastSelectedProvider)] = ((int)value).ToString(); }
        }

        public static string LastUser
        {
            get { return GetConfig(); }
            set { SetConfig(value); }
        }

        //public static IEnumerable<string> LastUsers
        //{
        //    get
        //    {
        //        var defaultValue = SharePointProviderType.SharePoint2013ClientModel;
        //        var value = ConfigurationManager.AppSettings[nameof(LastSelectedProvider)] ?? ((int)defaultValue).ToString();

        //        SharePointProviderType provider;
        //        return Enum.TryParse(value, out provider)
        //            ? provider
        //            : defaultValue;
        //    }
        //    set { ConfigurationManager.AppSettings[nameof(LastSelectedProvider)] = ((int)value).ToString(); }
        //}


        private static string GetConfig([CallerMemberName] string name = "")
        {
            return ConfigurationManager.AppSettings[name];
        }

        private static void SetConfig(object value, [CallerMemberName] string name = "")
        {
            ConfigurationManager.AppSettings[name] = value?.ToString();
        }
    }
}
