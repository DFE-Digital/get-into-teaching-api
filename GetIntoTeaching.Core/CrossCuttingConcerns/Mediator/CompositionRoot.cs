using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace GetIntoTeaching.Core.CrossCuttingConcerns.Mediator
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompositionRoot
    {
        public static IServiceCollection AddMediatorRegistrations(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            params Assembly[] assemblies)
        {
            ConcurrentDictionary<Type, Type>? handlersMetadata = new();
            const string handlerInterfaceName = "IHandler`2";

            foreach (var assembly in assemblies)
            {
                assembly.GetTypesFor(typeof(IRequest<>))
                    .ForEach(request =>
                        handlersMetadata[request] =
                            assembly.GetTypesFor(typeof(IHandler<,>)).SingleOrDefault(handler =>
                                request == handler.GetInterface(handlerInterfaceName)!.GetGenericArguments()[0]));

                IEnumerable<ServiceDescriptor> handlerServiceDescriptors =
                    assembly.GetTypesFor(typeof(IHandler<,>)).Select(type =>
                        new ServiceDescriptor(type, type, serviceLifetime));

                services.TryAdd(handlerServiceDescriptors);
            }

            services.AddSingleton<IMediator>(serviceProvider =>
                new Mediator(serviceProvider.GetRequiredService, handlersMetadata));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private static List<Type> GetTypesFor(this Assembly assembly, Type interfaceType)
        {
            return assembly.ExportedTypes.Where(type =>
            {
                IEnumerable<Type> genericInterfaceTypes =
                    type.GetInterfaces()
                        .Where(type => type.IsGenericType);

                bool implementsRequestType =
                    genericInterfaceTypes.Any(type =>
                        type.GetGenericTypeDefinition() == interfaceType);

                return !type.IsInterface && !type.IsAbstract && implementsRequestType;
            })
            .ToList();
        }
    }
}
