/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class Filter
    {
        public Filter(FilterType filterType)
        {
            FilterType = filterType;
        }

        public FilterType FilterType { get; }
    }

    public enum FilterType
    {
        Having,
        GroupBy,
        OrderBy,
        Between,
        In,
        NotIn,
        Any,
        All,
        Exists

    }

    public enum Order
    {
        Asc = 0,
        Desc = 1
    }
}
