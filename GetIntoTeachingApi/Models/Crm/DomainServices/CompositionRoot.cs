using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices
{
    /// <summary>
    /// The composition root provides a unified location in the application where the composition
    /// of the object graphs for the degree-status inference dependencies happen, using the IOC container.
    /// </summary>
    public static class CompositionRoot
    {
        /// <summary>
        /// Extension method which provides all the pre-registrations required to invoke the degree-status inference logic.
        /// </summary>
        /// <param name="services">
        /// The originating application services on which to register the degree-status inference dependencies.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The exception thrown if no valid <see cref="IServiceCollection"/> is provisioned.
        /// </exception>
        public static void RegisterDegreeStatusInferenceServices(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services),
                    "A service collection is required to configure the 'DegreeStatus' inference service dependencies.");
            }

            // Register evaluators.
            services.AddScoped<InferFirstYearOfDegree>();
            services.AddScoped<InferSecondYearOfDegree>();
            services.AddScoped<InferFinalYearOfDegree>();
            services.AddScoped<InferAlreadyHasDegree>();

            // Register the scoped evaluation handlers.
            services.AddScopedEvaluationHandler<InferFirstYearOfDegree>();
            services.AddScopedEvaluationHandler<InferSecondYearOfDegree>();
            services.AddScopedEvaluationHandler<InferFinalYearOfDegree>();
            services.AddScopedEvaluationHandler<InferAlreadyHasDegree>();
            
            // Register the default implementation of the 'ICurrentYearProvider'.
            services.AddTransient<ICurrentYearProvider, CurrentYearProvider>();
        }
    }

    /// <summary>
    /// Extension methods which provide utility methods for the service provider.
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// Utility method which allows us to cleanly register a scoped evaluation handler to the service provider.
        /// </summary>
        /// <typeparam name="TEvaluationHandler">
        /// The type of evaluation handler to be assigned to the <see cref="ChainEvaluationHandler{TEvaluationRequest, TEvaluationResponse}"/>.
        /// </typeparam>
        /// <param name="services">
        /// The service collection instance to which the scoped evaluation handler is to be registered.
        /// </param>
        public static void AddScopedEvaluationHandler<TEvaluationHandler>(
            this IServiceCollection services)
                where TEvaluationHandler : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus> =>
                    services.AddScoped<IChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>>(provider =>
                        provider.AddEvaluationHandler<TEvaluationHandler>());
        /// <summary>
        /// Utility method which allows us to derive an <see cref="ChainEvaluationHandler{TEvaluationRequest, TEvaluationResponse}"/> instance.
        /// </summary>
        /// <typeparam name="TEvaluationHandler">
        /// The type of evaluation handler to be assigned to the <see cref="ChainEvaluationHandler{TEvaluationRequest, TEvaluationResponse}"/>.
        /// </typeparam>
        /// <param name="serviceProvider">
        /// The service provider instance from which to extract the required TEvaluationHandler service.
        /// </param>
        /// <returns>
        /// A configured instance of the <see cref="ChainEvaluationHandler{TEvaluationRequest, TEvaluationResponse}"/>.
        /// </returns>
        public static ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus> AddEvaluationHandler<
            TEvaluationHandler>(this IServiceProvider serviceProvider)
                where TEvaluationHandler : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus> =>
                    new (serviceProvider.CreateScope()
                        .ServiceProvider.GetRequiredService<TEvaluationHandler>());
    }
}