using Data.Runtime.Sql.Filters;
using Data.Runtime.Sql.Queries;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Sql Table Query
    /// </summary>
    public static class TableQuery
    {
        public static TableSelectQuery<TElement> Select<TElement>(params string[] columns) where TElement : class, new()
        {
            if (columns.Length == 0)
            {
                return new TableSelectQuery<TElement>();
            }
            return new TableSelectQuery<TElement>(columns);
        }

        public static TableFilterQuery<object> Delete()
        {
            return new TableFilterQuery<object>(TableOperationType.Delete);
        }

        public static TableUpdateQuery<TElement> Update<TElement>(TElement element) where TElement : class, new()
        {
            return new TableUpdateQuery<TElement>(element);
        }

        public static TableInsertQuery<TElement> Insert<TElement>(TElement element) where TElement : class, new()
        {
            return new TableInsertQuery<TElement>(element);
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

        public static TableSelectQuery<TElement> In<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase<TOther> otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.In));
            return query;
        }

        public static TableSelectQuery<TElement> In<TElement, TOther>(this TableSelectQuery<TElement> query, string column, params TOther[] others) where TElement : class, new()
        {
            query.Filters.Add(new InItemsFilter<TOther>(others, column));
            return query;
        }

        public static TableSelectQuery<TElement> Any<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase<TOther> otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.Any));
            return query;
        }

        public static TableSelectQuery<TElement> All<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase<TOther> otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.All));
            return query;
        }

        public static TableSelectQuery<TElement> Exists<TElement, TOther>(this TableSelectQuery<TElement> query, string column, TableQueryBase<TOther> otherQuery) where TElement : class, new() where TOther : class, new()
        {
            query.Filters.Add(new QueryFilter<TOther>(column, otherQuery, FilterType.Exists));
            return query;
        }
    }
}
