/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class OrderFilter : ConditionFilter
    {
        public OrderFilter(FilterType filterType) : base(filterType)
        {
        }

        public Order Order { get; set; }
    }
}
