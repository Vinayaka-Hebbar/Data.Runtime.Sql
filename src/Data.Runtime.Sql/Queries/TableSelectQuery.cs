using Data.Runtime.Sql.Filters;
using System.Collections.Generic;
using System.Linq;

/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Queries
{
    public class TableSelectQuery<TElement> : TableFilterQuery<TElement> where TElement : class, new()
    {
        public TableSelectQuery(params string[] columns) : base(TableOperationType.Retrieve)
        {
            Columns = columns;
        }

        internal TableSelectQuery() : base(TableOperationType.Retrieve)
        {
            Columns = typeof(TElement).GetDataMemberNames().ToArray();
        }

        public ICollection<string> Columns { get; set; }

        public uint Max { get; private set; }

        public bool HasLimit { get; private set; }

        public TableSelectQuery<TElement> SelectColumns(params string[] columns)
        {
            Columns = columns;
            return this;
        }

        public TableSelectQuery<TElement> Limit(uint max)
        {
            Max = max;
            //Todo somthing to do with query
            HasLimit = true;
            return this;
        }

        public ICollection<Filter> Filters { get; } = new List<Filter>();
    }
}
