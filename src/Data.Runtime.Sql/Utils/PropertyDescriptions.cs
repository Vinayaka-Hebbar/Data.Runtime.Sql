using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SqlDb.Data.Utils
{
    internal sealed class PropertyDescriptions : IEnumerable<PropertyDescription>
    {
        internal readonly IDictionary<string, PropertyDescription> Properties;

        public PropertyDescriptions(IEnumerable<PropertyDescription> properties)
        {
            Properties = properties.ToDictionary(p => p.Name);
        }

        public IEnumerator<PropertyDescription> GetEnumerator()
        {
            return Properties.Values.GetEnumerator();
        }

        public bool TrySetValue(object obj, string name, object value)
        {
            if (Properties.TryGetValue(name, out PropertyDescription description))
            {
                return description.TrySetValue(obj, name, value);
            }
            foreach (var sub in Properties.Values.Where(p => p.SubDescription != null))
            {
                if (sub.TrySetValue(obj, name, value))
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Properties.Values.GetEnumerator();
        }
    }
}
