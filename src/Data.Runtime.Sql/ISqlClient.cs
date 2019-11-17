using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Interface for Factory Implementation
    /// </summary>
    public interface ISqlDbClient : System.IDisposable
    {
        bool IsConnected { get; }

        DbConnection GetConnection();
        Task<ISqlDbClient> ConnectAsync();

        Task<QueryResult> ExecuteNonQueryAsync(string query);

        Task<QueryResult> ExecuteProcedureAsync(string procedureName, object input);
        QueryResult ExecuteProcedure(string procedureName, object input);
        Task<List<TModel>> ExecuteProcedureSegmentAsync<TModel>(string procedureName, object input = null) where TModel : class, new();
        List<TModel> ExecuteProcedureSegment<TModel>(string procedureName, object input = null) where TModel : class, new();
        Task<TModel> ExecuteStoredProcedureAsync<TModel>(string procedureName, object input = null) where TModel : class, new();
        TModel ExecuteStoredProcedure<TModel>(string procedureName, object input = null) where TModel : class, new();
    }
}
