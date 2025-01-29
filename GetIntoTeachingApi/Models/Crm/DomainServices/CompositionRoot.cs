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
    public class CompositionRoot
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
                where TEvaluationHandler : IEvaluator<GraduationYear, DegreeStatus> =>
                    services.AddScoped<IChainEvaluationHandler<GraduationYear, DegreeStatus>>(provider =>
                        provider.AddEvaluationHandler<TEvaluationHandler>());
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvaluationHandler"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ChainEvaluationHandler<GraduationYear, DegreeStatus> AddEvaluationHandler<
            TEvaluationHandler>(this IServiceProvider serviceProvider)
                where TEvaluationHandler : IEvaluator<GraduationYear, DegreeStatus> =>
                    ChainEvaluationHandler<GraduationYear, DegreeStatus>.Create(
                        serviceProvider.CreateScope()
                            .ServiceProvider.GetRequiredService<TEvaluationHandler>());
    }
}