#if NETFRAMEWORK
using System.Configuration;

namespace SqlDb.Data.ConfigFile
{
    [ConfigurationCollection(typeof(ProviderSettings), AddItemName = ProviderKey)]
    public class ProviderCollection : ConfigurationElementCollection
    {
        public const string ProviderKey = "provider";

        readonly ConfigurationPropertyCollection _properties;

        public ProviderCollection()
        {
            _properties = new ConfigurationPropertyCollection();
        }

        protected override string ElementName
        {
            get
            {
                return ProviderKey;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderSettings();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderSettings)element).Name;
        }

        public new ProviderSettings this[string key]
        {
            get
            {
                return (ProviderSettings)BaseGet(key);
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get => _properties;
        }
    }
}
#endif