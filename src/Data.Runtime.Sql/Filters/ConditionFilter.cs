/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class ConditionFilter : Filter
    {
        public ConditionFilter(FilterType filterType) : base(filterType)
        {
        }

        public string Condition { get; set; }
    }
}
