using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Data.Runtime.Sql.Constants;

namespace Data.Runtime.Sql
{
    //Todo fetch of class for ISerializable
    /// <summary>
    /// Factory Implementation Class 
    /// <list type="number">
    ///    <item><description>if class has <see cref="DataMemberAttribute"/> if it has to be out parameter make <see cref="DataMemberAttribute.IsRequired"/> to <code>true</code></description>  </item>
    /// </list>
    /// 
    /// </summary>
    [Obfuscation(Exclude = true)]
    public class SqlDbClient : ISqlDbClient, IEqualityComparer<DbParameter>
    {
        private readonly IDbClientImpl clientImplementation;

        /// <summary>
        /// Creates a Factory Implementation of <paramref name="clientImplementation"/>
        /// </summary>
        /// <param name="clientImplementation"></param>
        public SqlDbClient(IDbClientImpl clientImplementation)
        {
            this.clientImplementation = clientImplementation;
        }

        /// <summary>
        /// Connection state of Client
        /// </summary>
        public bool IsConnected => clientImplementation.IsConnected;

        /// <summary>
        /// DbConnection of Implementation
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return clientImplementation.GetConnection();
        }

        /// <summary>
        /// Creates Connection to Database
        /// </summary>
        /// <returns></returns>
        public async Task<ISqlDbClient> ConnectAsync()
        {
            DbConnection connection = clientImplementation.GetConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
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
            using (DbCommand cmd = clientImplementation.CreateCommand(query))
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
            using (DbCommand cmd = clientImplementation.CreateProcedureCall(procedureName))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DbParameter paramter in GetInputParams(input).Latest())
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
            IEnumerable<Utils.PropertyDescription> propertieList = typeof(TModel).GetPropertyDescriptions();
            var propertiesDesc = new Utils.PropertyDescriptions(propertieList);
            using (DbCommand cmd = clientImplementation.CreateProcedureCall(procedureName))
            {
                IEnumerable<DbParameter> parameters = GetInputParams(input)
                    .Latest()
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
                            string field = fields[index];
                            propertiesDesc.TrySetValue(field, model, reader.GetValue(index));
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
        public async Task<TModel> ExecuteStoredProcedureAsync<TModel>(string procedureName, object input = null) where TModel : class, new()
        {
            var result = await ExecuteProcedureSegmentAsync<TModel>(procedureName, input);
            return result.FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// Get TableRef for Client Implementation
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>Table Implementation for Client</returns>
        public SqlTableRef GetTableRef(string tableName)
        {
            return clientImplementation.GetTableRef(tableName);
        }

        /// <summary>
        /// Dispose of Client Implemetation
        /// </summary>
        public void Dispose()
        {
            clientImplementation.Dispose();
        }

        #region Private Methods
        private IEnumerable<DbParameter> GetOutParams(IEnumerable<Utils.PropertyDescription> properties)
        {
            foreach (var property in properties.Where(p => p.IsRequired).OrderBy(p => p.Order))
            {
                yield return clientImplementation.GetOutputParameter(property);
            }
        }

        private IEnumerable<DbParameter> GetInputParams(object input)
        {
            if (input != null)
            {
                Type objectType = input.GetType();
                if (objectType.IsDefined(typeof(DataContractAttribute), false))
                {
                    foreach (var param in GetInputParams(input, objectType))
                    {
                        yield return param;
                    }
                    yield break;
                }
                var properties = objectType.GetProperties();
                foreach (var property in properties)
                {
                    var propertyType = property.PropertyType;
                    var value = property.GetValue(input);
                    if (propertyType.IsValueType)
                    {
                        yield return clientImplementation.GetInputParameter(value, property.Name, propertyType.FullName);
                    }
                    else if (propertyType.IsSerializable)
                    {
                        //if generic type for dictionary
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
                    else if (propertyType.IsClass)
                    {
                        if (value != null)
                        {
                            foreach (var param in GetInputParams(value, propertyType))
                            {
                                yield return param;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<DbParameter> GetDictionaryParameter(object input)
        {
            if (input != null)
            {
                var dictionary = (IDictionary)input;
                foreach (var key in dictionary.Keys)
                {
                    var value = dictionary[key];
                    var valueType = value.GetType();
                    if (valueType.IsValueType)
                    {
                        yield return clientImplementation.GetInputParameter(value, key.ToString(), valueType.FullName);
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
                    yield return clientImplementation.GetInputParameter(value, property.Name, propertyType.FullName);
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
                else if (propertyType.IsClass)
                {
                    if (value != null)
                    {
                        foreach (var param in GetInputParams(value, propertyType))
                        {
                            yield return param;
                        }
                    }
                }
            }
        }

        private DbParameter GetSerializableParameter(object input, string name, string typeName)
        {
            switch (typeName)
            {
                case TypeString:
                    return clientImplementation.GetInputParameter(input, name, DbType.String);
                case TypeDateTime:
                    return clientImplementation.GetInputParameter(input, name, DbType.DateTime);
                default:
                    var type = clientImplementation.GetDbType(DbType.Object);
                    var value = input != null ? Json.Serialization.JsonConvert.Serialize(input) : EmptyJsonObject;
                    return clientImplementation.GetInputParameter(value, name, type);
            }
        }

        public bool Equals(DbParameter x, DbParameter y)
        {
            return x.ParameterName == y.ParameterName;
        }

        public int GetHashCode(DbParameter obj)
        {
            return obj.ParameterName.GetHashCode();
        }
        #endregion

    }
}