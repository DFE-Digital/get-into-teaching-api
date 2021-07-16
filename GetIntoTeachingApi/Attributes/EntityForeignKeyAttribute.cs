using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityForeignKeyAttribute : Attribute
    {
        public string NavigationProperty { get; }

        public EntityForeignKeyAttribute(string navigationproperty)
        {
            NavigationProperty = navigationproperty;
        }
    }
}
