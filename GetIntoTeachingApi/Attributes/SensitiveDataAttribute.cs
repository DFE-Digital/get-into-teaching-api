using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SensitiveDataAttribute : Attribute
    {
    }
}
