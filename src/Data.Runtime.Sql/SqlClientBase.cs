using SqlDb.Data.Common;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SqlDb.Data
{
    /// <summary>
    /// Base Class of Client Implementation
    /// </summary>
    /// <typeparam name="T">Connection Type <see cref="DbConnection"/></typeparam>
    public abstract class SqlClientBase<T> : ISqlClient where T : DbConnection
    {
        protected SqlClientBase(IConnectionOptions options)
        {
            Options = options ?? throw new System.ArgumentNullException(nameof(options));
        }

        public abstract Task<bool> ConnectAsync();

        public abstract T GetConnection();

        public IConnectionOptions Options { get; set; }

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

        DbConnection ISqlClient.GetConnection()
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
