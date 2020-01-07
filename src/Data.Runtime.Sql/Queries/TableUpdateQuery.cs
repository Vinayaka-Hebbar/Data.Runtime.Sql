using System.Collections.Generic;
using System.Linq;

/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Queries
{
    public class TableUpdateQuery<TElement> : TableFilterQuery<TElement> where TElement : class, new()
    {
        protected internal TableUpdateQuery(TElement element) : base(TableOperationType.Update)
        {
            Element = element;
            Items = ReflectionHelper.GetColumns(element)
                .ToDictionary(item => item.Key, item => item.Value);
        }

        public TElement Element { get; }

        public IDictionary<string, object> Items { get; }

        public TableUpdateQuery<TElement> Set(string key, object value)
        {
            Items[key] = value;
            return this;
        }
    }
}
