using System.Data.Common;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Interface for Client Implementation of Connection
    /// </summary>
    public interface IDbClientImplementation : IDbParameterImplementation, System.IDisposable
    {
        SqlTableRef GetTableRef(string tableName);
        Task ConnectAsync();
        bool IsConnected { get; }
        DbConnection GetConnection();
        DbCommand CreateCommand(string cmdText);
        DbCommand CreateProcedureCall(string procedureName);
        DbConnectionOptions Options { get; }
    }
}