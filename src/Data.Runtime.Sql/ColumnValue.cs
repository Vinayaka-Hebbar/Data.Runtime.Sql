using System;
using System.Runtime.Serialization;

namespace SqlDb.Data
{
    [Serializable]
    public
#if LATEST_VS
        readonly
#endif
        struct ColumnValue
    {
        readonly string name;
        readonly object value;

        internal ColumnValue(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
        
        public string Name => name;

        public object Value => value;
    }
}
