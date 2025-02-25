using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators;
using GetIntoTeachingApi.Models.Crm.DomainServices.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// Defines a default implementation of <see cref="IDegreeStatusDomainService"/> which
    /// provides the functionality for inferring degree status based on the proposed graduation year.
    /// </summary>
    public sealed class DegreeStatusDomainService : IDegreeStatusDomainService
    {
        private readonly List<IChainEvaluationHandler<
            DegreeStatusInferenceRequest, DegreeStatus>> _degreeStatusInferenceHandlers;

        /// <summary>
        /// Constructor is injected with DI configured degree status evaluators chained to <see cref="IChainEvaluationHandler{TRequest, TResult}"/>
        /// which allows the request to traverse through the chain of evaluators in to infer the degree status, if possible.
        /// </summary>
        /// <param name="degreeStatusInferenceHandlers">
        /// The DI configured <see cref="IChainEvaluationHandler{TRequest, TResult}"/> implementations used to chain evaluators.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The error thrown if no handler can be derived into a list.
        /// </exception>
        public DegreeStatusDomainService(
            IEnumerable<IChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers)
        {
            _degreeStatusInferenceHandlers = degreeStatusInferenceHandlers?.ToList() ??
                throw new ArgumentNullException(nameof(degreeStatusInferenceHandlers));

            _degreeStatusInferenceHandlers.ChainEvaluationHandlers();
        }

        /// <summary>
        /// Gets the inferred degree status from the proposed graduation year.
        /// </summary>
        /// <param name="degreeStatusInferenceRequest">
        /// This request parameter encapsulates the proposed graduation year and
        /// the <see cref="ICurrentYearProvider"/> implementation which provides year based conversion methods.
        /// </param>
        /// <returns>
        /// The nullable integer representation of the inferred degree status.
        /// </returns>
        public int? GetInferredDegreeStatusFromGraduationYear(DegreeStatusInferenceRequest degreeStatusInferenceRequest)
        {
            DegreeStatus degreeStatusResult =
                _degreeStatusInferenceHandlers[0].Evaluate(degreeStatusInferenceRequest);

            return Convert.ToInt32(degreeStatusResult, CultureInfo.CurrentCulture);
        }
    }
}
