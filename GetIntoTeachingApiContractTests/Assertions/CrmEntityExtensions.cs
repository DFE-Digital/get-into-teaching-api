using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApiContractTests.Assertions
{
    public static class CrmEntityExtensions 
    {
        public static CrmEntityAssertions Should(this Entity instance)
        {
            return new CrmEntityAssertions(instance); 
        } 
    }
}