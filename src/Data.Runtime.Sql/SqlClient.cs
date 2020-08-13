using SqlDb.Data.Common;
using SqlDb.Data.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static SqlDb.Data.Constants;

namespace SqlDb.Data
{
    //Todo fetch of class for ISerializable
    /// <summary>
    /// Factory Implementation Class 
    /// <list type="number">
    ///    <item><description>if class has <see cref="DataMemberAttribute"/> if it has to be out parameter make <see cref="DataMemberAttribute.IsRequired"/> to <code>true</code></description>  </item>
    /// </list>
    /// </summary>
    public class SqlClient : IDisposable
    {
        private readonly ISqlClient impl;

        /// <summary>
        /// Creates a Factory Implementation of <paramref name="impl"/>
        /// </summary>
        /// <param name="impl"></param>
        public SqlClient(ISqlClient impl)
        {
            this.impl = impl;
        }

#if NETFRAMEWORK
        public SqlClient() : this(SqlFactory.Default.CreateSqlClient())
        {

        }
#endif
        /// <summary>
        /// Connection state of Client
        /// </summary>
        public bool IsConnected => impl.IsConnected;

        /// <summary>
        /// DbConnection of Implementation
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return impl.GetConnection();
        }

        /// <summary>
        /// Creates Connection to Database
        /// </summary>
        /// <returns></returns>
        public async Task<SqlClient> ConnectAsync()
        {
            if (await impl.ConnectAsync())
            {
                var connection = impl.GetConnection();
                if (connection == null)
                    throw new NullReferenceException(nameof(connection));
                await connection.OpenAsync();
            }

            return this;
        }

        #region Synchronous Methods
        /// <summary>
        /// Non Query Procedure call (Synchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>Query Result</returns>
        public QueryResult ExecuteProcedure(string procedureName, object input = null)
        {
            return ExecuteProcedureAsync(procedureName, input).Result;
        }

        /// <summary>
        /// Procedure call with list of elements as result (Synchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>List of Elements</returns>
        public List<TModel> ExecuteProcedureSegment<TModel>(string procedureName, object input) where TModel : class, new()
        {
            return ExecuteProcedureSegmentAsync<TModel>(procedureName, input).Result;
        }

        /// <summary>
        /// Procedure call with single element as result (Synchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>Single Element of the result</returns>
        public TModel ExecuteStoredProcedure<TModel>(string procedureName, object input = null) where TModel : class, new()
        {
            return ExecuteProcedureSegmentAsync<TModel>(procedureName, input).Result.FirstOrDefault();
        }

        #endregion

