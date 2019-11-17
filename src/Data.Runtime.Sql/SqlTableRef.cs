using Data.Runtime.Sql.Queries;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Base Class for Connection TableRef
    /// </summary>
    public abstract class SqlTableRef
    {
        protected SqlTableRef(string tableName, IDbClientImplementation implementation)
        {
            TableName = tableName;
            ClientImplementation = implementation;
        }

        public IDbClientImplementation ClientImplementation { get; }

        public string TableName { get; set; }

        public async Task<IList<TElement>> ExecuteQueryAsync<TElement>(string queryString, Action<TElement> onElementCreated) where TElement : class, new()
        {
            IList<TElement> elements = new List<TElement>();
            var properties = typeof(TElement).GetProperties()
                .Where(property => property.CustomAttributes
                .Any(attribute => attribute.AttributeType.Equals(typeof(DataMemberAttribute))));
            using (var command = ClientImplementation.CreateCommand(queryString))
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    string[] fields = reader.GetFields().ToArray();
                    while (await reader.ReadAsync())
                    {
                        TElement element = new TElement();
                        for (int index = 0; index < fields.Length; index++)
                        {
                            if (reader.IsDBNull(index)) continue;
                            string field = fields[index];
                            PropertyInfo property = properties
                                .GetPropertyWithAttribute(attribute => attribute.NamedArguments.Any(arg => arg.MemberName.Equals(Constants.DataMemberName) && arg.TypedValue.Value.Equals(field)));
                            if (property != null)
                            {
                                property.SetElementValue(element, reader.GetValue(index));
                            }
                        }
                        onElementCreated(element);
                        elements.Add(element);
                    }
                }
            }
            return elements;
        }

        public abstract Task<IList<TElement>> ExecuteQuerySegmentAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new();

        public abstract Task<TElement> ExecuteQueryAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new();

        public abstract Task<QueryResult> ExecuteNonQueryAsync<TElement>(TableQueryBase<TElement> query) where TElement : class, new();
    }
}
