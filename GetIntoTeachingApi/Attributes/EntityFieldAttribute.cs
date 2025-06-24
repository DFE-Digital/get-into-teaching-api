using GetIntoTeachingApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }
        public string Reference { get; }
        public string[] Features { get; }

        private readonly bool _ignored;

        public bool Ignored
        {
            get
            {
                if (_ignored || Features == null)
                {
                    return false;
                }

                return Features!.All(f => new Env().IsFeatureOff(f));
            }
        }

        public EntityFieldAttribute(string name, Type type = null, string reference = null, string[] features = null, bool ignore = false)
        {
            Name = name;
            Type = type;
            Reference = reference;
            Features = features;
            _ignored = ignore;
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