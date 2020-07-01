using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        public string LogicalName { get; }

        public EntityAttribute(string logicalName)
        {
            LogicalName = logicalName;
        }
    }
}