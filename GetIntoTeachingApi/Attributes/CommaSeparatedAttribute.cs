using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class CommaSeparatedAttribute : Attribute
    {
        public CommaSeparatedAttribute()
        {
        }

        public CommaSeparatedAttribute(params string[] attributeNames)
        {
            AttributeNames = attributeNames;
        }

        public string[] AttributeNames { get; }
    }
}
