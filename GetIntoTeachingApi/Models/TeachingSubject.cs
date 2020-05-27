using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetIntoTeachingApi.Models
{
    public class TeachingSubject
    {
        [Column("dfe_teachingsubjectlistid")]
        public Guid Id { get; set; }
    }
}
