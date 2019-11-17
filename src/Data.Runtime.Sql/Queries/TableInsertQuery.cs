using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

/**
 * @author Vinayaka Hebbar
**/

namespace Data.Runtime.Sql.Queries
{
    public class TableInsertQuery<TElement> : TableQueryBase<TElement> where TElement : class, new()
    {
        protected internal TableInsertQuery(TElement element) : base(TableOperationType.Insert)
        {
            Element = element;
            IEnumerable<PropertyInfo> properties = typeof(TElement).GetProperties().GetPropertiesWithAttribute(typeof(DataMemberAttribute));
            Columns = properties.GetDataMemberNames();
            Values = properties.Select(property => property.GetValue(element));
        }

        public TElement Element { get; }

        public IEnumerable<string> Columns { get; set; }

        public IEnumerable<object> Values { get; set; }

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
    }
}
