using System.Data;
using System.Data.Common;

namespace SqlDb.Data.Common
{
    /// <summary>
    /// Interface for Client Implementation of DbParameter 
    /// </summary>
    public interface ISqlParameter
    {
        int GetDbType(string typeName);
        int GetDbType(DbType type);
        DbParameter GetInputParameter(object value, string propertyName, string typeName);
        DbParameter GetInputParameter(object value, string propertyName, int type);
        DbParameter GetInputParameter(object value, string propertyName, DbType type);
        //Todo replace property info with parameters
        DbParameter GetOutputParameter(IPropertyDescription property);
    }
}