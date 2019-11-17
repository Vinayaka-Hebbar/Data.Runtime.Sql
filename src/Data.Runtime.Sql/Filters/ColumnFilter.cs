/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public abstract class ColumnFilter : Filter
    {
        public ColumnFilter(string column, FilterType filterType) : base(filterType)
        {
            Column = column;
        }

        public string Column { get; }
    }
}
