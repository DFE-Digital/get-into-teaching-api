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
    /// 
    /// </summary>
    public sealed class DegreeStatusDomainService : IDegreeStatusDomainService
    {
        private readonly IList<IChainEvaluationHandler<
            DegreeStatusInferenceRequest, DegreeStatus>> _degreeStatusInferenceHandlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degreeStatusInferenceHandlers"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DegreeStatusDomainService(
            IEnumerable<IChainEvaluationHandler<DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers)
        {
            _degreeStatusInferenceHandlers = degreeStatusInferenceHandlers?.ToList() ??
                throw new ArgumentNullException(nameof(degreeStatusInferenceHandlers));

            _degreeStatusInferenceHandlers.ChainEvaluationHandlers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degreeStatusInferenceRequest"></param>
        /// <returns></returns>
        public int? GetInferredDegreeStatusFromGraduationYear(DegreeStatusInferenceRequest degreeStatusInferenceRequest)
        {
            DegreeStatus degreeStatusResult =
                _degreeStatusInferenceHandlers[0].Evaluate(degreeStatusInferenceRequest);

            return Convert.ToInt32(degreeStatusResult, CultureInfo.CurrentCulture);
        }
    }
}
