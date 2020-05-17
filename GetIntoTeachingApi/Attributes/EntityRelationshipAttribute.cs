using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityRelationshipAttribute : Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}