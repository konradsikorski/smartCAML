using System;
using KoS.Apps.SharePoint.SmartCAML.Editor.Enums;
using KoS.Apps.SharePoint.SmartCAML.Model;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Builder.QueryFilters
{
    class QueryFilterFactory
    {
        public static IQueryFilterController Create(Field filterField, FilterOperator? filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.IsNull:
                case FilterOperator.IsNotNull:
                    return new EmptyQueryFilterController(filterField, filterOperator);
                case FilterOperator.Membership:
                    return new MembershipQueryFilterController(filterField, filterOperator);
                case FilterOperator.In:
                    return new InQueryFilterController(filterField, filterOperator);
            }

            switch (filterField.Type)
            {
                case FieldType.DateTime:
                    return new DateTimeQueryFilterController(filterField, filterOperator);
                case FieldType.Choice:
                    return new ChoiceQueryFilterController(filterField, filterOperator);
                case FieldType.Boolean:
                    return new BooleanQueryFilterController(filterField, filterOperator);
                case FieldType.User:
                    return new UserQueryFilterController(filterField, filterOperator);
                case FieldType.ContentTypeId:
                    return new ContentTypeIdQueryFilterController(filterField, filterOperator);
                case FieldType.Lookup:
                    return new LookupQueryFilterController(filterField, filterOperator);

                default:
                    return new DropdownQueryFilterController(filterField, filterOperator);
            }
        }
    }
}
