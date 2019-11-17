/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class BetweenFilter<TValue> : ColumnFilter
    {
        public BetweenFilter(string column, TValue value1, TValue value2) : base(column, FilterType.Between)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public TValue Value1 { get; }

        public TValue Value2 { get; }
    }
}
