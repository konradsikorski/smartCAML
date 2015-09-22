namespace KoS.Apps.SharePoint.SmartCAML.Editor.Enums
{
    public enum FilterOperator
    {
        /// <summary>
        /// Searches for a string at the start of a column that holds Text or Note field type values
        /// https://msdn.microsoft.com/en-us/library/office/ms476051.aspx
        /// </summary>
        BeginsWith,
        /// <summary>
        /// Searches for a string anywhere within a column that holds Text or Note field type values.
        /// https://msdn.microsoft.com/en-us/library/office/ms196501.aspx
        /// </summary>
        Contains,
        /// <summary>
        /// Arithmetic operator that means "equal to" and is used within a query.
        /// https://msdn.microsoft.com/en-us/library/office/ms479601.aspx
        /// </summary>
        Eq,
        /// <summary>
        /// Arithmetic operator that means "greater than or equal to." This element can be used within a Where element in a query.
        /// https://msdn.microsoft.com/en-us/library/office/ms416296.aspx
        /// </summary>
        Geq,
        /// <summary>
        /// Arithmetic operator that means "greater than." This element is used similarly to the Eq and Lt elements.
        /// https://msdn.microsoft.com/en-us/library/office/ms458990.aspx
        /// </summary>
        Gt,
        /// <summary>
        /// Specifies whether the value of a list item for the field specified by the FieldRef element is equal to one of the values specified by the Values element.
        ///https://msdn.microsoft.com/en-us/library/office/ff625761.aspx
        /// </summary>
        /// <example>
        /// <In>
        ///     <FieldRef Name = "Field_Name" />
        ///     <Values >    
        ///         <Value Type = "Field_Type" />
        ///     </Values>
        /// </In >
        /// </example>
        In,
        /// <summary>
        /// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is included in the list item for the field that is specified by the FieldRef element.
        /// https://msdn.microsoft.com/en-us/library/office/ff630172.aspx
        /// </summary>
        Includes,
        /// <summary>
        /// Used within a query to return items that are not empty (Null).
        /// https://msdn.microsoft.com/en-us/library/office/ms465807.aspx
        /// </summary>
        IsNotNull,
        /// <summary>
        /// Used within a query to return items that are empty (Null).
        /// https://msdn.microsoft.com/en-us/library/office/ms462425.aspx
        /// </summary>
        IsNull,
        /// <summary>
        /// Arithmetic operator that means "less than or equal to." The Leq element is used in view queries similarly to the Eq and Geq elements
        /// https://msdn.microsoft.com/en-us/library/office/ms431787.aspx
        /// </summary>
        Leq,
        /// <summary>
        /// Arithmetic operator that means "less than" and is used in queries in views. This element is used similarly to the Eq and Gt elements
        /// https://msdn.microsoft.com/en-us/library/office/ms479398.aspx
        /// </summary>
        Lt,
        /// <summary>
        /// Defines a filter based on the type of membership for the user.
        /// https://msdn.microsoft.com/en-us/library/office/aa544234.aspx
        /// </summary>
        Membership,
        /// <summary>
        /// Arithmetic operator that means "not equal to" and is used in queries.
        /// https://msdn.microsoft.com/en-us/library/office/ms452901.aspx
        /// </summary>
        Neq,
        /// <summary>
        /// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is excluded from the list item for the field that is specified by the FieldRef element.
        /// https://msdn.microsoft.com/en-us/library/office/ff630174.aspx
        /// </summary>
        NotIncludes
    }
}