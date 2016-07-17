using System;
using System.Windows.Controls;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    interface IQueryFilterController : IDisposable
    {
        Field Field { get; }
        FilterOperator? FilterOperator { get; }
        void Initialize(Panel parent, string oldValue);
        string GetValue();
        IFilter GetFilter(QueryOperator queryOperator);

        event EventHandler ValueChanged;
    }
}
