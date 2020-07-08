using System.Data.Common;
using System.Threading.Tasks;

namespace SqlDb.Data.Common
{
    /// <summary>
    /// Interface for Client Implementation of Connection
    /// </summary>
    public interface ISqlClient : ISqlParameter, System.IDisposable
    {
        SqlTableRef GetTableRef(string tableName);
        Task<bool> ConnectAsync();
        bool IsConnected { get; }
        DbConnection GetConnection();
        DbCommand CreateCommand(string cmdText);
        DbCommand CreateProcedureCall(string procedureName);
        IConnectionOptions Options { get; set; }
    }
}