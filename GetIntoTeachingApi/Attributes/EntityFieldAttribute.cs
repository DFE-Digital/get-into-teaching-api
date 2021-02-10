using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }
        public string Reference { get; }
        public string[] IgnoreInEnvironments { get; }

        public bool Ignored
        {
            get
            {
                return IgnoreInEnvironments?.Contains(new Env().EnvironmentName) == true;
            }
        }

        public EntityFieldAttribute(string name, Type type = null, string reference = null, string[] ignoreInEnvironments = null)
        {
            Name = name;
            Type = type;
            Reference = reference;
            IgnoreInEnvironments = ignoreInEnvironments;
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