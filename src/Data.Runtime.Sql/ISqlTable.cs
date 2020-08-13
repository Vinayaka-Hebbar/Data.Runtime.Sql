using SqlDb.Data.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlDb.Data
{
    public interface ISqlTable
    {
        string TableName { get; }

        Task<QueryResult> ExecuteNonQueryAsync(QueryString queryString);
        Task<IList<TElement>> ExecuteQueryAsync<TElement>(QueryString queryString, Action<TElement> onElementCreated) where TElement : new();
        Task<ScalarResult> ExecuteScalarAsync(QueryString queryString);
    }
}