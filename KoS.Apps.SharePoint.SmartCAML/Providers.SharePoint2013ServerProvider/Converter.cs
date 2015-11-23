using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace KoS.Apps.SharePoint.SmartCAML.Providers.SharePoint2013ServerProvider
{
    static class Converter
    {
        public static List<Model.ContentType> ToContentTypes(SPContentTypeCollection contentTypes)
        {
            return contentTypes.Cast<SPContentType>().Select(ct => new Model.ContentType
            {
                Id = ct.Id.ToString(),
                Name = ct.Name
            }).ToList();
        }
    }
}
