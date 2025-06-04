using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference;

/// <summary>
/// Abstract class for inferring the degree status based on the graduation year.
/// This class provides functionality to determine a student's degree status and
/// their inferred graduation date.
/// </summary>
public abstract class DegreeStatusInference
{
    /// <summary>
    /// The inferred degree status ID based on the graduation year.
    /// </summary>
    public int? DegreeStatusId { get; set; }

    /// <summary>
    /// The graduation year provided for inference.
    /// </summary>
    public virtual int? GraduationYear { get; set; } = null!;

    /// <summary>
    /// The inferred graduation date, calculated based on the graduation year.
    /// The default graduation date is set to August 31st of the given graduation year.
    /// </summary>
    public virtual DateTime? InferredGraduationDate { get; set; } = null!;

    /// <summary>
    /// Infers the degree status based on the graduation year, if provided.
    /// This method utilizes the given domain service to obtain the inferred degree status,
    /// while also calculating a standardized graduation date (August 31st).
    /// </summary>
    /// <param name="degreeStatusDomainService">
    /// Service responsible for determining the degree status based on the graduation year.
    /// </param>
    /// <param name="currentYearProvider">
    /// Utility that provides the current year and methods to process it.
    /// </param>
    /// <returns>
    /// The inferred degree status ID, or null if the graduation year is not provided.
    /// </returns>
    protected int? GetInferredDegreeStatus(
        IDegreeStatusDomainService degreeStatusDomainService,
        ICurrentYearProvider currentYearProvider)
    {
        // Ensure a graduation year is provided before attempting inference.
        if (GraduationYear != null)
        {
            // Define the standard graduation date (August 31st).
            const int GraduationDay = 31;
            const int GraduationMonth = 8;

            // Create a degree status inference request based on the graduation year.
            DegreeStatusInferenceRequest degreeStatusInferenceRequest =
                DegreeStatusInferenceRequest.Create(
                    new GraduationYear(GraduationYear.Value, currentYearProvider), currentYearProvider);

            // Use the domain service to infer the degree status based on the graduation year.
            DegreeStatusId =
                degreeStatusDomainService
                    .GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);

            // Set the inferred graduation date to the standardized graduation date.
            InferredGraduationDate =
                new DateTime(GraduationYear.Value, GraduationMonth, GraduationDay);
        }

        return DegreeStatusId;
    }
}
