using SqlDb.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Queries
{
    public class TableSelectQuery<TElement> : TableFilterQuery where TElement : new()
    {
        public TableSelectQuery(string[] columns, SqlTable table) : base(table)
        {
            Columns = columns;
        }

        internal TableSelectQuery(SqlTable table) : base(table)
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

        public TableSelectQuery<TElement> AppendColumn(string columnName)
        {
            var newColumns = new string[Columns.Count + 1];
            Columns.CopyTo(newColumns, 0);
            newColumns[Columns.Count] = columnName;
            Columns = newColumns;
            return this;
        }

        public TableSelectQuery<TElement> Limit(uint max)
        {
            Max = max;
            //Todo somthing to do with query
            HasLimit = true;
            return this;
        }

        internal static readonly Action<TElement> nothing = Nothing;

        static void Nothing(TElement e) { }

        public Action<TElement> ElementCreated { get; private set; } = nothing;

        public TableSelectQuery<TElement> OnElementCreated(Action<TElement> onRefElement)
        {
            ElementCreated = onRefElement;
            return this;
        }

        public ICollection<Filter> Filters { get; } = new List<Filter>();

        public override TableOperationType OperationType => TableOperationType.Retrieve;

        public Task<IList<TElement>> ExecuteSegmentAsync()
        {
            return Table.ExecuteQueryAsync(new QueryString(Table.BuildRetrieveQuery(this), TableOperationType.Retrieve), ElementCreated);
        }

        public override Task<QueryResult> ExecuteAsync()
        {
            return Table.ExecuteNonQueryAsync(new QueryString(Table.BuildRetrieveQuery(this), TableOperationType.Retrieve));
        }

        public TableSelectQuery<TElement> Where(string filter, params object[] filterParams)
        {
            WhereFilter = string.Format(filter, filterParams);
            return this;
        }

        public TableSelectQuery<TElement> Where(System.Linq.Expressions.Expression<Func<TElement, bool>> filter)
        {
            WhereFilter = new Utils.ExpressionVisitor(Table).Get(filter.Body);
            return this;
        }

        public override string GetQueryString()
        {
            return Table.BuildRetrieveQuery(this);
        }
    }
}
