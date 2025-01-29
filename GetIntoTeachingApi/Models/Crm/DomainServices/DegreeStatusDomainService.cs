using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DegreeStatusDomainService : IDegreeStatusDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graduationYear"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int? GetInferredDegreeStatusFromGraduationYear(GraduationYear graduationYear)
        {
            ArgumentNullException.ThrowIfNull(graduationYear);

            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        private enum DegreeStatus
        {
            HasDegree = 222750000,
            FinalYear = 222750001,
            SecondYear = 222750002,
            FirstYear = 222750003,
            NoDegree = 222750004,
            Other = 222750005,
        }
    }
}
