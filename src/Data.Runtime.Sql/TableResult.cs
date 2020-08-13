using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SqlDb.Data
{
    [Serializable]
    public class TableResult : ISerializable
    {
        private List<object[]> rows;
        private string[] columns;

        public TableResult(string[] columns)
        {
            Columns = columns;
            Rows = new List<object[]>();
        }

        public List<object[]> Rows { get => rows; set => rows = value; }

        public string[] Columns { get => columns; set => columns = value; }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(columns), columns);
            info.AddValue(nameof(rows), rows);
        }

        public override string ToString()
        {
            return $"{Rows.Count} rows";
        }
    }
}
