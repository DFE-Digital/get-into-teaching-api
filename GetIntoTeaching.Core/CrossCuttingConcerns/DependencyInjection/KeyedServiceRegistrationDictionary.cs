using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.DependencyInjection
{
    public sealed class KeyedServiceRegistrationDictionary<TKey, TService> :
        ReadOnlyDictionary<TKey, TService>
            where TKey : notnull
            where TService : notnull
    {
        public KeyedServiceRegistrationDictionary(
            KeyedServiceCache<TKey, TService> keys, IServiceProvider serviceProvider) :
            base(Create(keys, serviceProvider)){
        }

        private static Dictionary<TKey, TService> Create(
            KeyedServiceCache<TKey, TService> keys, IServiceProvider serviceProvider)
        {
            Dictionary<TKey, TService> dict = new(capacity: keys.Keys.Length);

            foreach (TKey key in keys.Keys)
            {
                dict[key] = serviceProvider.GetRequiredKeyedService<TService>(key);
            }

            return dict;
        }
    }

    public sealed class KeyedServiceCache<TKey, TService>
        where TKey : notnull
        where TService : notnull
    {
        private readonly IServiceCollection _serviceCollection;

        public KeyedServiceCache(IServiceCollection serviceCollection)  
        {
            _serviceCollection = serviceCollection;
        }

        public TKey[] Keys => (
            from service in _serviceCollection
            where service.ServiceKey != null
            where service.ServiceKey!.GetType() == typeof(TKey)
            where service.ServiceType == typeof(TService)
            select (TKey)service.ServiceKey!)
            .ToArray();
    }
}
