/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Filters
{
    public class ConditionFilter : Filter
    {
        public ConditionFilter(FilterType filterType) : base(filterType)
        {
        }

        public string Condition { get; set; }
    }
}
