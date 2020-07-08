using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

/**
 * @author Vinayaka Hebbar
**/

namespace SqlDb.Data.Queries
{
    public class TableInsertQuery<TElement> : TableQueryBase
    {
        protected internal TableInsertQuery(TElement element, SqlTable table) : base(table)
        {
            Element = element;
            IEnumerable<PropertyInfo> properties = element.GetType().GetProperties().GetPropertiesWithAttribute(typeof(DataMemberAttribute));
            Columns = properties.GetDataMemberNames().ToArray();
            Values = properties.Select(property => property.GetValue(element)).ToArray();
        }

        public TElement Element { get; }

        public ICollection<string> Columns { get; set; }

        public ICollection<object> Values { get; set; }

        public TableInsertQuery<TElement> SetValues(params object[] values)
        {
            Values = values;
            return this;
        }

        public TableInsertQuery<TElement> SetColumns(params string[] columns)
        {
            Columns = columns;
            return this;
        }

        public override TableOperationType OperationType => TableOperationType.Insert;

        public Task<QueryResult> ExecuteAsync()
        {
            return Table.ExecuteNonQueryAsync(new QueryString(Table.BuildInsertQuery(this), TableOperationType.Insert));
        }

        public override string GetQueryString()
        {
            return Table.BuildInsertQuery(this);
        }
    }
}
