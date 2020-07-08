using SqlDb.Data.Filters;
using SqlDb.Data.Queries;

namespace SqlDb.Data
{
    /// <summary>
    /// Sql Table Query
    /// </summary>
    public static class TableQueryExtensions
    {
        public static TableSelectQuery<TElement> CreateSelect<TElement>(this SqlTable table, params string[] columns) where TElement : new()
        {
            if (columns.Length == 0)
            {
                return new TableSelectQuery<TElement>(table);
            }
            return new TableSelectQuery<TElement>(columns, table);
        }

        public static TableDeleteQuery<TElement> CreateDelete<TElement>(this SqlTable table, TElement element = default(TElement))
        {
            return new TableDeleteQuery<TElement>(element, table);
        }

        public static TableUpdateQuery<TElement> CreateUpdate<TElement>(this SqlTable table, TElement element)
        {
            return new TableUpdateQuery<TElement>(element, table);
        }

        public static TableInsertQuery<TElement> CreateInsert<TElement>(this SqlTable table, TElement element)
        {
            return new TableInsertQuery<TElement>(element, table);
        }

        public static TableSelectQuery<TElement> OrderBy<TElement>(this TableSelectQuery<TElement> query, string condition) where TElement : class, new()
        {
            query.Filters.Add(new OrderFilter(FilterType.OrderBy) { Condition = condition });
            return query;
        }

        public static TableSelectQuery<TElement> OrderBy<TElement>(this TableSelectQuery<TElement> query, string condition, Order order) where TElement : class, new()
        {
            query.Filters.Add(new OrderFilter(FilterType.OrderBy) { Condition = condition, Order = order });
            return query;
        }

        public static TableSelectQuery<TElement> Having<TElement>(this TableSelectQuery<TElement> query, string condition) where TElement : class, new()
        {
            query.Filters.Add(new ConditionFilter(FilterType.Having) { Condition = condition });
            return query;
        }

        public static TableSelectQuery<TElement> GroupBy<TElement>(this TableSelectQuery<TElement> query, string condition) where TElement : class, new()
        {
            query.Filters.Add(new ConditionFilter(FilterType.GroupBy) { Condition = condition });
            return query;
        }

        public static TableSelectQuery<TElement> Select<TElement>(this TableSelectQuery<TElement> query, string column) where TElement : class, new()
        {
            query.Columns.Add(column);
            return query;
        }

        public static TableSelectQuery<TElement> Between<TElement, TValue>(this TableSelectQuery<TElement> query, string column, TValue value1, TValue value2) where TElement : class, new()
        {
            query.Filters.Add(new BetweenFilter<TValue>(column, value1, value2));
            return query;
        }

        public static TableSelectQuery<TElement> In<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.In));
            return query;
        }

        public static TableSelectQuery<TElement> In<TElement, TOther>(this TableSelectQuery<TElement> query, string column, params TOther[] others) where TElement : class, new()
        {
            query.Filters.Add(new InItemsFilter<TOther>(others, column));
            return query;
        }

        public static TableSelectQuery<TElement> Any<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.Any));
            return query;
        }

        public static TableSelectQuery<TElement> All<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.All));
            return query;
        }

        public static TableSelectQuery<TElement> Exists<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.Exists));
            return query;
        }
    }
}
