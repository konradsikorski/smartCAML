using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.ServiceProxy
{
    class TypeHelper
    {
        public static Dictionary<string, object> ObjectToDictionary(object value)
        {
            var dictionary = new Dictionary<string, object>();
            if (value != null)
            {
                foreach (var property in PropertyHelper.GetProperties(value))
                    dictionary.Add(property.Name, property.GetValue(value));
            }

            return dictionary;
        }
    }
}
