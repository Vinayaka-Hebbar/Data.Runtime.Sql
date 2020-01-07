using Data.Runtime.Sql.Queries;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Base Class for Connection TableRef
    /// </summary>
    public class SqlTableRef 
    {
        internal const string SuccessMessage = "Success";
        internal const string FailMessage = "Failed";

        public SqlTableRef(string tableName, IDbClientImpl impl)
        {
            TableName = tableName;
            ClientImpl = impl;
        }

        public IDbClientImpl ClientImpl { get; }

        public string TableName { get; }

        public async Task<IList<TElement>> ExecuteQueryAsync<TElement>(string queryString, Action<TElement> onElementCreated) where TElement : class, new()
        {
            IList<TElement> elements = new List<TElement>();
            IEnumerable<Utils.PropertyDescription> propertieList = typeof(TElement).GetPropertyDescriptions();
            var propertiesDesc = new Utils.PropertyDescriptions(propertieList);
            using (var command = ClientImpl.CreateCommand(queryString))
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    string[] fields = reader.GetFields().ToArray();
                    while (await reader.ReadAsync())
                    {
                        TElement element = new TElement();
                        for (int index = 0; index < fields.Length; index++)
                        {
                            if (reader.IsDBNull(index)) continue;
                            string field = fields[index];
                            propertiesDesc.TrySetValue(field, element, reader.GetValue(index));
                        }
                        onElementCreated(element);
                        elements.Add(element);
                    }
                }
            }
            return elements;
        }

        public async Task<QueryResult> ExecuteNonQueryAsync(string queryString)
        {
            using (DbCommand command = ClientImpl.CreateCommand(queryString))
            {
                try
                {
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return QueryResult.Success(affectedRows);
                }
                catch (Exception ex)
                {
                    return QueryResult.Error(ex);
                }
            }
        }

        /// <summary>
        /// Get the sql last inserted Id
        /// </summary>
        /// <returns>Unsigned int</returns>
        public virtual Task<ulong> GetLastInsertId()
        {
            return Task.FromResult<ulong>(0);
        }

        public async Task<ScalarResult> ExecuteScalarAsync(string queryString)
        {
            using (DbCommand command = ClientImpl.CreateCommand(queryString))
            {
                try
                {
                    var value = await command.ExecuteScalarAsync();
                    return ScalarResult.Success(value);
                }
                catch (Exception ex)
                {
                    return ScalarResult.Error(ex);
                }
            }
        }

        public async Task<IList<TElement>> ExecuteQuerySegmentAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            string queryString = BuildQuery(query);
            return await ExecuteQueryAsync(queryString, query.ElementCreated);
        }

        public async Task<QueryResult> ExecuteNonQueryAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            var queryString = BuildQuery(query);
            return await ExecuteNonQueryAsync(queryString);
        }

        public async Task<ScalarResult> ExecuteScalarAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            var queryString = BuildQuery(query);
            return await ExecuteScalarAsync(queryString);
        }


        public async Task<TElement> ExecuteQueryAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            IList<TElement> result = await ExecuteQuerySegmentAsync(query);
            return result.FirstOrDefault();
        }

        protected virtual object GetValue(object value)
        {
            if (value == null) return "NULL";
            Type type = value.GetType();
            switch (type.FullName)
            {
                case Constants.TypeInt16:
                case Constants.TypeInt32:
                case Constants.TypeBool:
                case Constants.TypeDouble:
                    return value;
                case Constants.TypeStringArray:
                    return $"'{string.Join(Constants.Comma, (string[])value)}'";
                case Constants.TypeDateTime:
                    return $"'{((DateTime)value).ToString(Constants.DateFormat)}'";
                case Constants.TypeString:
                    return $"'{value}'";
                default:
                    if (type.IsEnum)
                        return type.GetField(Constants.EnumValue, Constants.InstantPublic).GetValue(value);
                    return $"'{value}'";
            }
        }

        protected virtual string BuildQuery<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            switch (query.OperationType)
            {
                case TableOperationType.Insert:
                    return BuildInsertQuery(query);
                case TableOperationType.Delete:
                    return BuildDeleteQuery(query);
                case TableOperationType.Retrieve:
                    return BuildRetrieveQuery(query);
                case TableOperationType.Update:
                    return BuildUpdateQuery(query);
            }
            return string.Empty;
        }

        protected virtual string BuildUpdateQuery<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            TableUpdateQuery<TElement> updateQuery = query as TableUpdateQuery<TElement>;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update `{0}` set ", TableName);
            sb.Append(string.Join(Constants.Comma, updateQuery.Items.Select(column => $"`{column.Key}` = {GetValue(column.Value)}")));
            sb.AppendFormat(" where {0}", updateQuery.WhereFilterString);
            return sb.ToString();
        }

        protected virtual string BuildInsertQuery<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            TableInsertQuery<TElement> insertQuery = query as TableInsertQuery<TElement>;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO `{0}`", TableName);
            if (insertQuery.Columns.Count() > 0)
            {
                sb.AppendFormat(" ({0}) ", string.Join(",", insertQuery.Columns.Select(column => $"`{column}`")));
            }
            sb.Append(" VALUES ");
            sb.AppendFormat("({0})", string.Join(",", insertQuery.Values.Select(GetValue)));
            return sb.ToString();
        }



        protected virtual string BuildDeleteQuery<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            TableFilterQuery<TElement> filterQuery = query as TableFilterQuery<TElement>;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from `{0}`", TableName);
            if (!string.IsNullOrEmpty(filterQuery.WhereFilterString))
            {
                sb.AppendFormat(" where {0}", filterQuery.WhereFilterString);
            }
            return sb.ToString();
        }

        protected virtual string BuildRetrieveQuery<TElement>(TableQueryBase<TElement> query) where TElement : class, new()
        {
            TableSelectQuery<TElement> selectQuery = query as TableSelectQuery<TElement>;
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            if (selectQuery.Columns.Count() == 0)
            {
                sb.Append("*");
            }
            else
            {
                sb.Append(string.Join(",", selectQuery.Columns.Select(column => $"`{column}`")));
            }
            sb.Append(" ");
            sb.AppendFormat("from `{0}`", TableName);
            if (!string.IsNullOrEmpty(selectQuery.WhereFilterString))
            {
                sb.Append(" where ");
                sb.Append(selectQuery.WhereFilterString);
            }
            if (selectQuery.HasLimit)
            {
                sb.AppendFormat(" limit {0}", selectQuery.Max);
            }
            return sb.ToString();
        }

    }
}
