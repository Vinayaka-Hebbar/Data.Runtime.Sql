using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDb.Data.Queries
{
    public class BatchQuery : TableQueryBase
    {
        readonly TableQueryBase[] Queries;

        public BatchQuery(SqlTable table, TableQueryBase[] tableQueries) : base(table)
        {
            Queries = tableQueries;
        }

        public override TableOperationType OperationType => 0;

        public override string GetQueryString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Queries.Length; i++)
            {
                sb.AppendLine(Queries[i].GetQueryString());
            }
            return sb.ToString();
        }

        public async Task ExecuteAsync()
        {
            foreach (var query in Queries)
            {
                await Table.ExecuteNonQueryAsync(new QueryString(query.GetQueryString(), query.OperationType));
            }
        }
    }
}
