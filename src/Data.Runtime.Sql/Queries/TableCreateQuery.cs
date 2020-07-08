using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDb.Data.Queries
{
    public class TableCreateQuery<TElement> : TableQueryBase
    {
        public TableCreateQuery(SqlTable table) : base(table)
        {
        }

        public override TableOperationType OperationType => TableOperationType.Create;

        public override string GetQueryString()
        {
            return Table.BuildCreateQuery(this);
        }
    }
}
