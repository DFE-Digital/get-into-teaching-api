namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators
{
    /// <summary>
    /// Request used to interact with degree status evaluators which encapsulates the year of graduation and the current year provider.
    /// </summary>
    public sealed class DegreeStatusInferenceRequest
    {
        /// <summary>
        /// Defines an immutable class for defining graduation year, as well as providing methods to convert and compare it.
        /// </summary>
        public GraduationYear YearOfGraduation { get; }

        /// <summary>
        /// Provides methods for converting the current <see cref="DateTimeOffset"/> to the integer based year format.
        /// </summary>
        public ICurrentYearProvider CurrentCalendarYearProvider { get; }

        /// <summary>
        /// Creates an immutable instance of <see cref="DegreeStatusInferenceRequest"/>.
        /// </summary>
        /// <param name="graduationYear">
        /// Defines an immutable class for defining graduation year.
        /// </param>
        /// <param name="currentYearProvider">
        /// Provides methods for converting the current
        /// <see cref="DateTimeOffset"/> to the integer based year format.
        /// </param>
        public DegreeStatusInferenceRequest(
            GraduationYear graduationYear, ICurrentYearProvider currentYearProvider)
        {
            YearOfGraduation = graduationYear;
            CurrentCalendarYearProvider = currentYearProvider;
        }

        /// <summary>
        /// Factory method to create an instance of <see cref="DegreeStatusInferenceRequest"/>.
        /// </summary>
        /// <param name="graduationYear">
        /// Defines an immutable class for defining graduation year.
        /// </param>
        /// <param name="currentYearProvider">
        /// Provides methods for converting the current
        /// <see cref="DateTimeOffset"/> to the integer based year format.
        /// </param>
        /// <returns>
        /// A configured instance of <see cref="DegreeStatusInferenceRequest"/>.
        /// </returns>
        public static DegreeStatusInferenceRequest Create
            (GraduationYear graduationYear, ICurrentYearProvider currentYearProvider) =>
                new(graduationYear, currentYearProvider);
    }
}
