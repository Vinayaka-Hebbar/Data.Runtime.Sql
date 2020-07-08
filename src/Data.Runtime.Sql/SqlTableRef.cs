using SqlDb.Data.Common;
using SqlDb.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SqlDb.Data
{
    //todo generic argument
    /// <summary>
    /// Base Class for Connection TableRef
    /// </summary>
    public class SqlTableRef : SqlTable
    {
        public SqlTableRef(string tableName, ISqlClient client) : base(tableName)
        {
            Client = client;
        }

        public ISqlClient Client { get; }

        public override async Task<IList<TElement>> ExecuteQueryAsync<TElement>(QueryString queryString, Action<TElement> onElementCreated)
        {
            IList<TElement> elements = new List<TElement>();
            var propertiesDesc = new Utils.PropertyDescriptions(typeof(TElement).GetPropertyDescriptions());
            using (var command = Client.CreateCommand(queryString.Value))
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    string[] fields = reader.GetFields().ToArray();
                    while (await reader.ReadAsync())
                    {
                        var element = new TElement();
                        for (int index = 0; index < fields.Length; index++)
                        {
                            if (reader.IsDBNull(index)) continue;
                            propertiesDesc.TrySetValue(element, fields[index], reader.GetValue(index));
                        }
                        onElementCreated(element);
                        elements.Add(element);
                    }
                }
            }
            return elements;
        }

        public override async Task<QueryResult> ExecuteNonQueryAsync(QueryString queryString)
        {
            using (DbCommand command = Client.CreateCommand(queryString.Value))
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
        public virtual Task<ulong> GetLastInsertId<TElement>()
        {
            return Task.FromResult<ulong>(0);
        }

        public override async Task<ScalarResult> ExecuteScalarAsync(QueryString queryString)
        {
            using (DbCommand command = Client.CreateCommand(queryString.Value))
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

        public async Task<IList<TElement>> ExecuteQuerySegmentAsync<TElement>(TableQueryBase query) where TElement : new()
        {
            string queryString = BuildQuery<TElement>(query);
            return await ExecuteQueryAsync(new QueryString(queryString, query.OperationType), TableSelectQuery<TElement>.nothing);
        }

        public async Task<QueryResult> ExecuteNonQueryAsync<TElement>(TableQueryBase query) where TElement : new()
        {
            var queryString = BuildQuery<TElement>(query);
            return await ExecuteNonQueryAsync(new QueryString(queryString, query.OperationType));
        }

        public async Task<ScalarResult> ExecuteScalarAsync<TElement>(TableQueryBase query) where TElement : new()
        {
            var queryString = BuildQuery<TElement>(query);
            return await ExecuteScalarAsync(new QueryString(queryString, query.OperationType));
        }


        public async Task<TElement> ExecuteQueryAsync<TElement>(TableQueryBase query) where TElement : new()
        {
            IList<TElement> result = await ExecuteQuerySegmentAsync<TElement>(query);
            return result.FirstOrDefault();
        }

    }
}
