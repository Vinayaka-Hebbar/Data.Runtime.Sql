#if NET45 || NET40
using Data.Runtime.Sql.Utils;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Data.Runtime.Sql
{

    public sealed class SqlClient : DbClientImpl<SqlConnection>
    {
        private SqlConnection connection;

        public override bool IsConnected => connection != null && connection.State == ConnectionState.Open;

        public async override System.Threading.Tasks.Task ConnectAsync()
        {
            connection = new SqlConnection(Options["ConnectionString"]);
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
        }

        public override DbCommand CreateCommand(string cmdText)
        {
            return new SqlCommand(cmdText, connection);
        }

        public override DbCommand CreateProcedureCall(string procedureName)
        {
            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        public override SqlConnection GetConnection()
        {
            return connection;
        }

        private static SqlDbType GetMySqlDbType(string typeName)
        {
            switch (typeName)
            {
                case Constants.TypeByte:
                case Constants.TypeSByte:
                    return SqlDbType.TinyInt;
                case Constants.TypeInt16:
                case Constants.TypeUInt16:
                    return SqlDbType.SmallInt;
                case Constants.TypeInt32:
                case Constants.TypeUInt32:
                    return SqlDbType.Int;
                case Constants.TypeInt64:
                case Constants.TypeUInt64:
                    return SqlDbType.BigInt;
                case Constants.TypeBool:
                    return SqlDbType.Bit;
                case Constants.TypeChar:
                    return SqlDbType.Char;
                case Constants.TypeDateTime:
                    return SqlDbType.DateTime;
                case Constants.TypeGuid:
                    return SqlDbType.UniqueIdentifier;
                case Constants.TypeFloat:
                    return SqlDbType.Real;
                case Constants.TypeDouble:
                    return SqlDbType.Float;
                case Constants.TypeDecimal:
                    return SqlDbType.Decimal;
                case Constants.TypeString:
                case Constants.TypeStringArray:
                    return SqlDbType.VarChar;
                default:
                    return SqlDbType.Variant;
            }
        }

        private static SqlDbType GetMySqlDbType(DbType type)
        {
            switch (type)
            {
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.String:
                    return SqlDbType.VarChar;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.SByte:
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Date:
                    return SqlDbType.Date;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Time:
                    return SqlDbType.Time;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Int16:
                case DbType.UInt16:
                    return SqlDbType.SmallInt;
                case DbType.Int32:
                case DbType.UInt32:
                    return SqlDbType.Int;
                case DbType.Int64:
                case DbType.UInt64:
                    return SqlDbType.BigInt;
                case DbType.Decimal:
                case DbType.Currency:
                case DbType.VarNumeric:
                    return SqlDbType.Decimal;
                case DbType.Binary:
                    return SqlDbType.Binary;
                case DbType.Object:
                default:
                    return SqlDbType.Variant;
            }
        }

        public override int GetDbType(string typeName)
        {
            return (int)GetMySqlDbType(typeName);
        }

        public override int GetDbType(DbType type)
        {
            return (int)GetMySqlDbType(type);
        }

        public override DbParameter GetInputParameter(object value, string propertyName, string typeName)
        {
            return new SqlParameter(propertyName, GetMySqlDbType(typeName))
            {
                Value = value,
                Direction = ParameterDirection.Input
            };
        }

        public override DbParameter GetInputParameter(object value, string propertyName, int type)
        {
            return new SqlParameter(propertyName, (SqlDbType)type)
            {
                Value = value,
                Direction = ParameterDirection.Input
            };
        }

        public override DbParameter GetInputParameter(object value, string propertyName, DbType type)
        {
            return new SqlParameter(propertyName, value)
            {
                DbType = type,
                Direction = ParameterDirection.Input
            };
        }

        public override DbParameter GetOutputParameter(PropertyDescription property)
        {
            return new SqlParameter(property.Name, GetMySqlDbType(property.PropertyType.FullName))
            {
                Direction = ParameterDirection.Output
            };
        }
    }
}
#endif
