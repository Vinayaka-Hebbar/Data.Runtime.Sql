using System;

namespace SqlDb.Data.Queries
{
    public abstract class TableQueryBase
    {
        protected readonly SqlTable Table;
        protected TableQueryBase(SqlTable table)
        {
            Table = table;
        }

        public abstract TableOperationType OperationType { get; }

        public abstract string GetQueryString();
    }
}