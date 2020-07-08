/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Filters
{
    public class OrderFilter : ConditionFilter
    {
        public OrderFilter(FilterType filterType) : base(filterType)
        {
        }

        public Order Order { get; set; }
    }
}
