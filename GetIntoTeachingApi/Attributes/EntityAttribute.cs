using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityAttribute : Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Reference { get; set; }
    }
}