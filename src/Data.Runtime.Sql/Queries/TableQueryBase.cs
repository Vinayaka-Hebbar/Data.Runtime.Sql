using System;

namespace Data.Runtime.Sql.Queries
{
    public abstract class TableQueryBase<TElement> where TElement : class, new()
    {
        protected TableQueryBase(TableOperationType operationType)
        {
            OperationType = operationType;
        }

        public TableOperationType OperationType { get; }

        private static readonly Action<TElement> nothing = (e) => { };

        public Action<TElement> ElementCreated { get; private set; } = nothing;

        public TableQueryBase<TElement> OnElementCreated(Action<TElement> onRefElement)
        {
            ElementCreated = onRefElement;
            return this;
        }
    }
}