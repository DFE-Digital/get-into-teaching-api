using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityRelationshipAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }

        public EntityRelationshipAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public IDictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>() { { "Name", Name } };

            if (Type != null)
            {
                dictionary.Add("Type", Type.ToString());
            }

            return dictionary;
        }
    }
}