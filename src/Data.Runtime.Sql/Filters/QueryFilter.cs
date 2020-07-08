using SqlDb.Data.Queries;

/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Filters
{
    public class QueryFilter<TElement> : ColumnFilter where TElement : class, new()
    {
        public QueryFilter(string column, TableQueryBase query, FilterType filterType) : base(column, filterType)
        {
            Query = query;
        }

        public TableQueryBase Query { get; }
    }
}
