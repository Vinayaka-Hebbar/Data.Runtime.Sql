using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Interface for Client Implementation of DbParameter 
    /// </summary>
    public interface IDbParameterImplementation
    {
        int GetDbType(string typeName);
        int GetDbType(DbType type);
        DbParameter GetInputParameter(object value, string propertyName, string typeName);
        DbParameter GetInputParameter(object value, string propertyName, int type);
        DbParameter GetInputParameter(object value, string propertyName, DbType type);
        //Todo replace property info with parameters
        DbParameter GetOutputParameter(Utils.PropertyDescription property);
    }
}