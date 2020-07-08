#if NETFRAMEWORK
using System.Configuration;
using System.Linq;

namespace SqlDb.Data.ConfigFile
{
    // <summary>
    // Represents a series of parameters to pass to a method
    // </summary>
    public sealed class ParameterCollection : ConfigurationElementCollection
    {
        private const string ParameterKey = "parameter";
        private int _nextKey;

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new ParameterElement(_nextKey);
            _nextKey++;
            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return ParameterKey; }
        }

        public object[] GetTypedParameterValues()
        {
            return this.Cast<ParameterElement>()
                       .Select(e => e.GetTypedParameterValue())
                       .ToArray();
        }

        // <summary>
        // Adds a new parameter to the collection
        // Used for unit testing
        // </summary>
        internal ParameterElement NewElement()
        {
            var element = CreateNewElement();
            base.BaseAdd(element);
            return (ParameterElement)element;
        }
    }
}

#endif