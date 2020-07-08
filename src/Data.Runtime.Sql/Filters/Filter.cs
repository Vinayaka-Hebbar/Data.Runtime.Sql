/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Filters
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
