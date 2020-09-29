using System;

namespace GetIntoTeachingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LoggableAttribute : Attribute
    {
    }
}
