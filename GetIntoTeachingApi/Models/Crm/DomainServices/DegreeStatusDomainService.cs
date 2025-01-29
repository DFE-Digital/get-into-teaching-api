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
            int? rawGraduationYear = graduationYear.GetYear();

            //              Our academic_year will be defined as between September YEAR and August YEAR + 1

            //              If a user says they will graduate in YEAR + 1 => infer FinalYear

            //              If a user says they will graduate in YEAR + 2 => infer SecondYear

            //              If a user says they will graduate in YEAR + 3 => infer FirstYear

            //              If a user says they will graduate in YEAR > 3 => infer FirstYear

            //              If a user says they will graduate in YEAR => infer HasDegree(TBC)

            return rawGraduationYear;
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
