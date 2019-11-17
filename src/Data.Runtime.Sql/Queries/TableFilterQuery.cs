/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Queries
{
    public class TableFilterQuery<TElement> : TableQueryBase<TElement> where TElement : class, new()
    {
        protected internal TableFilterQuery(TableOperationType operationType) : base(operationType)
        {

        }

        public string WhereFilterString { get; set; }

        public TableQueryBase<TElement> Where(string filter)
        {
            WhereFilterString = filter;
            return this;
        }

        public TableQueryBase<TElement> Where(string filter, params object[] @params)
        {
            WhereFilterString = string.Format(filter, @params);
            return this;
        }
    }
}
