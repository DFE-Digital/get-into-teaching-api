namespace GetIntoTeachingApi.Models.Crm.DomainServices.DegreeStatusInference
{
    /// <summary>
    /// Enumeration which defines the possible candidate degree stages (status) available.
    /// </summary>
    public enum DegreeStatus
    {
        /// <summary>
        /// Defines the 'HasDegree' status with the associated CRM code (222750000) applied.
        /// </summary>
        HasDegree = 222750000,
        /// <summary>
        /// Defines the 'FinalYear' status with the associated CRM code (222750001) applied.
        /// </summary>
        FinalYear = 222750001,
        /// <summary>
        /// Defines the 'SecondYear' status with the associated CRM code (222750002) applied.
        /// </summary>
        SecondYear = 222750002,
        /// <summary>
        /// Defines the 'FirstYear' status with the associated CRM code (222750003) applied.
        /// </summary>
        FirstYear = 222750003,
        /// <summary>
        /// Defines the 'NoDegree' status with the associated CRM code (222750004) applied.
        /// </summary>
        NoDegree = 222750004,
        /// <summary>
        /// Defines the 'Other' status with the associated CRM code (222750005) applied.
        /// </summary>
        Other = 222750005,
    }
}
