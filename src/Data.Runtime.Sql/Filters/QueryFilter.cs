using Data.Runtime.Sql.Queries;

/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class QueryFilter<TElement> : ColumnFilter where TElement : class, new()
    {
        public QueryFilter(string column, TableQueryBase<TElement> query, FilterType filterType) : base(column, filterType)
        {
            Query = query;
        }

        public TableQueryBase<TElement> Query { get; }
    }
}
