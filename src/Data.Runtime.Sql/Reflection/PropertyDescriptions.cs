using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SqlDb.Data.Reflection
{
    internal sealed class PropertyDescriptions : IEnumerable<PropertyDescription>
    {
        internal readonly IReadOnlyDictionary<string, PropertyDescription> Properties;

        public PropertyDescriptions(IEnumerable<PropertyDescription> properties)
        {
            Properties = properties.ToDictionary(p => p.m_name);
        }

        public IEnumerator<PropertyDescription> GetEnumerator()
        {
            return Properties.Values.GetEnumerator();
        }

        public bool SetValue(string name, object value, object obj)
        {
            if (Properties.TryGetValue(name, out PropertyDescription description))
            {
                return description.SetValue(obj, value);
            }
            foreach (var sub in Properties.Values)
            {
                if (sub.SubDescription != null)
                {
                    // sub property of property type this should not be equal to current type
                    var instance = sub.GetValue(obj);
                    if (instance == null)
                    {
                        if (sub.Property.CanWrite == false) continue;
                        instance = System.Activator.CreateInstance(sub.m_propType);
                        sub.Property.SetValue(obj, instance);
                    }
                    if (sub.SubDescription.SetValue(name, value, instance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Exists(string name)
        {
            return Properties.ContainsKey(name);
        }

        public bool TryGetProperty(string name, out PropertyDescription description)
        {
            if (Properties.TryGetValue(name, out description))
            {
                return true;
            }
            foreach (var desc in Properties.Values)
            {
                if (desc.SubDescription != null)
                {
                    if (desc.SubDescription.Exists(name))
                    {
                        description = desc;
                        return true;
                    }
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
