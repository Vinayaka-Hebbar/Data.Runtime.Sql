using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Base Class of Client Implementation
    /// </summary>
    /// <typeparam name="T">Connection Type <see cref="DbConnection"/></typeparam>
    public abstract class DbClientImpl<T> : IDbClientImpl where T : DbConnection
    {
        public DbClientImpl()
        {
            Options = new DbConnectionOptions();
        }

        public abstract Task ConnectAsync();

        public abstract T GetConnection();

        public DbConnectionOptions Options { get; }

        public abstract bool IsConnected { get; }

        public virtual SqlTableRef GetTableRef(string tableName)
        {
            return new SqlTableRef(tableName, this);
        }

        public virtual void Dispose()
        {
            var connection = GetConnection();
            if (connection != null)
            {
                connection.Dispose();
            }
        }

        DbConnection IDbClientImpl.GetConnection()
        {
            return GetConnection();
        }

        public abstract DbCommand CreateCommand(string cmdText);

        public abstract DbCommand CreateProcedureCall(string procedureName);

        public abstract DbParameter GetOutputParameter(Utils.PropertyDescription property);

        public abstract DbParameter GetInputParameter(object value, string propertyName, string typeName);

        public abstract DbParameter GetInputParameter(object value, string propertyName, int type);

        /// <summary>
        /// type name to implemented DbType
        /// </summary>
        /// <param name="typeName">TypeName</param>
        /// <returns>implemented Dbtype value</returns>
        public abstract int GetDbType(string typeName);

        /// <summary>
        /// DbType to implemented DbType
        /// </summary>
        /// <param name="typeName">TypeName</param>
        /// <returns>implemented Dbtype value</returns>
        public abstract int GetDbType(DbType type);

        public abstract DbParameter GetInputParameter(object value, string propertyName, DbType type);
    }
}
