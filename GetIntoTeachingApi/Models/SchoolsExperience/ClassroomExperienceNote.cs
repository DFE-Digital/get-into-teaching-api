using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.SchoolsExperience
{
    public class ClassroomExperienceNote
    {
        public static readonly string Header = "RECORDED   ACTION                 EXP DATE   URN    NAME\r\n\r\n";
        public static readonly string EntryFormat = "{0,10} {1,-22} {2,10} {3,-6} {4}\r\n";
        private static readonly string DateFormat = "dd/MM/yyyy";

        [SwaggerSchema(Format = "date")]
        public DateTime? RecordedAt { get; set; }
        public string Action { get; set; }
        [SwaggerSchema(Format = "date")]
        public DateTime? Date { get; set; }
        public int? SchoolUrn { get; set; }
        public string SchoolName { get; set; }

        public ClassroomExperienceNote()
        {
        }

        public override string ToString()
        {
            return string.Format(
                EntryFormat,
                RecordedAt?.ToString(DateFormat),
                Action,
                Date?.ToString(DateFormat),
                SchoolUrn,
                SchoolName);
        }
    }
}
