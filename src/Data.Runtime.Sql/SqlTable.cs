using SqlDb.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDb.Data
{
    public class SqlTable 
    {
        public SqlTable(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }

        public virtual Task<QueryResult> ExecuteNonQueryAsync(QueryString queryString)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IList<TElement>> ExecuteQueryAsync<TElement>(QueryString queryString, Action<TElement> onElementCreated) where TElement : new()
        {
            throw new NotImplementedException();
        }

        public virtual Task<ScalarResult> ExecuteScalarAsync(QueryString queryString)
        {
            throw new NotImplementedException();
        }

        public virtual object GetValue(object value)
        {
            if (value == null) return "NULL";
            Type type = value.GetType();
            if (type.IsPrimitive)
                return value;
            if (type.IsEnum)
                return type.GetField(Constants.EnumValue, Constants.InstantPublic).GetValue(value);
            switch (type.FullName)
            {
                case Constants.TypeDateTime:
                    return $"'{((DateTime)value).ToString(Constants.DateFormat)}'";
                case Constants.TypeString:
                    return $"'{value}'";
                default:
                    return $"'{value}'";
            }
        }

        protected virtual string BuildQuery<TElement>(TableQueryBase query) where TElement : new()
        {
            switch (query.OperationType)
            {
                case TableOperationType.Insert:
                    return BuildInsertQuery((TableInsertQuery<TElement>)query);
                case TableOperationType.Delete:
                    return BuildDeleteQuery((TableDeleteQuery<TElement>)query);
                case TableOperationType.Retrieve:
                    return BuildRetrieveQuery((TableSelectQuery<TElement>)query);
                case TableOperationType.Update:
                    return BuildUpdateQuery((TableUpdateQuery<TElement>)query);
                case TableOperationType.Create:
                    return BuildCreateQuery((TableCreateQuery<TElement>)query);
            }
            return string.Empty;
        }

        public virtual string BuildCreateQuery<TElement>(TableCreateQuery<TElement> tableCreateQuery)
        {
            sb.Append(string.Join(Constants.Comma, query.Items.Select(column => $"`{column.Key}` = {GetValue(column.Value)}")));
            if (!string.IsNullOrEmpty(query.WhereFilter))
            {
                sb.Append($" where {query.WhereFilter}");
            }
            else
            {
                var primary = ReflectionHelper.GetPrimaryKey(query.Element);
                sb.AppendFormat($" where `{primary.Key}` = {GetValue(primary.Value)}");
            }
            return sb.ToString();
        }

        public virtual string BuildUpdateQuery<TElement>(TableUpdateQuery<TElement> query)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"update `{TableName}` set ");
            sb.Append(string.Join(Constants.Comma, query.Items.Select(column => $"`{column.Key}` = {GetValue(column.Value)}")));
            if (!string.IsNullOrEmpty(query.WhereFilter))
            {
                sb.Append($" where {query.WhereFilter}");
            }
            else
            {
                var primary = ReflectionHelper.GetPrimaryKey(query.Element);
                sb.AppendFormat($" where `{primary.Key}` = {GetValue(primary.Value)}");
            }
            return sb.ToString();
        }

        public virtual string BuildInsertQuery<TElement>(TableInsertQuery<TElement> query)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO `{0}`", TableName);
            if (query.Columns.Count > 0)
            {
                sb.AppendFormat(" ({0}) ", string.Join(",", query.Columns.Select(column => $"`{column}`")));
            }
            sb.Append(" VALUES ");
            sb.AppendFormat("({0})", string.Join(",", query.Values.Select(GetValue)));
            return sb.ToString();
        }

        public virtual string BuildDeleteQuery<TElement>(TableDeleteQuery<TElement> query)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from `{0}`", TableName);
            if (!string.IsNullOrEmpty(query.WhereFilter))
            {
                sb.AppendFormat(" where {0}", query.WhereFilter);
            }
            else
            {
                var primary = ReflectionHelper.GetPrimaryKey(query.Element);
                sb.AppendFormat(" where `{0}` = {1}", primary.Key, GetValue(primary.Value));
            }
            return sb.ToString();
        }

        public virtual string BuildRetrieveQuery<TElement>(TableSelectQuery<TElement> query) where TElement : new()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            if (query.Columns.Count == 0)
            {
                sb.Append("*");
            }
            else
            {
                sb.Append(string.Join(",", query.Columns.Select(column => $"`{column}`")));
            }
            sb.AppendFormat($" from `{TableName}`");
            if (!string.IsNullOrEmpty(query.WhereFilter))
            {
                sb.Append($" where {query.WhereFilter}");
            }
            if (query.HasLimit)
            {
                sb.AppendFormat($" limit {query.Max}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// returns the primary key column name of the element <typeparamref name="TElement"/>
        /// </summary>
        public static string GetPrimaryKey<TElement>()
        {
            return ReflectionHelper.GetPrimaryKey(typeof(TElement));
        }
    }
}
