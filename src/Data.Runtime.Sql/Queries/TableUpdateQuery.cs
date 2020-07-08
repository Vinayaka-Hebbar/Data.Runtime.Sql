using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Queries
{
    public class TableUpdateQuery<TElement> : TableFilterQuery
    {
        protected internal TableUpdateQuery(TElement element, SqlTable table) : base(table)
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

        public override TableOperationType OperationType => TableOperationType.Update;

        public override Task<QueryResult> ExecuteAsync()
        {
            return Table.ExecuteNonQueryAsync(new QueryString(Table.BuildUpdateQuery(this), TableOperationType.Update));
        }

        public TableUpdateQuery<TElement> Where(string filter, params object[] filterParams)
        {
            WhereFilter = string.Format(filter, filterParams);
            return this;
        }

        public TableUpdateQuery<TElement> Where(System.Linq.Expressions.Expression<System.Func<TElement, bool>> filter)
        {
            WhereFilter = new Utils.ExpressionVisitor(Table).Get(filter.Body);
            return this;
        }

        public override string GetQueryString()
        {
            return Table.BuildUpdateQuery(this);
        }
    }
}
