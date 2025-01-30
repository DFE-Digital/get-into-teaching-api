using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompositionRoot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterDegreeStatusInferenceServices(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services),
                    "A service collection is required to configure the 'DegreeStatus' inference service dependencies.");
            }

            services.AddScoped<InferFirstYearOfDegree>();
            services.AddScoped<InferSecondYearOfDegree>();
            services.AddScoped<InferFinalYearOfDegree>();
            services.AddScoped<InferAlreadyHasDegree>();

            services.AddScopedEvaluationHandler<InferFirstYearOfDegree>();
            services.AddScopedEvaluationHandler<InferSecondYearOfDegree>();
            services.AddScopedEvaluationHandler<InferFinalYearOfDegree>();
            services.AddScopedEvaluationHandler<InferAlreadyHasDegree>();

            services.AddTransient<ICurrentYearProvider, CurrentYearProvider>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvaluationHandler"></typeparam>
        /// <param name="services"></param>
        public static void AddScopedEvaluationHandler<TEvaluationHandler>(
            this IServiceCollection services)
                where TEvaluationHandler : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus> =>
                    services.AddScoped<IChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>>(provider =>
                        provider.AddEvaluationHandler<TEvaluationHandler>());
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvaluationHandler"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus> AddEvaluationHandler<
            TEvaluationHandler>(this IServiceProvider serviceProvider)
                where TEvaluationHandler : IEvaluator<DegreeStatusInferenceRequest, DegreeStatus> =>
                    new (serviceProvider.CreateScope()
                        .ServiceProvider.GetRequiredService<TEvaluationHandler>());
    }
}