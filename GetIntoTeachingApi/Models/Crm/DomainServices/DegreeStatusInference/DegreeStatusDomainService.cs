using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
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
        private readonly ICurrentYearProvider _currentYearProvider;

        private readonly IList<IChainEvaluationHandler<
            GraduationYear, DegreeStatus>> _degreeStatusInferenceHandlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degreeStatusInferenceHandlers"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DegreeStatusDomainService(
            ICurrentYearProvider currentYearProvider,
            IEnumerable<IChainEvaluationHandler<GraduationYear, DegreeStatus>> degreeStatusInferenceHandlers)
        {
            _currentYearProvider = currentYearProvider;

            _degreeStatusInferenceHandlers = degreeStatusInferenceHandlers?.ToList() ??
                throw new ArgumentNullException(nameof(degreeStatusInferenceHandlers));

            _degreeStatusInferenceHandlers.ChainEvaluationHandlers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graduationYear"></param>
        /// <returns></returns>
        public int? GetInferredDegreeStatusFromGraduationYear(GraduationYear graduationYear)
        {
            DegreeStatus degreeStatusResult =
                _degreeStatusInferenceHandlers[0].Evaluate(graduationYear);

            return Convert.ToInt32(degreeStatusResult, CultureInfo.CurrentCulture);
        }
    }
}
