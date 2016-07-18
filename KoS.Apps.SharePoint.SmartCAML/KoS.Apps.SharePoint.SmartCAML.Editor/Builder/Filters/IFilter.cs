using System.Xml.Linq;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters
{
    public interface IFilter
    {
        QueryOperator QueryOperator { get; set; }
        FilterOperator? QueryFilter { get; set; }
        XElement ToXml();
    }
}