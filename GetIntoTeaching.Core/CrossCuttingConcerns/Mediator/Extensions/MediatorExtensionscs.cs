using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator.Extensions
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediator(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            params Assembly[] assemblies)
        {
            ConcurrentDictionary<Type, Type>? handlerInfos = new();

            foreach (var assembly in assemblies)
            {
                List<Type>? requests = GetImplementationsFor(assembly, typeof(IRequest<>));
                List<Type>? requestHandlers = GetImplementationsFor(assembly, typeof(IHandler<,>));

                requests.ForEach(request =>
                    handlerInfos[request] = requestHandlers.SingleOrDefault(handler =>
                        request == handler.GetInterface("IRequestHandler`2")!.GetGenericArguments()[0]));

                var handlerServiceDescriptors =
                    requestHandlers.Select(type =>
                        new ServiceDescriptor(type, type, serviceLifetime));

                services.TryAdd(handlerServiceDescriptors);
            }

            services.AddSingleton<IMediator>(serviceProvider =>
                new Mediator(serviceProvider.GetRequiredService, handlerInfos));

            return services;
        }

        private static List<Type> GetImplementationsFor(Assembly assembly, Type interfaceType)
        {
            return assembly.ExportedTypes.Where(type =>
            {
                IEnumerable<Type> genericInterfacesTypes =
                    type.GetInterfaces().Where(type => type.IsGenericType);

                bool implementRequestType =
                    genericInterfacesTypes.Any(type =>
                        type.GetGenericTypeDefinition() == interfaceType);

                return !type.IsInterface && !type.IsAbstract && implementRequestType;
            })
            .ToList();
        }
    }
}
