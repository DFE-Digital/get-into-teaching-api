namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference.Evaluators
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DegreeStatusInferenceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public GraduationYear YearOfGraduation { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICurrentYearProvider CurrentCalendarYearProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graduationYear"></param>
        /// <param name="currentYearProvider"></param>
        public DegreeStatusInferenceRequest(
            GraduationYear graduationYear, ICurrentYearProvider currentYearProvider)
        {
            YearOfGraduation = graduationYear;
            CurrentCalendarYearProvider = currentYearProvider;
        }

        /// <summary>
        /// Factory method to create an instance of <see cref="DegreeStatusInferenceRequest"/>.
        /// </summary>
        /// <param name="graduationYear"></param>
        /// <param name="currentYearProvider"></param>
        /// <returns></returns>
        public static DegreeStatusInferenceRequest Create
            (GraduationYear graduationYear, ICurrentYearProvider currentYearProvider) =>
                new(graduationYear, currentYearProvider);
    }
}
