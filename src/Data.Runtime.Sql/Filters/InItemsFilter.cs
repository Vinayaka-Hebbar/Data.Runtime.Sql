using System.Collections.Generic;

/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Filters
{
    public class InItemsFilter<TElement> : ColumnFilter
    {
        public InItemsFilter(ICollection<TElement> items, string column) : base(column, FilterType.In)
        {
            Items = items;
        }

        public ICollection<TElement> Items { get; }
    }
}
