using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Data.Runtime.Sql.Utils
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

        public bool TrySetValue(string name, object obj, object value)
        {
            if (Properties.TryGetValue(name, out PropertyDescription description))
            {
                return description.TrySetValue(name, obj, value);
            }
            foreach (var sub in Properties.Values.Where(p => p.SubDescription != null))
            {
                if(sub.TrySetValue(name, obj, value))
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Properties.GetEnumerator();
        }
    }
}
