using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }
        public string Reference { get; }
        public bool Transient { get; }

        public EntityFieldAttribute(string name, Type type = null, string reference = null, bool transient = false)
        {
            Name = name;
            Type = type;
            Reference = reference;
            Transient = transient;
        }

        public IDictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>() { { "Name", Name } };

            if (Type != null)
            {
                dictionary.Add("Type", Type.ToString());
            }

            if (Reference != null)
            {
                dictionary.Add("Reference", Reference);
            }

            return dictionary;
        }
    }
}