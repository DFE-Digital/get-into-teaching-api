using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class KeyedServiceRegistrations
    {
        public static void RegisterKeyedServicesAsDictionary(this IServiceCollection serviceCollection)
        {
            // KeyedServiceCache caches all the keys of a given type for a
            // specific service type. By making it a singleton we only have
            // determine the keys once, which makes resolving the dict very fast.
            serviceCollection.AddSingleton(typeof(KeyedServiceCache<,>));

            // KeyedServiceCache depends on the IServiceCollection to get
            // the list of keys. That's why we register that here as well, as it
            // is not registered by default in MS.DI.
            serviceCollection.AddSingleton(serviceCollection);

            // Last we make the registration for the dictionary itself, which maps
            // to our custom type below. This registration must be  transient, as
            // the containing services could have any lifetime and this registration
            // should by itself not cause Captive Dependencies.
            serviceCollection.AddTransient(typeof(IDictionary<,>), typeof(KeyedServiceRegistrationDictionary<,>));

            // For completeness, let's also allow IReadOnlyDictionary to be resolved.
            serviceCollection.AddTransient(
                typeof(IReadOnlyDictionary<,>), typeof(KeyedServiceRegistrationDictionary<,>));
        }
    }
}
