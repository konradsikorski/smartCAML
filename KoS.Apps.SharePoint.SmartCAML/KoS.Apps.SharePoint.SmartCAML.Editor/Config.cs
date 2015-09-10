using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.Editor
{
    static class Config
    {
        public static IEnumerable<string> LastSharePointUrl
        {
            get
            {
                var lastSharePointUrl = ConfigurationManager.AppSettings["LastSharePointUrl"]?.Split(new[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
                return lastSharePointUrl ?? new string[0];
            }
            set { ConfigurationManager.AppSettings["LastSharePointUrl"] = String.Join(";#", value); }
        }
    }
}
