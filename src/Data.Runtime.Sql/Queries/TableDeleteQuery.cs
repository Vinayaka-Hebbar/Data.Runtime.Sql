using System.Threading.Tasks;

namespace SqlDb.Data.Queries
{
    public class TableDeleteQuery<TElement> : TableFilterQuery
    {
        protected internal TableDeleteQuery(TElement element, SqlTable table) : base(table)
        {
            Element = element;
        }

        public TElement Element { get; }

        public override TableOperationType OperationType => TableOperationType.Delete;

        public override Task<QueryResult> ExecuteAsync()
        {
            return Table.ExecuteNonQueryAsync(new QueryString(Table.BuildDeleteQuery(this), TableOperationType.Delete));
        }

        public override string GetQueryString()
        {
            return Table.BuildDeleteQuery(this);
        }

        public TableDeleteQuery<TElement> Where(string filter, params object[] filterParams)
        {
            WhereFilter = string.Format(filter, filterParams);
            return this;
        }

        public TableDeleteQuery<TElement> Where(System.Linq.Expressions.Expression<System.Func<TElement, bool>> filter)
        {
            WhereFilter = new Utils.ExpressionVisitor(Table).Get(filter.Body);
            return this;
        }
    }
}
