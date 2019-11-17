using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Base Class of Client Implementation
    /// </summary>
    /// <typeparam name="T">Connection Type <see cref="DbConnection"/></typeparam>
    public abstract class DbImplementation<T> : IDbClientImplementation where T : DbConnection
    {
        public DbImplementation()
        {
            Options = new DbConnectionOptions();
        }

        public abstract Task ConnectAsync();

        public abstract T GetConnection();

        public DbConnectionOptions Options { get; }

        public abstract bool IsConnected { get; }

        public abstract SqlTableRef GetTableRef(string tableName);

        public virtual void Dispose()
        {

        }

        DbConnection IDbClientImplementation.GetConnection()
        {
            return GetConnection();
        }

        public abstract DbCommand CreateCommand(string cmdText);

        public abstract DbCommand CreateProcedureCall(string procedureName);

        public abstract DbParameter GetOutputParameter(Utils.PropertyDescription property);

        public abstract DbParameter GetInputParameter(object value, string propertyName, string typeName);

        public abstract DbParameter GetInputParameter(object value, string propertyName, int type);

        public abstract int GetDbType(string typeName);

        public abstract int GetDbType(DbType type);

        public abstract DbParameter GetInputParameter(object value, string propertyName, DbType type);
    }
}