        #region Asynchronous Methods
        /// <summary>
        /// Non Query Sql Call
        /// </summary>
        /// <param name="query">Query String</param>
        /// <returns></returns>
        public async Task<QueryResult> ExecuteNonQueryAsync(string query)
        {
            using (DbCommand cmd = impl.CreateCommand(query))
            {
                try
                {
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                    return QueryResult.Success(affectedRows);
                }
                catch (Exception ex)
                {
                    return QueryResult.Error(ex);
                }
            }
        }
        /// <summary>
        /// Non Query Procedure call (Asynchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>Query Result</returns>
        public async Task<QueryResult> ExecuteProcedureAsync(string procedureName, object input)
        {
            using (DbCommand cmd = impl.CreateProcedureCall(procedureName))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DbParameter paramter in GetInputParams(input))
                {
                    cmd.Parameters.Add(paramter);
                }
                try
                {
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                    return QueryResult.Success(affectedRows);
                }
                catch (Exception ex)
                {
                    return QueryResult.Error(ex);
                }
            }
        }

        /// <summary>
        /// Procedure call with list of elements as result (Asynchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>List of Elements</returns>
        public async Task<List<TModel>> ExecuteProcedureSegmentAsync<TModel>(string procedureName, object input = null) where TModel : class, new()
        {
            List<TModel> results = new List<TModel>();
            IEnumerable<PropertyDescription> propertieList = typeof(TModel).GetPropertyDescriptions();
            var propertiesDesc = new PropertyDescriptions(propertieList);
            using (DbCommand cmd = impl.CreateProcedureCall(procedureName))
            {
                IEnumerable<DbParameter> parameters = GetInputParams(input)
                    .Concat(GetOutParams(propertieList));
                foreach (DbParameter paramter in parameters)
                {
                    cmd.Parameters.Add(paramter);
                }
                using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    string[] fields = reader.GetFields().ToArray();
                    while (await reader.ReadAsync())
                    {
                        var model = new TModel();
                        for (int index = 0; index < fields.Length; index++)
                        {
                            if (reader.IsDBNull(index)) continue;
                            propertiesDesc.SetValue(fields[index], reader.GetValue(index), model);
                        }
                        results.Add(model);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Procedure call with single element as result (Asynchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns>Single Element of the result</returns>
        public async Task<TModel> ExecuteProcedureAsync<TModel>(string procedureName, object input = null) where TModel : class, new()
        {
            var result = await ExecuteProcedureSegmentAsync<TModel>(procedureName, input);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Procedure call with <see cref="TableResult"/> as result (Asynchronous)
        /// </summary>
        /// <param name="procedureName">Name of the Procedure Call</param>
        /// <param name="input">Input for the procedure call</param>
        /// <returns><see cref="TableResult"/> of stored procedure</returns>
        public async Task<TableResult> ExecuteStoredProcedureAsync(string procedureName, object input = null)
        {
            using (DbCommand cmd = impl.CreateProcedureCall(procedureName))
            {
                IEnumerable<DbParameter> parameters = GetInputParams(input);
                foreach (DbParameter paramter in parameters)
                {
                    cmd.Parameters.Add(paramter);
                }
                using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    string[] fields = reader.GetFields().ToArray();
                    TableResult result = new TableResult(fields);
                    int length = fields.Length;
                    while (await reader.ReadAsync())
                    {
                        var row = new object[length];
                        for (int index = 0; index < length; index++)
                        {
                            if (reader.IsDBNull(index)) continue;
                            row[index]= reader.GetValue(index);
                        }
                        result.Rows.Add(row);
                    }
                    return result;
                }
            }
        }
        #endregion

        /// <summary>
        /// Get TableRef for Client Implementation
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>Table Implementation for Client</returns>
        public SqlTableRef GetTableRef(string tableName)
        {
            return impl.GetTableRef(tableName);
        }

        #region Private Methods
        private IEnumerable<DbParameter> GetOutParams(IEnumerable<PropertyDescription> properties)
        {
            return properties.Where(p => p.IsRequired)
                .OrderBy(p => p.Order).Select(p => impl.GetOutputParameter(p));
        }

        private IEnumerable<DbParameter> GetInputParams(object input)
        {
            if (input != null)
            {
                // result of the properties
                InputParams result = new InputParams();
                Type objectType = input.GetType();
                if (objectType.IsDefined(typeof(DataContractAttribute), false))
                {
                    result.Set(GetInputParams(input, objectType));
                    return result;
                }
                foreach (var property in objectType.GetProperties())
                {
                    var propertyType = property.PropertyType;
                    var value = property.GetValue(input);
                    if (propertyType.IsValueType)
                    {
                        if (propertyType.IsPrimitive || propertyType.IsEnum == false)
                        {
                            result[property.Name] = impl.GetInputParameter(value, property.Name, propertyType.FullName);
                        }
                        else
                        {
                            System.Reflection.FieldInfo field = propertyType.GetField(EnumValue, InstantPublic);
                            result[property.Name] = impl.GetInputParameter(field.GetValue(value), property.Name, field.FieldType.FullName);
                        }
                    }
                    else if (propertyType.IsSerializable)
                    {
                        //if generic type for dictionary
                        if (propertyType.IsGenericType)
                        {
                            //Check whether Dictionary Value
                            if (typeof(IDictionary).IsAssignableFrom(propertyType))
                            {
                                result.Set(GetDictionaryParameter(value));
                                continue;
                            }
                        }
                        result[property.Name] = GetSerializableParameter(value, property.Name, propertyType.FullName);
                    }
                    else if (propertyType.IsClass && value != null)
                    {
                        result.Set(GetInputParams(value, propertyType));
                    }
                }
                return result;
            }
            return new DbParameter[0];
        }

        private IEnumerable<DbParameter> GetDictionaryParameter(object input)
        {
            if (input != null)
            {
                var dictionary = (IDictionary)input;
                foreach (object key in dictionary.Keys)
                {
                    var value = dictionary[key];
                    var valueType = value.GetType();
                    if (valueType.IsValueType)
                    {
                        if (valueType.IsPrimitive || valueType.IsEnum == false)
                        {
                            yield return impl.GetInputParameter(value, key.ToString(), valueType.FullName);
                        }
                        else
                        {
                            System.Reflection.FieldInfo field = valueType.GetField(EnumValue, InstantPublic);
                            yield return impl.GetInputParameter(field.GetValue(value), key.ToString(), field.FieldType.FullName);
                        }
                    }
                    else if (valueType.IsSerializable)
                    {
                        yield return GetSerializableParameter(value, key.ToString(), valueType.FullName);
                    }
                }
            }
        }

        private IEnumerable<DbParameter> GetInputParams(object input, Type objectType)
        {
            foreach (var property in objectType.GetPropertyItems())
            {
                Type propertyType = property.Type;
                object value = property.GetValue(input);

                if (propertyType.IsValueType)
                {
                    if (propertyType.IsPrimitive || propertyType.IsEnum == false)
                    {
                        yield return impl.GetInputParameter(value, property.Name, propertyType.FullName);
                    }
                    else
                    {
                        System.Reflection.FieldInfo field = propertyType.GetField(EnumValue, InstantPublic);
                        yield return impl.GetInputParameter(field.GetValue(value), property.Name, field.FieldType.FullName);
                    }
                }
                else if (propertyType.IsSerializable)
                {
                    if (propertyType.IsGenericType)
                    {
                        //Check whether Dictionary Value
                        if (typeof(IDictionary).IsAssignableFrom(propertyType))
                        {
                            foreach (var parameter in GetDictionaryParameter(value))
                            {
                                yield return parameter;
                            }
                        }
                        continue;
                    }
                    yield return GetSerializableParameter(value, property.Name, propertyType.FullName);
                }
                else if (propertyType.IsClass && value != null)
                {
                    foreach (var param in GetInputParams(value, propertyType))
                    {
                        yield return param;
                    }
                }
            }
        }

        private DbParameter GetSerializableParameter(object input, string name, string typeName)
        {
            switch (typeName)
            {
                case TypeString:
                    return impl.GetInputParameter(input, name, DbType.String);
                case TypeDateTime:
                    return impl.GetInputParameter(input, name, DbType.DateTime);
                default:
                    var value = input != null ? Serialization.JsonConvert.Serialize(input) : EmptyJsonObject;
                    return impl.GetInputParameter(value, name, impl.GetDbType(DbType.Object));
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    impl.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        class InputParams : Dictionary<string, DbParameter>, IEnumerable<DbParameter>
        {
            internal void Set(IEnumerable<DbParameter> parameters)
            {
                foreach (var item in parameters)
                {
                    this[item.ParameterName] = item;
                }
            }

            IEnumerator<DbParameter> IEnumerable<DbParameter>.GetEnumerator()
            {
                return Values.GetEnumerator();
            }
        }
    }
}