using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Reference { get; set; }

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